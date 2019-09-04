using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
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

            foreach (var position in ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions)
                results.Add(GetModelObjectViewModel(position));

            foreach (var transition in ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions)
                results.Add(GetModelObjectViewModel(transition));

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
                VisualViewModel.ClearVisualGroups();
                SynchronizeWithModel();

                VisualViewModel.AddVisualGroup(CreateCellFrameLineVisual().AsSingleton(),
                    Resources.DisplayName_ModelViewport_CellFrameLayer);

                foreach (var item in ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions)
                    VisualViewModel.AddVisualGroup(EnumeratePositionVisuals(item), item.Name, GetModelObjectViewModel(item).IsVisible);

                foreach (var item in ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions)
                    VisualViewModel.AddVisualGroup(EnumerateTransitionVisuals(item), item.Name, GetModelObjectViewModel(item).IsVisible);

                if (VisualViewModel.IsAutoUpdating) VisualViewModel.UpdateVisual();
            }
            catch (Exception e)
            {
                OnRenderError(e);
            }
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
        ///     Enumerates the <see cref="SphereVisual3D" /> objects for a <see cref="UnitCellPositionGraph" />
        /// </summary>
        /// <param name="positionGraph"></param>
        /// <returns></returns>
        private IEnumerable<SphereVisual3D> EnumeratePositionVisuals(UnitCellPositionGraph positionGraph)
        {
            var graphViewModel = GetModelObjectViewModel(positionGraph);
            var sourceVector = new Fractional3D(positionGraph.A, positionGraph.B, positionGraph.C);
            var (startVector, endVector) = RenderResourcesViewModel.GetRenderCuboidVectors();
            var cellPositions = SpaceGroupService.GetPositionsInCuboid(sourceVector, startVector, endVector);

            var brush = new SolidColorBrush(graphViewModel.Color);
            var generator = VisualViewModel.CreateSphereGenerator(graphViewModel.Scaling, brush, graphViewModel.MeshQuality);
            foreach (var center in cellPositions.Select(x => VectorTransformer.ToCartesian(x).AsPoint3D()))
            {
                var visual = VisualViewModel.CreateVisual(center, generator);
                yield return visual;
            }
        }

        /// <summary>
        ///     Enumerates the <see cref="ArrowVisual3D" /> objects for a <see cref="KineticTransitionGraph" />
        /// </summary>
        /// <param name="transitionGraph"></param>
        /// <returns></returns>
        private IEnumerable<ArrowVisual3D> EnumerateTransitionVisuals(KineticTransitionGraph transitionGraph)
        {
            var graphViewModel = GetModelObjectViewModel(transitionGraph);
            var vectorSequence = transitionGraph.PositionVectors.Select(x => new Fractional3D(x.A, x.B, x.C));

            var sequences = SpaceGroupService.GetAllWyckoffOriginSequences(vectorSequence);

            RemoveNegativeDirectionSequences(sequences);
            RemoveInverseSequences(sequences, SpaceGroupService.Comparer);

            var brush = new SolidColorBrush(graphViewModel.Color);
            var generator = VisualViewModel.CreateDirectionArrowGenerator(graphViewModel.Scaling, brush, graphViewModel.MeshQuality);
            foreach (var sequence in sequences)
            {
                for (var i = 1; i < sequence.Length; i++)
                {
                    var point1 = VectorTransformer.ToCartesian(sequence[i - 1]).AsPoint3D();
                    var point2 = VectorTransformer.ToCartesian(sequence[i]).AsPoint3D();
                    var direction = CreateDirectionWithPositionCorrection(point1, point2);
                    var visual = VisualViewModel.CreateVisual((point1, direction), generator);
                    yield return visual;
                }
            }
        }

        /// <summary>
        ///     Calculates the corrected direction <see cref="Vector3D" /> under consideration of existing positions spheres
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private Vector3D CreateDirectionWithPositionCorrection(in Point3D start, in Point3D end)
        {
            var direction = end - start;
            var length = direction.Length;
            direction.Normalize();

            var positionVisuals = VisualViewModel.VisualGroups.SelectMany(x => x.Items).Where(x => x is SphereVisual3D);
            var endVector = new Fractional3D(end.X, end.Y, end.Z);

            foreach (var visual in positionVisuals.Cast<SphereVisual3D>())
            {
                var centerVector = new Fractional3D(visual.Center.X, visual.Center.Y, visual.Center.Z);
                if (SpaceGroupService.Comparer.Compare(endVector, centerVector) == 0) return (length - visual.Radius) * direction;
            }

            return length * direction;
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
            var baseLinePairs = new[]
            {
                (new Fractional3D(0, 0, 0), new Fractional3D(1, 0, 0)),
                (new Fractional3D(0, 0, 0), new Fractional3D(0, 1, 0)),
                (new Fractional3D(0, 0, 0), new Fractional3D(0, 0, 1))
            };

            var points3D = new Point3DCollection();
            for (var a = minA; a <= maxA; a++)
            {
                for (var b = minC; b <= maxB; b++)
                {
                    for (var c = minC; c <= maxC; c++)
                    {
                        var shift = new Fractional3D(a, b, c);
                        foreach (var (startVector, endVector) in baseLinePairs.Select(x => (x.Item1 + shift, x.Item2 + shift)))
                        {
                            if (endVector.A > maxA || endVector.B > maxB || endVector.C > maxC) continue;
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
        ///     Removes the inverse duplicate entries form a list of <see cref="Fractional3D" /> sequences
        /// </summary>
        /// <param name="sequences"></param>
        /// <param name="comparer"></param>
        private void RemoveInverseSequences(IList<Fractional3D[]> sequences, IComparer<Fractional3D> comparer)
        {
            bool IsEqual(Fractional3D[] lhs, Fractional3D[] rhs)
            {
                for (var (i, j) = (0, rhs.Length - 1); i < rhs.Length; (i, j) = (i + 1, j - 1))
                    if (comparer.Compare(lhs[i], rhs[j]) != 0)
                        return false;

                return true;
            }

            sequences.RemoveDuplicates(new EqualityCompareAdapter<Fractional3D[]>(IsEqual, x => x.Length));
        }

        /// <summary>
        ///     Removes the negative direction entries form a list of <see cref="Fractional3D" /> sequences
        /// </summary>
        /// <param name="sequences"></param>
        private void RemoveNegativeDirectionSequences(IList<Fractional3D[]> sequences)
        {
            for (var i = sequences.Count - 1; i >= 0; i--)
            {
                var vector = sequences[i][sequences[i].Length - 1];
                if (vector.A < 0 || vector.B < 0 || vector.C < 0) sequences.RemoveAt(i);
            }
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