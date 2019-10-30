using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
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
using Mocassin.UI.Xml.Customization;
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
        ///     Get or set <see cref="IList{T}" /> of the position sin the base cuboid (In contrast to a unit cell the =1 values
        ///     are included)
        /// </summary>
        private IList<(Fractional3D Vector, ProjectObject3DViewModel ObjectViewModel)> BaseCuboidPositionData { get; set; }

        /// <summary>
        ///     Get the <see cref="Viewport3DViewModel" /> that manages the visual objects
        /// </summary>
        public Viewport3DViewModel VisualViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ModelRenderResourcesViewModel" /> that manages the user defined render resources
        /// </summary>
        public ModelRenderResourcesViewModel RenderResourcesViewModel { get; }

        /// <summary>
        ///     Provides an <see cref="ObservableCollectionViewModel{T}" /> for <see cref="ProjectObject3DViewModel" />
        /// </summary>
        public ObservableCollectionViewModel<ProjectObject3DViewModel> ModelObjectViewModels { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to update the model object render data list
        /// </summary>
        public RelayCommand UpdateObjectViewModelsCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to refresh the visual object layer contents
        /// </summary>
        public RelayCommand RefreshVisualGroupsCommand { get; }

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
            ModelObjectViewModels = new ObservableCollectionViewModel<ProjectObject3DViewModel>();
            RenderResourcesViewModel = new ModelRenderResourcesViewModel();
            UpdateObjectViewModelsCommand = new RelayCommand(SynchronizeWithModel);
            RefreshVisualGroupsCommand = new RelayCommand(RefreshVisualGroups);
        }

        /// <inheritdoc />
        public override async void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            await ExecuteIfConstantContentSource(RefreshVisualContent, TimeSpan.FromMilliseconds(250), true);
        }

        /// <summary>
        ///     Executes a full visual content update of the view model
        /// </summary>
        private void RefreshVisualContent()
        {
            if (ContentSource == null)
            {
                VisualViewModel.ClearVisualGroups();
                ModelObjectViewModels.ClearCollection();
                IsSynchronizedWithModel = true;
                return;
            }

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
        ///     Generates the <see cref="ProjectObject3DViewModel" /> instances for all displayable
        ///     <see cref="ExtensibleProjectObjectGraph" />
        ///     instances in the content source
        /// </summary>
        public void UpdateObjectViewModels()
        {
            if (ContentSource == null)
            {
                ModelObjectViewModels.ClearCollection();
                return;
            }

            var results = new List<ProjectObject3DViewModel>(ModelObjectViewModels.ObservableItems.Count)
            {
                GetProjectObjectViewModel(ContentSource.ProjectModelGraph.StructureModelGraph.StructureInfo)
            };
            results.AddRange(ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions.Select(GetProjectObjectViewModel));
            results.AddRange(ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions.Select(GetProjectObjectViewModel));

            ModelObjectViewModels.ClearCollection();
            ModelObjectViewModels.AddCollectionItems(results);
        }

        /// <summary>
        ///     Get the <see cref="ProjectObject3DViewModel" /> for the passed <see cref="ExtensibleProjectObjectGraph" /> or
        ///     creates a new one
        ///     if none exists
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private ProjectObject3DViewModel GetProjectObjectViewModel(ExtensibleProjectObjectGraph objectGraph)
        {
            return ModelObjectViewModels.ObservableItems.FirstOrDefault(x => x.ObjectGraph == objectGraph)
                   ?? new ProjectObject3DViewModel(objectGraph);
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
                BaseCuboidPositionData = CreateBaseCuboidPositionData();
                VisualViewModel.ClearVisualGroups();
                SynchronizeWithModel();

                VisualViewModel.AddVisualGroup(CreateCellFrameLineVisual(), Resources.DisplayName_ModelViewport_CellFrameLayer,
                    GetProjectObjectViewModel(ContentSource.ProjectModelGraph.StructureModelGraph.StructureInfo).IsVisible);

                foreach (var item in ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions)
                    VisualViewModel.AddVisualGroup(CreatePositionVisuals(item), item.Name, GetProjectObjectViewModel(item).IsVisible);

                foreach (var item in ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions)
                    VisualViewModel.AddVisualGroup(CreateTransitionVisuals(item), item.Name, GetProjectObjectViewModel(item).IsVisible);

                foreach (var item in ContentSource.ProjectCustomizationGraphs.FirstOrDefault()?.EnergyModelCustomization.StablePairEnergyParameterSets ?? new List<PairEnergySetGraph>())
                    VisualViewModel.AddVisualGroup(CreatePairInteractionVisuals(item), $"{item.Name}(stable)", false);

                foreach (var item in ContentSource.ProjectCustomizationGraphs.FirstOrDefault()?.EnergyModelCustomization.UnstablePairEnergyParameterSets ?? new List<PairEnergySetGraph>())
                    VisualViewModel.AddVisualGroup(CreatePairInteractionVisuals(item), $"{item.Name}(unstable)", false);

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
        ///     Creates the base cuboid position to view model data for vector hit testing and size correction
        /// </summary>
        /// <returns></returns>
        private IList<(Fractional3D, ProjectObject3DViewModel)> CreateBaseCuboidPositionData()
        {
            var (startVector, endVector) = RenderResourcesViewModel.GetRenderCuboidVectors();
            return ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions
                .Select(graph => (new Fractional3D(graph.A, graph.B, graph.C), GetProjectObjectViewModel(graph)))
                .SelectMany(tuple => SpaceGroupService.GetPositionsInCuboid(tuple.Item1, startVector, endVector).Select(vec => (vec, tuple.Item2)))
                .OrderBy(value => value.vec, SpaceGroupService.Comparer)
                .ToList();
        }

        /// <summary>
        ///     Creates a list of the <see cref="MeshGeometryVisual3D" /> objects for a <see cref="UnitCellPositionGraph" />
        /// </summary>
        /// <param name="positionGraph"></param>
        /// <returns></returns>
        private IList<MeshGeometryVisual3D> CreatePositionVisuals(UnitCellPositionGraph positionGraph)
        {
            var objectViewModel = GetProjectObjectViewModel(positionGraph);
            var sourceVector = new Fractional3D(positionGraph.A, positionGraph.B, positionGraph.C);
            var (startVector, endVector) = RenderResourcesViewModel.GetRenderCuboidVectors();
            var cellPositions = SpaceGroupService.GetPositionsInCuboid(sourceVector, startVector, endVector);

            var phiDiv = (int) (Settings.Default.Default_Render_Sphere_PhiDiv * objectViewModel.MeshQuality);
            var thetaDiv = (int) (Settings.Default.Default_Render_Sphere_ThetaDiv * objectViewModel.MeshQuality);
            var diameter = objectViewModel.Scaling;
            var visualFactory = VisualViewModel.BuildSphereVisualFactory(diameter, thetaDiv, phiDiv);

            var result = new List<MeshGeometryVisual3D>(cellPositions.Count);
            foreach (var center in cellPositions.Select(x => VectorTransformer.ToCartesian(x).AsPoint3D()))
            {
                var visual = VisualViewModel.CreateVisual(VisualViewModel.GetOriginOffsetTransform3D(center), visualFactory);
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
            var objectViewModel = GetProjectObjectViewModel(positionGraph);
            var sourceVector = new Fractional3D(positionGraph.A, positionGraph.B, positionGraph.C);
            var (startVector, endVector) = RenderResourcesViewModel.GetRenderCuboidVectors();
            var cellPositions = SpaceGroupService.GetPositionsInCuboid(sourceVector, startVector, endVector);

            var points = new Point3DCollection(cellPositions.Count);
            foreach (var center in cellPositions.Select(x => VectorTransformer.ToCartesian(x).AsPoint3D())) points.Add(center);

            var alpha = positionGraph.PositionStatus == PositionStatus.Unstable
                ? Settings.Default.Default_Render_UnstablePosition_Alpha
                : (byte) 255;

            var result = new PointsVisual3D {Size = 10 * objectViewModel.Scaling, Color = objectViewModel.Color.ChangeAlpha(alpha), Points = points};
            return result;
        }

        /// <summary>
        ///     Creates a list of the <see cref="MeshGeometryVisual3D" /> items for visualization of the provided
        ///     <see cref="KineticTransitionGraph" />
        /// </summary>
        /// <param name="transitionGraph"></param>
        /// <returns></returns>
        private IList<MeshGeometryVisual3D> CreateTransitionVisuals(KineticTransitionGraph transitionGraph)
        {
            var modelObjectVm = GetProjectObjectViewModel(transitionGraph);
            var fractionalPath = transitionGraph.PositionVectors.Select(x => new Fractional3D(x.A, x.B, x.C)).ToList();
            var pathTransforms = GetVisuallyUniqueP1PathTransformsForRenderArea(fractionalPath);
            var renderArea = RenderResourcesViewModel.GetRenderCuboidVectors();

            var headLength = Settings.Default.Default_Render_Arrow_HeadLength;
            var thetaDiv = (int) (Settings.Default.Default_Render_Arrow_ThetaDiv * modelObjectVm.MeshQuality);
            var diameter = modelObjectVm.Scaling;

            var result = new List<MeshGeometryVisual3D>(pathTransforms.Count);

            for (var i = 0; i < fractionalPath.Count - 1; i++)
            {
                var (startPoint, endPoint) = GetAtomRadiusCorrectedTransitionStepPoints(fractionalPath[i], fractionalPath[i + 1]);
                var visualFactory = VisualViewModel.BuildDualHeadedArrowVisualFactory(diameter, startPoint, endPoint, headLength, thetaDiv);

                var transformEnum = pathTransforms
                    .Select(transform3D => (transform3D, transform3D.Transform(startPoint), transform3D.Transform(endPoint)))
                    .Where(x => RenderAreaContainsPoint(x.Item2, renderArea.StartVector, renderArea.EndVector))
                    .Where(x => RenderAreaContainsPoint(x.Item3, renderArea.StartVector, renderArea.EndVector))
                    .Select(x => x.transform3D);

                result.AddRange(transformEnum.Select(transform3D => VisualViewModel.CreateVisual(transform3D, visualFactory)));
            }

            VisualViewModel.SetMeshGeometryVisualBrush(result, new SolidColorBrush(modelObjectVm.Color));
            return result;
        }

        /// <summary>
        ///     Creates a <see cref="LinesVisual3D" /> for visualization of a <see cref="PairEnergySetGraph" />
        /// </summary>
        /// <param name="energySetGraph"></param>
        /// <returns></returns>
        private IList<LinesVisual3D> CreatePairInteractionVisuals(PairEnergySetGraph energySetGraph)
        {
            var fractionalPath = energySetGraph.AsVectorPath().ToList();
            var transforms = GetVisuallyUniqueP1PathTransformsForRenderArea(fractionalPath);
            var (point0, point1) = (VectorTransformer.ToCartesian(fractionalPath[0]).AsPoint3D(), VectorTransformer.ToCartesian(fractionalPath[1]).AsPoint3D());

            var firstHalfPoints = new Point3DCollection(fractionalPath.Count * transforms.Count);
            var secondHalfPoints = new Point3DCollection(fractionalPath.Count * transforms.Count);
            var (renderAreaStart, renderAreaEnd) = RenderResourcesViewModel.GetRenderCuboidVectors();

            foreach (var (startPoint, endPoint) in transforms.Select(x => (x.Transform(point0), x.Transform(point1))))
            {
                if (!RenderAreaContainsPoint(endPoint, renderAreaStart, renderAreaEnd)) continue;
                if (!RenderAreaContainsPoint(startPoint, renderAreaStart, renderAreaEnd)) continue;
                var middlePoint = startPoint + 0.5 * (endPoint - startPoint);
                firstHalfPoints.Add(startPoint);
                firstHalfPoints.Add(middlePoint);
                secondHalfPoints.Add(middlePoint);
                secondHalfPoints.Add(endPoint);
            }
            var result0 = VisualViewModel.CreateVisual(Transform3D.Identity, VisualViewModel.BuildLinesVisualFactory(firstHalfPoints));
            var result1 = VisualViewModel.CreateVisual(Transform3D.Identity, VisualViewModel.BuildLinesVisualFactory(secondHalfPoints));

            var objectVm = GetProjectObjectViewModel(energySetGraph);
            var (startAtomVm, endAtomVm) = (HitTestAtomObject3DViewModel(fractionalPath[0]), HitTestAtomObject3DViewModel(fractionalPath[1]));

            objectVm.Scaling = 2;
            result0.Thickness = objectVm.Scaling;
            result0.Color = startAtomVm?.Color ?? objectVm.Color;
            result1.Thickness = objectVm.Scaling;
            result1.Color = endAtomVm?.Color ?? objectVm.Color;

            return new List<LinesVisual3D> {result0, result1};
        }

        /// <summary>
        ///     Creates a <see cref="MeshGeometryVisual3D" /> for visualization of a <see cref="PairEnergySetGraph" />
        /// </summary>
        /// <param name="energySetGraph"></param>
        /// <returns></returns>
        private IList<MeshGeometryVisual3D> CreateGroupInteractionVisuals(GroupEnergySetGraph energySetGraph)
        {
            var result = new List<MeshGeometryVisual3D>();
            var transforms = GetVisuallyUniqueP1PathTransformsForRenderArea(energySetGraph.AsVectorPath());
            return result;
        }

        /// <summary>
        ///     Get the extended unique set of <see cref="Transform3D" /> required for P1 extension of the passed path geometry to
        ///     the render area (Optional flag to get only transforms)
        /// </summary>
        /// <param name="pathGeometry"></param>
        /// <returns></returns>
        private IList<Transform3D> GetVisuallyUniqueP1PathTransformsForRenderArea(IEnumerable<Fractional3D> pathGeometry)
        {
            var cellTransforms = SpaceGroupService.GetMinimalUnitCellP1PathExtensionOperations(pathGeometry, true)
                .Select(x => x.ToTransform3D(VectorTransformer.FractionalSystem));
            return ExtendUnitCellTransformsToRenderArea(cellTransforms).ToList();
        }

        /// <summary>
        ///     Extends a sequence of <see cref="Transform3D" /> for the origin (0,0,0) unit cell to the set for the full render
        ///     area
        /// </summary>
        /// <param name="transforms"></param>
        /// <returns></returns>
        private IEnumerable<Transform3D> ExtendUnitCellTransformsToRenderArea(IEnumerable<Transform3D> transforms)
        {
            if (!(transforms is IReadOnlyCollection<Transform3D> transformCollection)) transformCollection = transforms.ToList();
            foreach (var (a, b, c) in EnumerateRenderedCellOffsets(false, true))
            {
                if (a == 0 && b == 0 && c == 0)
                {
                    foreach (var item in transformCollection)
                        yield return item;
                }

                var shift = VectorTransformer.ToCartesian(new Fractional3D(a, b, c));
                foreach (var transform in transformCollection)
                {
                    var matrix = transform.Value;
                    matrix.OffsetX += shift.X;
                    matrix.OffsetY += shift.Y;
                    matrix.OffsetZ += shift.Z;
                    var shiftedTransform = new MatrixTransform3D(matrix);
                    shiftedTransform.Freeze();
                    yield return shiftedTransform;
                }
            }
        }

        /// <summary>
        ///     Translates two <see cref="Fractional3D" /> that describe a transition step into two atom radii corrected
        ///     <see cref="Point3D" /> for arrow generation
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private (Point3D Point0, Point3D Point1) GetAtomRadiusCorrectedTransitionStepPoints(in Fractional3D start, in Fractional3D end)
        {
            var point0 = VectorTransformer.ToCartesian(start).AsPoint3D();
            var point1 = VectorTransformer.ToCartesian(end).AsPoint3D();
            var direction = point1 - point0;
            direction.Normalize();

            var startVm = HitTestAtomObject3DViewModel(start);
            var endVm = HitTestAtomObject3DViewModel(end);
            if (startVm != null) point0 += direction * startVm.Scaling;
            if (endVm != null) point1 -= direction * endVm.Scaling;
            return (point0, point1);
        }

        /// <summary>
        ///     Get the <see cref="ProjectObject3DViewModel"/> if the provided <see cref="Fractional3D"/> points to an atom position or null otherwise
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private ProjectObject3DViewModel HitTestAtomObject3DViewModel(in Fractional3D vector)
        {
            var trimmed = vector.TrimToUnitCell(0);
            return BaseCuboidPositionData.FirstOrDefault(tuple => SpaceGroupService.Comparer.Compare(tuple.Vector, trimmed) == 0).ObjectViewModel;
        }

        /// <summary>
        ///     Checks if a <see cref="Point3D" /> lies within the provided fractional render area boundaries (Optional strict
        ///     flags to make affiliated tests treat exact edge hits as failures)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="renderAreaStart"></param>
        /// <param name="renderAreaEnd"></param>
        /// <param name="includeUpper"></param>
        /// <param name="includeLower"></param>
        /// <returns></returns>
        private bool RenderAreaContainsPoint(in Point3D point, in Fractional3D renderAreaStart, in Fractional3D renderAreaEnd, bool includeLower = true,
            bool includeUpper = true)
        {
            var comparer = UtilityProject.GeometryNumeric.RangeComparer;
            var fractional3D = VectorTransformer.ToFractional(new Cartesian3D(point.X, point.Y, point.Z));

            var compValue = comparer.Compare(fractional3D.A, renderAreaStart.A);
            if (!(includeLower && compValue >= 0 || compValue > 0)) return false;

            compValue = comparer.Compare(fractional3D.B, renderAreaStart.B);
            if (!(includeLower && compValue >= 0 || compValue > 0)) return false;

            compValue = comparer.Compare(fractional3D.C, renderAreaStart.C);
            if (!(includeLower && compValue >= 0 || compValue > 0)) return false;

            compValue = comparer.Compare(fractional3D.A, renderAreaEnd.A);
            if (!(includeUpper && compValue <= 0 || compValue < 0)) return false;

            compValue = comparer.Compare(fractional3D.B, renderAreaEnd.B);
            if (!(includeUpper && compValue <= 0 || compValue < 0)) return false;

            compValue = comparer.Compare(fractional3D.C, renderAreaEnd.C);
            return includeUpper && compValue <= 0 || compValue < 0;
        }

        /// <summary>
        ///     Creates the <see cref="LinesVisual3D" /> that describes the unit cell cell frame of the current render area
        /// </summary>
        private LinesVisual3D CreateCellFrameLineVisual()
        {
            var objectVm = GetProjectObjectViewModel(ContentSource.ProjectModelGraph.StructureModelGraph.StructureInfo);
            var comparer = UtilityProject.GeometryNumeric.RangeComparer;
            var baseLinePairs = new[]
            {
                (new Fractional3D(0, 0, 0), new Fractional3D(1, 0, 0)),
                (new Fractional3D(0, 0, 0), new Fractional3D(0, 1, 0)),
                (new Fractional3D(0, 0, 0), new Fractional3D(0, 0, 1))
            };

            var points3D = new Point3DCollection();
            var max = RenderResourcesViewModel.GetRenderCuboidVectors().EndVector;
            foreach (var (a, b, c) in EnumerateRenderedCellOffsets(false, true))
            {
                var shift = new Fractional3D(a, b, c);
                foreach (var (start, end) in baseLinePairs.Select(x => (x.Item1 + shift, x.Item2 + shift)))
                {
                    if (comparer.Compare(end.A, max.A) > 0 || comparer.Compare(end.B, max.B) > 0 || comparer.Compare(end.C, max.C) > 0) continue;
                    points3D.Add(VectorTransformer.ToCartesian(start).AsPoint3D());
                    points3D.Add(VectorTransformer.ToCartesian(end).AsPoint3D());
                }
            }

            points3D.Freeze();
            var result = ExecuteOnDispatcher(() => new LinesVisual3D {Points = points3D, Color = objectVm.Color, Thickness = objectVm.Scaling});
            return result;
        }

        /// <summary>
        ///     Enumerates the (x,y,z) cell offsets of the current render area as index tuples with different properties
        /// </summary>
        /// <param name="skipOrigin"></param>
        /// <param name="includeMax"></param>
        /// <returns></returns>
        private IEnumerable<(int A, int B, int C)> EnumerateRenderedCellOffsets(bool skipOrigin = false, bool includeMax = false)
        {
            var (minA, minB, minC, maxA, maxB, maxC) = RenderResourcesViewModel.GetFlooredRenderArea(UtilityProject.GeometryNumeric.RangeComparer);
            if (!includeMax)
            {
                maxA--;
                maxB--;
                maxC--;
            }

            for (var a = minA; a <= maxA; a++)
            {
                for (var b = minB; b <= maxB; b++)
                {
                    for (var c = minC; c <= maxC; c++)
                    {
                        if (skipOrigin && a == 0 && b == 0 && c == 0) continue;
                        yield return (a, b, c);
                    }
                }
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