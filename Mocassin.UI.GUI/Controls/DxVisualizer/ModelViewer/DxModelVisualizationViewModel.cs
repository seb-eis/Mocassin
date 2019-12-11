using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Helper;
using Mocassin.UI.GUI.Controls.Visualizer.DataControl;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;
using Mocassin.UI.Xml.StructureModel;
using Mocassin.UI.Xml.TransitionModel;
using SharpDX;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> that controls object generation and supply for model components to
    ///     a <see cref="DxViewportViewModel" />
    /// </summary>
    public class DxModelVisualizationViewModel : ProjectGraphControlViewModel
    {
        private ProjectCustomizationGraph selectedCustomization;

        /// <summary>
        ///     Get or set the <see cref="IDisposable" /> that cancels the content synchronization for selectable
        ///     <see cref="ProjectCustomizationGraph" /> intances
        /// </summary>
        private IDisposable CustomizationLinkDisposable { get; set; }

        /// <summary>
        ///     Get the internal <see cref="IModelProject" /> that handles model data processing
        /// </summary>
        private IModelProject ModelProject { get; }

        /// <summary>
        ///     Get the <see cref="IVectorTransformer" /> that manages the transformations of the coordinate context
        /// </summary>
        private IVectorTransformer VectorTransformer => ModelProject.CrystalSystemService.VectorTransformer;

        /// <summary>
        ///     Get the <see cref="ISpaceGroupService" /> that provided the space group operations and functionality
        /// </summary>
        private ISpaceGroupService SpaceGroupService => ModelProject.SpaceGroupService;

        /// <summary>
        ///     Get the <see cref="DxViewportViewModel" /> that controls the 3D scene
        /// </summary>
        public DxViewportViewModel ViewportViewModel { get; }

        /// <summary>
        ///     Get the <see cref="Visualizer.DataControl.ModelRenderResourcesViewModel" /> that controls the model render
        ///     resources
        /// </summary>
        public ModelRenderResourcesViewModel ModelRenderResourcesViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of <see cref="ObjectRenderResourcesViewModel" /> that
        ///     manages the model object render settings
        /// </summary>
        public ObservableCollectionViewModel<ObjectRenderResourcesViewModel> ModelObjectResourcesViewModels { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of <see cref="ObjectRenderResourcesViewModel" /> that
        ///     manages the customization object render settings
        /// </summary>
        public ObservableCollectionViewModel<ObjectRenderResourcesViewModel> CustomizationObjectResourcesViewModels { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of selectable <see cref="ProjectCustomizationGraph" />
        ///     instances
        /// </summary>
        public ObservableCollectionViewModel<ProjectCustomizationGraph> CustomizationCollectionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="AsyncCommand" /> to invalidate and rebuild the entire scene
        /// </summary>
        public ParameterlessAsyncCommand InvalidateSceneCommand { get; }

        /// <summary>
        ///     Get or set the selected <see cref="ProjectCustomizationGraph" />
        /// </summary>
        public ProjectCustomizationGraph SelectedCustomization
        {
            get => selectedCustomization;
            set => SetProperty(ref selectedCustomization, value, OnSelectedCustomizationChanged);
        }

        /// <inheritdoc />
        public DxModelVisualizationViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ModelProject = projectControl.CreateModelProject();
            ViewportViewModel = new DxViewportViewModel();
            ModelRenderResourcesViewModel = new ModelRenderResourcesViewModel();
            ModelObjectResourcesViewModels = new ObservableCollectionViewModel<ObjectRenderResourcesViewModel>();
            CustomizationObjectResourcesViewModels = new ObservableCollectionViewModel<ObjectRenderResourcesViewModel>();
            CustomizationCollectionViewModel = new ObservableCollectionViewModel<ProjectCustomizationGraph>();
            InvalidateSceneCommand = new AsyncRelayCommand(InvalidateSceneAsync);
        }

        /// <inheritdoc />
        public override async void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            await ExecuteIfContentSourceUnchanged(() => Task.Run(ChangeContentSourceInternal), TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        ///     Internal implementation of content source change that updates the view model if a new
        ///     <see cref="MocassinProjectGraph" /> is set
        /// </summary>
        /// <returns></returns>
        public void ChangeContentSourceInternal()
        {
            if (ContentSource == null)
            {
                ClearContent();
                return;
            }

            RebuildContentAccess();
            InvalidateSceneCommand.ExecuteAsync();
        }

        /// <summary>
        ///     Invalidates and clears all visualization content sources and resets the viewport view model
        /// </summary>
        private void ClearContent()
        {
            ContentSource = null;
            SelectedCustomization = null;
            ModelRenderResourcesViewModel.SetDataSource(null);
            ModelObjectResourcesViewModels.Clear();
            CustomizationObjectResourcesViewModels.Clear();
            CustomizationCollectionViewModel.Clear();
            ViewportViewModel.Reset();
        }

        /// <summary>
        ///     Rebuilds and synchronizes the data access structure after a <see cref="MocassinProjectGraph" /> content source
        ///     change
        /// </summary>
        private void RebuildContentAccess()
        {
            RebuildModelAccess();
            RebuildCustomizationAccess();
            RebuildRenderResourceAccess();
        }

        /// <summary>
        ///     Rebuilds and synchronizes the <see cref="ProjectModelGraph" /> content access after the primary content source has
        ///     changed
        /// </summary>
        private void RebuildModelAccess()
        {
            ModelProject.ResetProject();
            if (ContentSource?.ProjectModelGraph == null) return;

            var spaceGroupData = ContentSource.ProjectModelGraph.StructureModelGraph.SpaceGroupInfo.GetInputObject();
            var cellData = ContentSource.ProjectModelGraph.StructureModelGraph.CellParameters.GetInputObject();
            ModelProject.InputPipeline.PushToProject(spaceGroupData);
            ModelProject.InputPipeline.PushToProject(cellData);
        }

        /// <summary>
        ///     Rebuilds and synchronizes the <see cref="ProjectCustomizationGraph" /> content access after the primary content
        ///     source has changed
        /// </summary>
        private void RebuildCustomizationAccess()
        {
            CustomizationLinkDisposable?.Dispose();
            CustomizationCollectionViewModel.Clear();
            if (ContentSource == null) return;
            CustomizationCollectionViewModel.AddItems(ProjectCustomizationGraph.Empty.AsSingleton().Concat(ContentSource.ProjectCustomizationGraphs));
            CustomizationLinkDisposable = CustomizationCollectionViewModel.ObservableItems.ListenToContentChanges(ContentSource.ProjectCustomizationGraphs);
            if (CustomizationCollectionViewModel.ObservableItems.Contains(SelectedCustomization)) return;
            SelectedCustomization = ProjectCustomizationGraph.Empty;
        }

        /// <summary>
        ///     Rebuilds and synchronizes all render resource content access objects after the primary content source has changed
        /// </summary>
        private void RebuildRenderResourceAccess()
        {
            RebuildModelRenderResourceAccess();
            RebuildCustomizationRenderResourceAccess();
        }

        /// <summary>
        ///     Rebuilds and synchronizes all render resource content access objects affiliated with the model source
        /// </summary>
        private void RebuildModelRenderResourceAccess()
        {
            ModelRenderResourcesViewModel.SetDataSource(ContentSource?.Resources);
            ModelObjectResourcesViewModels.Clear();
            if (ContentSource == null) return;
            var source = ContentSource.ProjectModelGraph;
            ModelObjectResourcesViewModels.AddItem(GetModelRenderResourcesViewModel(source.StructureModelGraph.StructureInfo));
            ModelObjectResourcesViewModels.AddItems(source.StructureModelGraph.UnitCellPositions.Select(GetModelRenderResourcesViewModel));
            ModelObjectResourcesViewModels.AddItems(source.TransitionModelGraph.KineticTransitions.Select(GetModelRenderResourcesViewModel));
        }

        /// <summary>
        ///     Rebuilds and synchronizes all render resource content access objects affiliated with the customization source
        /// </summary>
        private void RebuildCustomizationRenderResourceAccess()
        {
            CustomizationObjectResourcesViewModels.Clear();
            if (ContentSource == null || SelectedCustomization == null || ReferenceEquals(SelectedCustomization, ProjectCustomizationGraph.Empty)) return;
            var source = SelectedCustomization.EnergyModelCustomization;
            CustomizationObjectResourcesViewModels.AddItems(source.StablePairEnergyParameterSets.Select(GetCustomizationRenderResourcesViewModel));
            CustomizationObjectResourcesViewModels.AddItems(source.UnstablePairEnergyParameterSets.Select(GetCustomizationRenderResourcesViewModel));
            CustomizationObjectResourcesViewModels.AddItems(source.GroupEnergyParameterSets.Select(GetCustomizationRenderResourcesViewModel));
        }

        /// <summary>
        ///     Get the <see cref="ObjectRenderResourcesViewModel" /> for the provided <see cref="ExtensibleProjectObjectGraph" />
        ///     or creates the instance if required
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private ObjectRenderResourcesViewModel GetModelRenderResourcesViewModel(ExtensibleProjectObjectGraph objectGraph)
        {
            return GetObjectRenderViewModelAny(objectGraph, ModelObjectResourcesViewModels.ObservableItems);
        }

        /// <summary>
        ///     Get the <see cref="ObjectRenderResourcesViewModel" /> for the provided <see cref="ExtensibleProjectObjectGraph" />
        ///     or creates the instance if required
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private ObjectRenderResourcesViewModel GetCustomizationRenderResourcesViewModel(ExtensibleProjectObjectGraph objectGraph)
        {
            return GetObjectRenderViewModelAny(objectGraph, CustomizationObjectResourcesViewModels.ObservableItems);
        }

        /// <summary>
        ///     Get the <see cref="ObjectRenderResourcesViewModel" /> for the provided <see cref="ExtensibleProjectObjectGraph" />
        ///     from the provided source <see cref="ICollection{T}" /> or creates and adds a new one if required
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private ObjectRenderResourcesViewModel GetObjectRenderViewModelAny(ExtensibleProjectObjectGraph objectGraph,
            ICollection<ObjectRenderResourcesViewModel> source)
        {
            var result = source.FirstOrDefault(x => ReferenceEquals(x.ObjectGraph, objectGraph));
            if (result != null) return result;
            result = new ObjectRenderResourcesViewModel(objectGraph, GetVisualObjectCategory(objectGraph));
            source.Add(result);
            return result;
        }

        /// <summary>
        ///     Gets the <see cref="VisualObjectCategory" /> for the provided <see cref="ExtensibleProjectObjectGraph" />
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private VisualObjectCategory GetVisualObjectCategory(ExtensibleProjectObjectGraph objectGraph)
        {
            return objectGraph switch
            {
                StructureInfoGraph _ => VisualObjectCategory.CellFrame,
                KineticTransitionGraph _ => VisualObjectCategory.Transition,
                UnitCellPositionGraph _ => VisualObjectCategory.Position,
                PairEnergySetGraph _ => VisualObjectCategory.Interaction,
                GroupEnergySetGraph _ => VisualObjectCategory.Cluster,
                _ => VisualObjectCategory.Unknown
            };
        }

        /// <summary>
        ///     Handles the required actions if the target <see cref="ProjectCustomizationGraph" /> was changed
        /// </summary>
        private void OnSelectedCustomizationChanged()
        {
            RebuildCustomizationAccess();
        }

        /// <summary>
        ///     Action that handles a render <see cref="Exception" />
        /// </summary>
        /// <param name="e"></param>
        /// <param name="callerMemberName"></param>
        private void OnRenderError(Exception e, [CallerMemberName] string callerMemberName = null)
        {
            SendCallErrorMessage(e, callerMemberName);
            MessageBox.Show(Resources.Viewer3D_Error_Visual_Generation, Resources.Viewer3D_Error_Box_Caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        ///     Asynchronously clears the scene data, rebuilds the scene and passes the provided data to the viewport
        /// </summary>
        /// <returns></returns>
        private async Task InvalidateSceneAsync()
        {
            var watch = Stopwatch.StartNew();
            ViewportViewModel.Reset(false);
            SendCallInfoMessage($"Scene invalidation: Cleanup of old scene in {watch.Elapsed}.");
            watch.Restart();
            var sceneModels = await BuildSceneComponentsAsync();
            SendCallInfoMessage($"Scene invalidation: Rebuild of new scene in {watch.Elapsed}.");
            watch.Restart();
            ViewportViewModel.AddSceneElements(sceneModels);
            SendCallInfoMessage($"Scene invalidation: Rendering of new scene in {watch.Elapsed}.");
        }

        /// <summary>
        ///     Asynchronously builds the 3D scene <see cref="SceneNodeGroupModel3D" /> instances
        /// </summary>
        /// <returns></returns>
        private async Task<IList<SceneNodeGroupModel3D>> BuildSceneComponentsAsync()
        {
            var tasks = StartSceneComponentBuilding();
            await Task.WhenAll(tasks);
            return tasks.Select(x => x.Result).ToList(tasks.Count);
        }

        /// <summary>
        ///     Starts the parallel build process of the scene components and returns the <see cref="IList{T}" /> of build
        ///     <see cref="Task" /> instances
        /// </summary>
        /// <returns></returns>
        private IList<Task<SceneNodeGroupModel3D>> StartSceneComponentBuilding()
        {
            var renderBox = ModelRenderResourcesViewModel.GetRenderBox3D();
            var result = new List<Task<SceneNodeGroupModel3D>>(10);
            result.AddRange(StartCellPositionSceneBuilding(renderBox));
            return result;
        }

        /// <summary>
        ///     Starts the scene build tasks for all unit cell position in model of the current content source
        /// </summary>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private IEnumerable<Task<SceneNodeGroupModel3D>> StartCellPositionSceneBuilding(FractionalBox3D renderBox)
        {
            return ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions
                .Select(positionGraph => Task.Run(() => BuildCellPositionSceneNode(positionGraph, renderBox)));
        }

        /// <summary>
        ///     Builds a <see cref="SceneNodeGroupModel3D" /> for the provided <see cref="UnitCellPositionGraph" />. The scene is
        ///     extended based on the provided render box
        /// </summary>
        /// <param name="positionGraph"></param>
        /// <param name="renderBox"></param>
        /// <param name="useBatchedMesh"></param>
        /// <returns></returns>
        private SceneNodeGroupModel3D BuildCellPositionSceneNode(UnitCellPositionGraph positionGraph, in FractionalBox3D renderBox, bool useBatchedMesh = false)
        {
            var transformMatrices = GetCellPositionSceneItemTransforms(new Fractional3D(positionGraph.A, positionGraph.B, positionGraph.C), renderBox);
            var resourcesViewModel = GetModelRenderResourcesViewModel(positionGraph);
            var geometry = CreateSphereGeometry(resourcesViewModel);
            var material = CreateFrozenMaterial(resourcesViewModel);
            return BuildMeshSceneNode(geometry, material, transformMatrices, useBatchedMesh, x => x.IsHitTestVisible = false);
        }

        /// <summary>
        ///     Creates the <see cref="IList{T}" /> of <see cref="Matrix" /> transforms that represent the symmetry extension of
        ///     the origin point into the provided <see cref="FractionalBox3D" /> when applied to a 0,0,0 vector
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private IList<Matrix> GetCellPositionSceneItemTransforms(in Fractional3D origin, in FractionalBox3D renderBox)
        {
            var positions = SpaceGroupService.GetPositionsInCuboid(origin, renderBox);
            var result = new List<Matrix>(positions.Count);
            foreach (var fractional3D in positions)
            {
                var cartesian3D = VectorTransformer.ToCartesian(fractional3D);
                var matrix = Matrix.Translation((float) cartesian3D.X, (float) cartesian3D.Y, (float) cartesian3D.Z);
                result.Add(matrix);
            }

            return result;
        }

        /// <summary>
        ///     Creates a new <see cref="Material" /> based on the settings in the provided
        ///     <see cref="ObjectRenderResourcesViewModel" /> and freezes the object
        /// </summary>
        /// <param name="resourcesViewModel"></param>
        /// <returns></returns>
        private Material CreateFrozenMaterial(ObjectRenderResourcesViewModel resourcesViewModel)
        {
            var color = resourcesViewModel.Color.ToColor4();
            var result = PhongMaterials.PolishedSilver;
            result.DiffuseColor = color;
            result.Freeze();
            return result;
        }

        /// <summary>
        ///     Creates a new sphere <see cref="Geometry3D" /> object around (0,0,0) using the settings on the provided
        ///     <see cref="ObjectRenderResourcesViewModel" />
        /// </summary>
        /// <param name="resourcesViewModel"></param>
        /// <returns></returns>
        private Geometry3D CreateSphereGeometry(ObjectRenderResourcesViewModel resourcesViewModel)
        {
            var meshBuilder = new MeshBuilder();
            var radius = resourcesViewModel.Scaling;
            var thetaDiv = (Settings.Default.Default_Render_Sphere_ThetaDiv * resourcesViewModel.MeshQuality).FloorToInt();
            var phiDiv = (Settings.Default.Default_Render_Sphere_PhiDiv * resourcesViewModel.MeshQuality).FloorToInt();
            meshBuilder.AddSphere(new Vector3(), radius, thetaDiv, phiDiv);
            return meshBuilder.ToMesh();
        }

        /// <summary>
        ///     Creates a batched or loose <see cref="SceneNodeGroupModel3D" /> from a prepared mesh <see cref="Geometry3D" /> and
        ///     <see cref="Material" /> using a list of transform matrices
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transforms"></param>
        /// <param name="useBatchedMesh"></param>
        /// <param name="nodeCallback"></param>
        /// <returns></returns>
        private SceneNodeGroupModel3D BuildMeshSceneNode(Geometry3D geometry, Material material, IList<Matrix> transforms, bool useBatchedMesh,
            Action<SceneNode> nodeCallback = null)
        {
            var sceneBuilder = new DxSceneBuilder(useBatchedMesh ? 1 : transforms.Count);
            if (useBatchedMesh)
                sceneBuilder.AddBatchedMeshTransforms(geometry, material, transforms, nodeCallback);
            else
                sceneBuilder.AddMeshTransforms(geometry, material, transforms, nodeCallback);
            return sceneBuilder.ToModel();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            ViewportViewModel.Dispose();
            base.Dispose();
        }
    }
}