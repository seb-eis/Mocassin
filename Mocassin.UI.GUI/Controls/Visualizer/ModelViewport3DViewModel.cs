using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Visualizer.DataControl;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.GUI.Extensions;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;
using Mocassin.UI.Xml.StructureModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.Visualizer
{
    /// <summary>
    ///     A <see cref="ProjectGraphControlViewModel" /> to manages the provision of model data as visual 3D objects
    /// </summary>
    public class ModelViewport3DViewModel : ProjectGraphControlViewModel
    {
        private bool isRefreshingVisuals;

        /// <summary>
        ///     Get or set a boolean flag if the viewport is synchronized with the model data
        /// </summary>
        private bool IsSynchronizedWithModel { get; set; }

        /// <summary>
        ///     Get the <see cref="IModelProject" /> the visualizer uses for utility functions
        /// </summary>
        private IModelProject UtilityProject { get; }

        /// <summary>
        ///     Get the active <see cref="IVectorTransformer" /> matching the model content source
        /// </summary>
        private IVectorTransformer VectorTransformer => UtilityProject.CrystalSystemService.VectorTransformer;

        /// <summary>
        ///     Get the active <see cref="ISpaceGroupService" /> matching the model content source
        /// </summary>
        private ISpaceGroupService SpaceGroupService => UtilityProject.SpaceGroupService;

        /// <summary>
        ///     Get a <see cref="IList{T}" /> of the loaded space groups operations as <see cref="Transform3D" />
        /// </summary>
        private IList<Transform3D> GroupTransforms3D { get; set; }

        /// <summary>
        ///     Get the <see cref="Viewport3DViewModel" /> that manages the visual objects
        /// </summary>
        public Viewport3DViewModel VisualViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ModelRenderResourcesViewModel" /> that manages the user defined render resources
        /// </summary>
        public ModelRenderResourcesViewModel RenderResourcesViewModel { get; }

        /// <summary>
        ///     Provides an <see cref="ObservableCollectionViewModel{T}" /> for <see cref="ModelObject3DViewModel" />
        /// </summary>
        public ObservableCollectionViewModel<ModelObject3DViewModel> ModelObjectViewModels { get; }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to update the model object render data list
        /// </summary>
        public ParameterlessCommand UpdateObjectViewModelsCommand { get; }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to refresh the visual object layer contents
        /// </summary>
        public ParameterlessCommand RefreshVisualGroupsCommand { get; }

        /// <summary>
        ///     Get or set a boolean flag if the system is refreshing the visual data
        /// </summary>
        public bool IsRefreshingVisuals
        {
            get => isRefreshingVisuals;
            private set => SetProperty(ref isRefreshingVisuals, value);
        }

        /// <inheritdoc />
        public ModelViewport3DViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            UtilityProject = projectControl.CreateModelProject();
            VisualViewModel = new Viewport3DViewModel();
            ModelObjectViewModels = new ObservableCollectionViewModel<ModelObject3DViewModel>();
            RenderResourcesViewModel = new ModelRenderResourcesViewModel();
            UpdateObjectViewModelsCommand = new RelayCommand(SynchronizeWithModel);
            RefreshVisualGroupsCommand = new RelayCommand(RefreshVisualGroups);
        }

        /// <inheritdoc />
        public override async void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            if (ContentSource == null) return;
            await ExecuteIfConstantContentSource(RefreshVisualContent, TimeSpan.FromMilliseconds(250), true);
        }

        /// <summary>
        ///     Executes a full visual content update of the view model
        /// </summary>
        private void RefreshVisualContent()
        {
            IsSynchronizedWithModel = false;
            VisualViewModel.ClearVisual();
            RenderResourcesViewModel.ChangeDataSource(ContentSource?.Resources);
            SynchronizeWithModel();
            RefreshVisualGroups();
            VisualViewModel.UpdateVisual();
        }

        /// <summary>
        ///     Prepares the utility project for usage with the passed <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <param name="projectGraph"></param>
        private void PrepareUtilityProject(MocassinProjectGraph projectGraph)
        {
            UtilityProject.ResetProject();
            if (projectGraph == null) return;

            var spaceGroupData = projectGraph.ProjectModelGraph.StructureModelGraph.SpaceGroupInfo.GetInputObject();
            var cellData = projectGraph.ProjectModelGraph.StructureModelGraph.CellParameters.GetInputObject();
            UtilityProject.InputPipeline.PushToProject(spaceGroupData);
            UtilityProject.InputPipeline.PushToProject(cellData);
            GroupTransforms3D = SpaceGroupService.LoadedGroup.Operations
                .Select(x => x.ToTransform3D(VectorTransformer.FractionalSystem))
                .ToList();
        }

        /// <summary>
        ///     Generates the <see cref="ModelObject3DViewModel" /> instances for all displayable <see cref="ModelObjectGraph" />
        ///     instances in the content source
        /// </summary>
        public void UpdateObjectViewModels()
        {
            if (ContentSource == null)
            {
                ModelObjectViewModels.ClearCollection();
                return;
            }

            var results = new List<ModelObject3DViewModel>(ModelObjectViewModels.ObservableItems.Count);
            results.AddRange(ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions.Select(GetModelObjectViewModel));
            results.AddRange(ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions.Select(GetModelObjectViewModel));

            ModelObjectViewModels.ClearCollection();
            ModelObjectViewModels.AddCollectionItems(results);
        }

        /// <summary>
        ///     Get the <see cref="ModelObject3DViewModel" /> for the passed <see cref="ModelObjectGraph" /> or creates a new one
        ///     if none exists
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private ModelObject3DViewModel GetModelObjectViewModel(ModelObjectGraph objectGraph)
        {
            return ModelObjectViewModels.ObservableItems.FirstOrDefault(x => x.ObjectGraph == objectGraph)
                   ?? new ModelObject3DViewModel(objectGraph);
        }

        /// <summary>
        ///     Refreshes the set of <see cref="IVisualGroupViewModel" /> for each displayable model object
        /// </summary>
        public void RefreshVisualGroups()
        {
            if (ContentSource == null) return;

            try
            {
                IsRefreshingVisuals = true;
                VisualViewModel.ClearVisualGroups();
                SynchronizeWithModel();

                VisualViewModel.AddVisualGroup(CreateCellFrameLineVisual().AsSingleton(),
                    Resources.DisplayName_ModelViewport_CellFrameLayer);

                foreach (var item in ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions)
                    VisualViewModel.AddVisualGroup(CreatePositionVisuals(item), item.Name, GetModelObjectViewModel(item).IsVisible);

                foreach (var item in ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions)
                    VisualViewModel.AddVisualGroup(CreateTransitionVisuals(item), item.Name, GetModelObjectViewModel(item).IsVisible);

                if (VisualViewModel.IsAutoUpdating) VisualViewModel.UpdateVisual();
            }
            catch (Exception e)
            {
                OnRenderError(e);
            }

            IsRefreshingVisuals = false;
        }

        /// <summary>
        ///     Action to call to inform about an exception in the render process
        /// </summary>
        /// <param name="e"></param>
        /// <param name="callMemberName"></param>
        private void OnRenderError(Exception e, [CallerMemberName] string callMemberName = null)
        {
            SendCallErrorMessage(e, callMemberName);
            MessageBox.Show(Resources.Viewer3D_Error_Visual_Generation,
                Resources.Viewer3D_Error_Box_Caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        ///     Creates a list of the <see cref="MeshGeometryVisual3D" /> objects for a <see cref="UnitCellPositionGraph" />
        /// </summary>
        /// <param name="positionGraph"></param>
        /// <returns></returns>
        private IList<MeshGeometryVisual3D> CreatePositionVisuals(UnitCellPositionGraph positionGraph)
        {
            var objectViewModel = GetModelObjectViewModel(positionGraph);
            var sourceVector = new Fractional3D(positionGraph.A, positionGraph.B, positionGraph.C);
            var (startVector, endVector) = RenderResourcesViewModel.GetRenderCuboidVectors();
            var cellPositions = SpaceGroupService.GetPositionsInCuboid(sourceVector, startVector, endVector);

            var phiDiv = (int) (Settings.Default.Default_Render_Sphere_PhiDiv * objectViewModel.MeshQuality);
            var thetaDiv = (int) (Settings.Default.Default_Render_Sphere_ThetaDiv * objectViewModel.MeshQuality);
            var diameter = objectViewModel.Scaling;
            var visualFactory = VisualViewModel.GetSphereVisualFactory(diameter, thetaDiv, phiDiv);

            var result = new List<MeshGeometryVisual3D>(cellPositions.Count);
            foreach (var center in cellPositions.Select(x => VectorTransformer.ToCartesian(x).AsPoint3D()))
            {
                var visual = VisualViewModel.CreateVisual(center, visualFactory);
                result.Add(visual);
            }

            var alpha = positionGraph.PositionStatus == PositionStatus.Unstable
                ? Settings.Default.Default_Render_UnstablePosition_Alpha
                : (byte) 255;
            VisualViewModel.SetMeshGeometryVisualBrush(result, new SolidColorBrush(objectViewModel.Color.ChangeAlpha(alpha)));
            return result;
        }

        /// <summary>
        ///     Creates a single <see cref="PointsVisual3D" /> point cloud objects for a <see cref="UnitCellPositionGraph" />
        /// </summary>
        /// <param name="positionGraph"></param>
        /// <returns></returns>
        private PointsVisual3D CreatePositionVisualPointCloud(UnitCellPositionGraph positionGraph)
        {
            var objectViewModel = GetModelObjectViewModel(positionGraph);
            var sourceVector = new Fractional3D(positionGraph.A, positionGraph.B, positionGraph.C);
            var (startVector, endVector) = RenderResourcesViewModel.GetRenderCuboidVectors();
            var cellPositions = SpaceGroupService.GetPositionsInCuboid(sourceVector, startVector, endVector);

            var points = new Point3DCollection(cellPositions.Count);
            foreach (var center in cellPositions.Select(x => VectorTransformer.ToCartesian(x).AsPoint3D())) points.Add(center);
            var alpha = positionGraph.PositionStatus == PositionStatus.Unstable
                ? Settings.Default.Default_Render_UnstablePosition_Alpha
                : (byte) 255;

            var result = new PointsVisual3D
                {Size = 10 * objectViewModel.Scaling, Color = objectViewModel.Color.ChangeAlpha(alpha), Points = points};
            return result;
        }

        /// <summary>
        ///     Creates a list of the <see cref="MeshGeometryVisual3D" /> objects for a <see cref="KineticTransitionGraph" />
        /// </summary>
        /// <param name="transitionGraph"></param>
        /// <returns></returns>
        private IList<MeshGeometryVisual3D> CreateTransitionVisuals(KineticTransitionGraph transitionGraph)
        {
            var objectViewModel = GetModelObjectViewModel(transitionGraph);
            var pathPoints = transitionGraph.PositionVectors
                .Select(x => VectorTransformer.ToCartesian(new Fractional3D(x.A, x.B, x.C)).AsPoint3D())
                .ToList();

            var headLength = Settings.Default.Default_Render_Arrow_HeadLength;
            var thetaDiv = (int) (Settings.Default.Default_Render_Arrow_ThetaDiv * objectViewModel.MeshQuality);
            var diameter = objectViewModel.Scaling;

            var result = new List<MeshGeometryVisual3D>(GroupTransforms3D.Count * GroupTransforms3D.Count);

            for (var i = 0; i < pathPoints.Count - 1; i++)
            {
                var visualFactory = VisualViewModel.GetArrowVisualFactory(diameter, pathPoints[i], pathPoints[i + 1], headLength, thetaDiv);
                result.AddRange(GroupTransforms3D.Select(transform3D => VisualViewModel.CreateVisual(transform3D, visualFactory)));
            }

            VisualViewModel.SetMeshGeometryVisualBrush(result, new SolidColorBrush(objectViewModel.Color));
            return result;
        }


        /// <summary>
        ///     Creates the <see cref="LinesVisual3D" /> that describes the unit cell cell frame with the given size information
        /// </summary>
        /// <param name="minA"></param>
        /// <param name="minB"></param>
        /// <param name="minC"></param>
        /// <param name="maxA"></param>
        /// <param name="maxB"></param>
        /// <param name="maxC"></param>
        /// <returns></returns>
        private LinesVisual3D CreateCellFrameLineVisual(int minA, int minB, int minC, int maxA, int maxB, int maxC)
        {
            var comparer = UtilityProject.GeometryNumeric.RangeComparer;
            var baseLinePairs = new[]
            {
                (new Fractional3D(0, 0, 0), new Fractional3D(1, 0, 0)),
                (new Fractional3D(0, 0, 0), new Fractional3D(0, 1, 0)),
                (new Fractional3D(0, 0, 0), new Fractional3D(0, 0, 1))
            };

            var points3D = new Point3DCollection(baseLinePairs.Length * Math.Abs(maxA - minA) * Math.Abs(maxB - minB) * Math.Abs(maxC - minC));
            for (var a = minA; a <= maxA; a++)
            {
                for (var b = minB; b <= maxB; b++)
                {
                    for (var c = minC; c <= maxC; c++)
                    {
                        var shift = new Fractional3D(a, b, c);
                        foreach (var (startVector, endVector) in baseLinePairs.Select(x => (x.Item1 + shift, x.Item2 + shift)))
                        {
                            if (comparer.Compare(endVector.A, maxA) > 0 || comparer.Compare(endVector.B, maxB) > 0 || comparer.Compare(endVector.C, maxC) > 0) continue;
                            points3D.Add(VectorTransformer.ToCartesian(startVector).AsPoint3D());
                            points3D.Add(VectorTransformer.ToCartesian(endVector).AsPoint3D());
                        }
                    }
                }
            }

            return new LinesVisual3D {Points = points3D};
        }

        /// <summary>
        ///     Creates the <see cref="LinesVisual3D" /> that describes the unit cell cell frame with the current render range
        ///     information
        /// </summary>
        private LinesVisual3D CreateCellFrameLineVisual()
        {
            var (minA, minB, minC, maxA, maxB, maxC) =
                RenderResourcesViewModel.GetFlooredRenderArea(UtilityProject.CommonNumeric.RangeComparer);
            return CreateCellFrameLineVisual(minA, minB, minC, maxA, maxB, maxC);
        }

        /// <summary>
        ///     Synchronizes the viewport system with the model
        /// </summary>
        private void SynchronizeWithModel()
        {
            if (ContentSource != null && !IsSynchronizedWithModel)
            {
                PrepareUtilityProject(ContentSource);
                UpdateObjectViewModels();
            }

            IsSynchronizedWithModel = true;
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
        }

        /// <inheritdoc />
        protected override void OnProjectContentChangedInternal()
        {
            base.OnProjectContentChangedInternal();
            IsSynchronizedWithModel = false;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            VisualViewModel.Dispose();
            base.Dispose();
        }
    }
}