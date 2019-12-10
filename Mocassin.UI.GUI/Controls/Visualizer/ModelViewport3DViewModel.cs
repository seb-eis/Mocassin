using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        private ProjectCustomizationGraph selectedCustomizationGraph;

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
        private IList<(Fractional3D Vector, ObjectRenderResourcesViewModel ObjectViewModel)> BaseCuboidPositionData { get; set; }

        /// <summary>
        ///     Get the <see cref="Viewport3DViewModel" /> that manages the visual objects
        /// </summary>
        public Viewport3DViewModel VisualViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ModelRenderResourcesViewModel" /> that manages the user defined render resources
        /// </summary>
        public ModelRenderResourcesViewModel RenderResourcesViewModel { get; }

        /// <summary>
        ///     Provides an <see cref="ObservableCollectionViewModel{T}" /> for <see cref="ObjectRenderResourcesViewModel" /> of model
        ///     objects
        /// </summary>
        public ObservableCollectionViewModel<ObjectRenderResourcesViewModel> ModelObjectViewModels { get; }

        /// <summary>
        ///     Provides an <see cref="ObservableCollectionViewModel{T}" /> for <see cref="ObjectRenderResourcesViewModel" /> of
        ///     customization objects
        /// </summary>
        public ObservableCollectionViewModel<ObjectRenderResourcesViewModel> CustomizationObjectViewModels { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to update the model object render data list
        /// </summary>
        public RelayCommand UpdateObjectViewModelsCommand { get; }

        /// <summary>
        ///     Get a <see cref="AsyncRelayCommand" /> to refresh the visual object layer contents
        /// </summary>
        public AsyncRelayCommand RefreshVisualGroupsCommand { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}"/> instance for the selectable <see cref="ProjectCustomizationGraph"/> instances
        /// </summary>
        public ObservableCollectionViewModel<ProjectCustomizationGraph> SelectableCustomizationsViewModel { get; }


        /// <summary>
        ///     Get or set <see cref="ProjectCustomizationGraph" /> that is currently selected
        /// </summary>
        public ProjectCustomizationGraph SelectedCustomizationGraph
        {
            get => selectedCustomizationGraph;
            set => SetProperty(ref selectedCustomizationGraph, value);
        }

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
            ModelObjectViewModels = new ObservableCollectionViewModel<ObjectRenderResourcesViewModel>();
            CustomizationObjectViewModels = new ObservableCollectionViewModel<ObjectRenderResourcesViewModel>();
            SelectableCustomizationsViewModel = new ObservableCollectionViewModel<ProjectCustomizationGraph>();
            RenderResourcesViewModel = new ModelRenderResourcesViewModel();
            UpdateObjectViewModelsCommand = new RelayCommand(SynchronizeWithModel);
            RefreshVisualGroupsCommand = new AsyncRelayCommand(() => RefreshVisualGroups());
            PropertyChanged += OnCustomizationChanged;
        }

        /// <inheritdoc />
        public override async void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            await ExecuteIfContentSourceUnchanged(async () => await RefreshVisualContent(), TimeSpan.FromMilliseconds(250), true);
        }

        /// <summary>
        ///     Executes a full visual content update of the view model
        /// </summary>
        private async Task RefreshVisualContent()
        {
            if (ContentSource == null)
            {
                SelectedCustomizationGraph = null;
                VisualViewModel.ClearVisualGroups();
                ModelObjectViewModels.Clear();
                IsSynchronizedWithModel = true;
                return;
            }

            UpdateSelectableCustomizations();
            if (!SelectableCustomizationsViewModel.ObservableItems.Contains(SelectedCustomizationGraph))
                SelectedCustomizationGraph = ContentSource.ProjectCustomizationGraphs.FirstOrDefault();

            IsSynchronizedWithModel = false;
            VisualViewModel.ClearVisual();
            RenderResourcesViewModel.SetDataSource(ContentSource?.Resources);
            SynchronizeWithModel();
            await RefreshVisualGroups();
        }

        /// <summary>
        ///     Updates the collection of selectable <see cref="ProjectCustomizationGraph"/> instances
        /// </summary>
        private void UpdateSelectableCustomizations()
        {
            if (ContentSource == null)
            {
                SelectableCustomizationsViewModel.Clear();
                return;
            }

            SelectableCustomizationsViewModel.Clear();
            SelectableCustomizationsViewModel.AddItem(ProjectCustomizationGraph.Empty);
            SelectableCustomizationsViewModel.AddItems(ContentSource.ProjectCustomizationGraphs);
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
        ///     Generates the <see cref="ObjectRenderResourcesViewModel" /> instances for all displayable
        ///     <see cref="ExtensibleProjectObjectGraph" />
        ///     model object instances in the content source
        /// </summary>
        public void UpdateObjectViewModels()
        {
            if (ContentSource == null)
            {
                ModelObjectViewModels.Clear();
                return;
            }

            ModelObjectViewModels.Clear();
            ModelObjectViewModels.AddItem(GetProjectObjectViewModel(ContentSource.ProjectModelGraph.StructureModelGraph.StructureInfo));
            ModelObjectViewModels.AddItems(ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions.Select(GetProjectObjectViewModel));
            ModelObjectViewModels.AddItems(ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions.Select(GetProjectObjectViewModel));
        }

        /// <summary>
        ///     Generates the <see cref="ObjectRenderResourcesViewModel" /> instances for all displayable
        ///     <see cref="ExtensibleProjectObjectGraph" />
        ///     customization object instances in the selected customization
        /// </summary>
        public void UpdateCustomizationViewModels()
        {
            if (ContentSource == null || SelectedCustomizationGraph == null)
            {
                CustomizationObjectViewModels.Clear();
                return;
            }

            var energyCustomization = SelectedCustomizationGraph.EnergyModelCustomization;
            CustomizationObjectViewModels.Clear();
            CustomizationObjectViewModels.AddItems(energyCustomization.StablePairEnergyParameterSets.Select(GetProjectObjectViewModel));
            CustomizationObjectViewModels.AddItems(energyCustomization.UnstablePairEnergyParameterSets.Select(GetProjectObjectViewModel));
            CustomizationObjectViewModels.AddItems(energyCustomization.GroupEnergyParameterSets.Select(GetProjectObjectViewModel));
        }

        /// <summary>
        ///     Get the <see cref="ObjectRenderResourcesViewModel" /> for the passed <see cref="ExtensibleProjectObjectGraph" /> or
        ///     creates a new one
        ///     if none exists
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private ObjectRenderResourcesViewModel GetProjectObjectViewModel(ExtensibleProjectObjectGraph objectGraph)
        {
            var result = ModelObjectViewModels.ObservableItems.FirstOrDefault(x => x.ObjectGraph == objectGraph);
            if (result != null) return result;
            result = CustomizationObjectViewModels.ObservableItems.FirstOrDefault(x => x.ObjectGraph == objectGraph);
            if (result != null) return result;

            var objectCategory = objectGraph switch
            {
                StructureInfoGraph _ => VisualObjectCategory.Frame,
                KineticTransitionGraph _ => VisualObjectCategory.Transition,
                UnitCellPositionGraph _ => VisualObjectCategory.Position,
                PairEnergySetGraph _ => VisualObjectCategory.Interaction,
                GroupEnergySetGraph _ => VisualObjectCategory.Cluster,
                _ => VisualObjectCategory.Unknown
            };

            return new ObjectRenderResourcesViewModel(objectGraph, objectCategory);
        }

        /// <summary>
        ///     Refreshes the set of <see cref="IVisualGroupViewModel" /> with an option to force a visual update
        /// </summary>
        /// <param name="forceVisualUpdate"></param>
        /// <returns></returns>
        public async Task RefreshVisualGroups(bool forceVisualUpdate = false)
        {
            if (ContentSource == null) return;

            try
            {
                IsRefreshingVisuals = true;
                BaseCuboidPositionData = CreateBaseCuboidPositionData();
                VisualViewModel.ClearVisualGroups();
                SynchronizeWithModel();

                await Task.WhenAll(RefreshModelObjectVisualGroupsAsync(), RefreshCustomizationObjectVisualGroupsAsync());

                if (VisualViewModel.IsAutoUpdating || forceVisualUpdate) VisualViewModel.UpdateVisual();
            }
            catch (Exception e)
            {
                OnRenderError(e);
            }

            IsRefreshingVisuals = false;
        }

        /// <summary>
        ///     Refreshes the set of <see cref="IVisualGroupViewModel" /> for each displayable model object asynchronously
        /// </summary>
        private async Task RefreshModelObjectVisualGroupsAsync()
        {
            var structureInfo = ContentSource.ProjectModelGraph.StructureModelGraph.StructureInfo;
            var positionGraphs = ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions;
            var transitionGraphs = ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions;

            var cellFrameBuildTask = CreateCellFrameVisualAsync(structureInfo);
            var positionBuildTasks = positionGraphs.Select(CreatePositionVisualsAsync).ToList();
            var transitionBuildTasks = transitionGraphs.Select(CreateTransitionVisualsAsync).ToList();
            var awaitables = transitionBuildTasks.Cast<Task>().Concat(cellFrameBuildTask.AsSingleton()).Concat(positionBuildTasks);

            await Task.WhenAll(awaitables);

            VisualViewModel.AddVisualGroup(cellFrameBuildTask.Result, Resources.DisplayName_ModelViewport_CellFrameLayer,
                GetProjectObjectViewModel(structureInfo).IsVisible);

            for (var i = 0; i < positionGraphs.Count; i++)
                VisualViewModel.AddVisualGroup(positionBuildTasks[i].Result, positionGraphs[i].Name, GetProjectObjectViewModel(positionGraphs[i]).IsVisible);

            for (var i = 0; i < transitionGraphs.Count; i++)
                VisualViewModel.AddVisualGroup(transitionBuildTasks[i].Result, transitionGraphs[i].Name,
                    GetProjectObjectViewModel(transitionGraphs[i]).IsVisible);
        }

        /// <summary>
        ///     Refreshes the set of <see cref="IVisualGroupViewModel" /> for each displayable customization object asynchronously
        /// </summary>
        private async Task RefreshCustomizationObjectVisualGroupsAsync()
        {
            if (SelectedCustomizationGraph == null || ReferenceEquals(SelectedCustomizationGraph, ProjectCustomizationGraph.Empty)) return;

            var stableInteractions = SelectedCustomizationGraph.EnergyModelCustomization.StablePairEnergyParameterSets;
            var unstableInteractions = SelectedCustomizationGraph.EnergyModelCustomization.UnstablePairEnergyParameterSets;
            var groupInteractions = SelectedCustomizationGraph.EnergyModelCustomization.GroupEnergyParameterSets;

            var stableBuildTasks = stableInteractions.Select(CreatePairInteractionVisualsAsync).ToList();
            var unstableBuildTasks = unstableInteractions.Select(CreatePairInteractionVisualsAsync).ToList();
            var groupBuildTasks = groupInteractions.Select(CreateGroupInteractionVisualsAsync).ToList();

            var awaitables = stableBuildTasks.Concat(unstableBuildTasks).Concat(groupBuildTasks);

            await Task.WhenAll(awaitables);

            for (var i = 0; i < stableInteractions.Count; i++)
                VisualViewModel.AddVisualGroup(stableBuildTasks[i].Result, stableInteractions[i].Name, GetProjectObjectViewModel(stableInteractions[i]).IsVisible);

            for (var i = 0; i < unstableInteractions.Count; i++)
                VisualViewModel.AddVisualGroup(unstableBuildTasks[i].Result, unstableInteractions[i].Name, GetProjectObjectViewModel(unstableInteractions[i]).IsVisible);

            for (var i = 0; i < groupInteractions.Count; i++)
                VisualViewModel.AddVisualGroup(groupBuildTasks[i].Result, groupInteractions[i].Name, GetProjectObjectViewModel(groupInteractions[i]).IsVisible);
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
        private IList<(Fractional3D, ObjectRenderResourcesViewModel)> CreateBaseCuboidPositionData()
        {
            var (startVector, endVector) = RenderResourcesViewModel.GetRenderCuboidVectors();
            return ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions
                .Select(graph => (new Fractional3D(graph.A, graph.B, graph.C), GetProjectObjectViewModel(graph)))
                .SelectMany(tuple => SpaceGroupService.GetPositionsInCuboid(tuple.Item1, startVector, endVector).Select(vec => (vec, tuple.Item2)))
                .OrderBy(value => value.vec, SpaceGroupService.Comparer)
                .ToList();
        }

        /// <summary>
        ///     Creates the <see cref="ModelVisual3D" /> collection for a <see cref="UnitCellPositionGraph" /> visualization asynchronously
        /// </summary>
        /// <param name="positionGraph"></param>
        /// <returns></returns>
        private async Task<IReadOnlyList<ModelVisual3D>> CreatePositionVisualsAsync(UnitCellPositionGraph positionGraph)
        {
            var objectViewModel = GetProjectObjectViewModel(positionGraph);
            var positionTransforms = await Task.Run(() => CreatePositionVisualBuildData(new Fractional3D(positionGraph.A, positionGraph.B, positionGraph.C)));

            var phiDiv = (int) (Settings.Default.Default_Render_Sphere_PhiDiv * objectViewModel.MeshQuality);
            var thetaDiv = (int) (Settings.Default.Default_Render_Sphere_ThetaDiv * objectViewModel.MeshQuality);
            var renderSize = objectViewModel.Scaling;

            var visualFactory = positionGraph.PositionStatus == PositionStatus.Stable
                ? VisualViewModel.BuildSphereVisualFactory(renderSize, thetaDiv, phiDiv)
                : VisualViewModel.BuildCubeVisualFactory(renderSize);

            var result = new List<MeshGeometryVisual3D>(positionTransforms.Count);
            result.AddRange(positionTransforms.Select(x => VisualViewModel.CreateVisual(x, visualFactory)));

            VisualViewModel.SetMeshGeometryVisualBrush(result, new SolidColorBrush(objectViewModel.Color));
            return result;
        }

        /// <summary>
        ///     Creates the unique <see cref="Transform3D" /> collection to extend a <see cref="Fractional3D" /> source vector to
        ///     its render area symmetry equivalents (Does not require dispatcher execution)
        /// </summary>
        /// <param name="sourceVector"></param>
        /// <param name="freezeData"></param>
        /// <returns></returns>
        private IList<Transform3D> CreatePositionVisualBuildData(in Fractional3D sourceVector, bool freezeData = true)
        {
            var (renderAreaStart, renderAreaEnd) = RenderResourcesViewModel.GetRenderCuboidVectors();
            var extendedPositions = SpaceGroupService.GetPositionsInCuboid(sourceVector, renderAreaStart, renderAreaEnd);
            var result = new List<Transform3D>(extendedPositions.Count);
            var transforms = VisualViewModel.GetOriginOffsetTransforms(extendedPositions.Select(x => VectorTransformer.ToCartesian(x)).ToList(), freezeData);
            result.AddRange(transforms);
            return result;
        }

        /// <summary>
        ///     Creates a single <see cref="PointsVisual3D" /> point cloud objects for a <see cref="UnitCellPositionGraph" />
        ///     visualization
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
        ///     Creates the <see cref="ModelVisual3D" /> collection for a <see cref="KineticTransitionGraph" /> visualization
        ///     asynchronously
        /// </summary>
        /// <param name="transitionGraph"></param>
        /// <returns></returns>
        private async Task<IReadOnlyList<ModelVisual3D>> CreateTransitionVisualsAsync(KineticTransitionGraph transitionGraph)
        {
            var modelObjectVm = GetProjectObjectViewModel(transitionGraph);
            var fractionalPath = transitionGraph.PositionVectors.Select(x => new Fractional3D(x.A, x.B, x.C)).ToList();

            (Point3D StartPoint, Point3D EndPoint)[] pathPoints = null;
            var pathTransforms = await Task.Run(() => CreateTransitionVisualBuildData(fractionalPath, out pathPoints));

            var headLength = Settings.Default.Default_Render_Arrow_HeadLength;
            var thetaDiv = (int) (Settings.Default.Default_Render_Arrow_ThetaDiv * modelObjectVm.MeshQuality);
            var diameter = modelObjectVm.Scaling;

            var result = new List<MeshGeometryVisual3D>(pathTransforms.Count);
            for (var i = 0; i < pathPoints.Length; i++)
            {
                var visualFactory =
                    VisualViewModel.BuildDualHeadedArrowVisualFactory(diameter, pathPoints[i].StartPoint, pathPoints[i].EndPoint, headLength, thetaDiv);
                result.AddRange(pathTransforms[i].Select(transform3D => VisualViewModel.CreateVisual(transform3D, visualFactory)));
            }

            VisualViewModel.SetMeshGeometryVisualBrush(result, new SolidColorBrush(modelObjectVm.Color));
            return result;
        }

        /// <summary>
        ///     Creates the <see cref="Transform3D" /> and corrected <see cref="Point3D" /> raw data for transition path
        ///     visualization (Does not require dispatcher execution)
        /// </summary>
        /// <param name="fractionalPath"></param>
        /// <param name="atomRadiusCorrectedStepPoints"></param>
        /// <param name="freezeData"></param>
        /// <returns></returns>
        private IList<IList<Transform3D>> CreateTransitionVisualBuildData(IReadOnlyList<Fractional3D> fractionalPath,
            out (Point3D StartPoint, Point3D EndPoint)[] atomRadiusCorrectedStepPoints, bool freezeData = true)
        {
            var pathStepCount = fractionalPath.Count - 1;
            var rawPathTransforms = GetVisuallyUniqueP1PathTransformsForRenderArea(fractionalPath, freezeData);
            var (renderAreaStart, renderAreaEnd) = RenderResourcesViewModel.GetRenderCuboidVectors();

            atomRadiusCorrectedStepPoints = new (Point3D, Point3D)[pathStepCount];
            var result = new List<IList<Transform3D>>(pathStepCount);

            for (var i = 0; i < pathStepCount; i++)
            {
                var (startPoint, endPoint) = GetAtomRadiusCorrectedTransitionStepPoints(fractionalPath[i], fractionalPath[i + 1]);
                atomRadiusCorrectedStepPoints[i] = (startPoint, endPoint);

                var stepTransforms = new List<Transform3D>(rawPathTransforms.Count);
                var filteredTransforms = rawPathTransforms
                    .Select(transform3D => (transform3D, transform3D.Transform(startPoint), transform3D.Transform(endPoint)))
                    .Where(x => RenderAreaContainsPoint(x.Item2, renderAreaStart, renderAreaEnd) &&
                                RenderAreaContainsPoint(x.Item3, renderAreaStart, renderAreaEnd))
                    .Select(x => x.transform3D);

                stepTransforms.AddRange(filteredTransforms);
                result.Add(stepTransforms);
            }

            return result;
        }

        /// <summary>
        ///     Creates the <see cref="ModelVisual3D" /> collection for a <see cref="PairEnergySetGraph" /> visualization
        ///     asynchronously
        /// </summary>
        /// <param name="energySetGraph"></param>
        /// <returns></returns>
        private async Task<IReadOnlyList<ModelVisual3D>> CreatePairInteractionVisualsAsync(PairEnergySetGraph energySetGraph)
        {
            var fractionalPath = energySetGraph.AsVectorPath().ToList();

            var (firstPoints, secondPoints) = await Task.Run(() => CreatePairInteractionVisualBuildData(fractionalPath[0], fractionalPath[1]));

            var result0 = VisualViewModel.CreateVisual(Transform3D.Identity, VisualViewModel.BuildLinesVisualFactory(firstPoints));
            var result1 = VisualViewModel.CreateVisual(Transform3D.Identity, VisualViewModel.BuildLinesVisualFactory(secondPoints));

            var objectVm = GetProjectObjectViewModel(energySetGraph);
            var (startAtomVm, endAtomVm) = (HitTestAtomObject3DViewModel(fractionalPath[0]), HitTestAtomObject3DViewModel(fractionalPath[1]));

            result0.Thickness = objectVm.Scaling;
            result1.Thickness = objectVm.Scaling;
            result0.Color = objectVm.Color.A != byte.MaxValue ? startAtomVm.Color : objectVm.Color;
            result1.Color = objectVm.Color.A != byte.MaxValue ? endAtomVm.Color : objectVm.Color;

            return new[] {result0, result1};
        }

        /// <summary>
        ///     Creates the <see cref="Point3DCollection" /> items for first and second half for pair interaction network
        ///     visualization (Does not require dispatcher execution)
        /// </summary>
        /// <param name="startFractional"></param>
        /// <param name="endFractional"></param>
        /// <param name="freezeData"></param>
        /// <returns></returns>
        private (Point3DCollection, Point3DCollection) CreatePairInteractionVisualBuildData(in Fractional3D startFractional, in Fractional3D endFractional,
            bool freezeData = true)
        {
            var pathTransforms = GetVisuallyUniqueP1PathTransformsForRenderArea(new[] {startFractional, endFractional}, freezeData);
            var (point0, point1) = (VectorTransformer.ToCartesian(startFractional).AsPoint3D(), VectorTransformer.ToCartesian(endFractional).AsPoint3D());
            var (renderAreaStart, renderAreaEnd) = RenderResourcesViewModel.GetRenderCuboidVectors();

            var firstPoints = new Point3DCollection(2 * pathTransforms.Count);
            var secondPoints = new Point3DCollection(2 * pathTransforms.Count);

            foreach (var transform in pathTransforms)
            {
                var (startPoint, endPoint) = (transform.Transform(point0), transform.Transform(point1));
                if (!RenderAreaContainsPoint(startPoint, renderAreaStart, renderAreaEnd) ||
                    !RenderAreaContainsPoint(endPoint, renderAreaStart, renderAreaEnd)) continue;
                var middlePoint = startPoint + 0.5 * (endPoint - startPoint);
                firstPoints.Add(startPoint);
                firstPoints.Add(middlePoint);
                secondPoints.Add(middlePoint);
                secondPoints.Add(endPoint);
            }

            if (!freezeData) return (firstPoints, secondPoints);
            firstPoints.Freeze();
            secondPoints.Freeze();
            return (firstPoints, secondPoints);
        }

        /// <summary>
        ///     Creates the <see cref="ModelVisual3D" /> collection for visualization of a <see cref="PairEnergySetGraph" /> asynchronously
        /// </summary>
        /// <param name="energySetGraph"></param>
        /// <returns></returns>
        private async Task<IReadOnlyList<ModelVisual3D>> CreateGroupInteractionVisualsAsync(GroupEnergySetGraph energySetGraph)
        {
            var objectVm = GetProjectObjectViewModel(energySetGraph);
            var fractionalPath = energySetGraph.AsVectorPath(true).ToList();
            var points = fractionalPath.Select(x => VectorTransformer.ToCartesian(x).AsPoint3D()).ToList();
            var meshGeometry = await Task.Run(() => CreateGroupInteractionVisualNetworkMesh(fractionalPath));
            var visualFactory = VisualViewModel.BuildMeshVisualFactory(meshGeometry);

            var result = new List<MeshGeometryVisual3D>
            {
                VisualViewModel.CreateVisual(Transform3D.Identity, visualFactory)
            };
            VisualViewModel.SetMeshGeometryMaterial(result, MaterialHelper.CreateMaterial(new SolidColorBrush(objectVm.Color.ChangeAlpha(64)), 0, 255, true));
            return result;
        }

        /// <summary>
        ///     Creates a unified <see cref="MeshGeometry3D" /> required to describe the group interaction visualization (Far better performance than single transforms)
        /// </summary>
        /// <param name="fractionalPath"></param>
        /// <param name="freezeData"></param>
        /// <returns></returns>
        private MeshGeometry3D CreateGroupInteractionVisualNetworkMesh(IReadOnlyList<Fractional3D> fractionalPath, bool freezeData = true)
        {
            var transforms = CreateGroupInteractionVisualBuildData(fractionalPath, freezeData);
            var (renderAreaStart, renderAreaEnd) = RenderResourcesViewModel.GetRenderCuboidVectors();
            var points = fractionalPath.Select(x => VectorTransformer.ToCartesian(x).AsPoint3D()).ToList();
            var meshBuilder = new MeshBuilder();


            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i+1; j < points.Count; j++)
                {
                    for (var k = j+1; k < points.Count; k++)
                    {
                        foreach (var x in transforms)
                        {
                           meshBuilder.AddTriangle(x.Transform(points[i]), x.Transform(points[j]), x.Transform(points[k]));   
                        }
                    }
                }
            }
            return meshBuilder.ToMesh(freezeData);
        }

        /// <summary>
        ///     Creates the <see cref="Transform3D" /> required to describe group interaction visualization (Does not require dispatcher execution)
        /// </summary>
        /// <param name="fractionalPath"></param>
        /// <param name="freezeData"></param>
        /// <returns></returns>
        private IList<Transform3D> CreateGroupInteractionVisualBuildData(IReadOnlyList<Fractional3D> fractionalPath, bool freezeData = true)
        {
            var transforms = GetVisuallyUniqueP1PathTransformsForRenderArea(fractionalPath, freezeData);
            var (renderAreaStart, renderAreaEnd) = RenderResourcesViewModel.GetRenderCuboidVectors();
            var points = fractionalPath.Select(x => VectorTransformer.ToCartesian(x).AsPoint3D()).ToList();
            var result = new List<Transform3D>(transforms.Count);
            result.AddRange(transforms.Where(transform => points.All(x => RenderAreaContainsPoint(transform.Transform(x), renderAreaStart, renderAreaEnd))));
            return result;
        }

        /// <summary>
        ///     Creates the <see cref="ModelVisual3D" /> for a <see cref="StructureInfoGraph" /> cell frame visualization
        ///     asynchronously
        /// </summary>
        private async Task<ModelVisual3D> CreateCellFrameVisualAsync(StructureInfoGraph structureInfoGraph)
        {
            var objectVm = GetProjectObjectViewModel(structureInfoGraph);
            var framePoints = await Task.Run(() => CreateCellFrameVisualBuildData());
            var result = VisualViewModel.CreateVisual(Transform3D.Identity, VisualViewModel.BuildLinesVisualFactory(framePoints));
            result.Thickness = objectVm.Scaling;
            result.Color = objectVm.Color;
            return result;
        }

        /// <summary>
        ///     Creates the <see cref="Point3DCollection" /> required to describe the cell frame lines of the render area (Does not
        ///     require dispatcher execution)
        /// </summary>
        /// <param name="freezeData"></param>
        /// <returns></returns>
        private Point3DCollection CreateCellFrameVisualBuildData(bool freezeData = true)
        {
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

            if (freezeData) points3D.Freeze();
            return points3D;
        }

        /// <summary>
        ///     Get the extended unique set of <see cref="Transform3D" /> required for P1 extension of the passed path geometry to
        ///     the render area (Optional flag to freeze the objects)
        /// </summary>
        /// <param name="pathGeometry"></param>
        /// <param name="freezeTransforms"></param>
        /// <returns></returns>
        private IList<Transform3D> GetVisuallyUniqueP1PathTransformsForRenderArea(IEnumerable<Fractional3D> pathGeometry, bool freezeTransforms = true)
        {
            var cellTransforms = SpaceGroupService.GetMinimalUnitCellP1PathExtensionOperations(pathGeometry, true)
                .Select(x => x.ToTransform3D(VectorTransformer.FractionalSystem));
            if (freezeTransforms) cellTransforms = cellTransforms.Action(x => x.Freeze());
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
        ///     Get the <see cref="ObjectRenderResourcesViewModel" /> if the provided <see cref="Fractional3D" /> points to an atom
        ///     position or null otherwise
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private ObjectRenderResourcesViewModel HitTestAtomObject3DViewModel(in Fractional3D vector)
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
                if (!ReferenceEquals(SelectedCustomizationGraph, ProjectCustomizationGraph.Empty)) UpdateCustomizationViewModels();
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

        /// <summary>
        ///     Event reaction for changed <see cref="SelectedCustomizationGraph"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCustomizationChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(SelectedCustomizationGraph))
            {
                UpdateCustomizationViewModels();
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            VisualViewModel.Dispose();
            base.Dispose();
        }
    }
}