using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.Objects;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.DxVisualizer.Extensions;
using Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.DataControl;
using Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.Scene;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Base;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Enums;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Helper;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Scene;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;
using Mocassin.UI.Xml.StructureModel;
using Mocassin.UI.Xml.TransitionModel;
using SharpDX;
using SharpDX.Direct3D11;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> that controls object generation and supply for model components to
    ///     a <see cref="DxViewportViewModel" />
    /// </summary>
    public class DxModelSceneViewModel : ProjectGraphControlViewModel, IDxSceneController
    {
        private bool canInvalidateScene = true;
        private bool isInvalidScene;
        private bool isMatchInteractionToHit;
        private ProjectCustomizationTemplate selectedCustomization;

        /// <summary>
        ///     Get or set the <see cref="IDisposable" /> that cancels the content synchronization for selectable
        ///     <see cref="ProjectCustomizationTemplate" /> instances
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
        ///     Get or set a <see cref="SetList{T1}" /> that maps each <see cref="Fractional3D" /> of the unit cell to
        ///     its <see cref="CellReferencePositionData" />
        /// </summary>
        private SetList<KeyValuePair<Fractional3D, CellReferencePositionData>> CellReferencePositionCatalog { get; }

        /// <summary>
        ///     Get the <see cref="IDxSceneHost" /> that hosts the created scene elements
        /// </summary>
        public IDxSceneHost SceneHost { get; }

        /// <summary>
        ///     Get the <see cref="DxModelControlViewModel" /> that controls the model render configuration
        /// </summary>
        public DxModelControlViewModel ModelControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="DxCustomizationControlViewModel" /> that controls the model customization render configuration
        /// </summary>
        public DxCustomizationControlViewModel CustomizationControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of selectable <see cref="ProjectCustomizationTemplate" />
        ///     instances
        /// </summary>
        public ObservableCollectionViewModel<ProjectCustomizationTemplate> SelectableCustomizations { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> for <see cref="IDxMeshItemConfig" /> accesses for positions
        /// </summary>
        public ObservableCollectionViewModel<IDxMeshItemConfig> PositionItemConfigs { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> for <see cref="IDxMeshItemConfig" /> accesses for
        ///     transitions
        /// </summary>
        public ObservableCollectionViewModel<IDxMeshItemConfig> TransitionItemConfigs { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> for <see cref="IDxMeshItemConfig" /> accesses for pair
        ///     interactions
        /// </summary>
        public ObservableCollectionViewModel<IDxMeshItemConfig> PairInteractionItemConfigs { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> for <see cref="IDxMeshItemConfig" /> accesses for group
        ///     interactions
        /// </summary>
        public ObservableCollectionViewModel<IDxMeshItemConfig> GroupInteractionItemConfigs { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> for <see cref="IDxLineItemConfig" /> accesses for line
        ///     items
        /// </summary>
        public ObservableCollectionViewModel<IDxLineItemConfig> LineItemConfigs { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to invalidate and rebuild the entire scene
        /// </summary>
        public ICommand InvalidateSceneCommand { get; }

        /// <summary>
        ///     Get or set the selected <see cref="ProjectCustomizationTemplate" />
        /// </summary>
        public ProjectCustomizationTemplate SelectedCustomization
        {
            get => selectedCustomization;
            set => SetProperty(ref selectedCustomization, value, OnSelectedCustomizationChanged);
        }

        /// <summary>
        ///     Get or set a boolean flag that indicates if interaction configurations should be overwritten by the position
        ///     configs affiliated with them
        /// </summary>
        public bool IsMatchInteractionToHit
        {
            get => isMatchInteractionToHit;
            set => SetProperty(ref isMatchInteractionToHit, value, MarkSceneAsInvalid);
        }

        /// <inheritdoc />
        public bool CanInvalidateScene
        {
            get => canInvalidateScene && ContentSource != null;
            protected set => SetProperty(ref canInvalidateScene, value);
        }

        /// <inheritdoc />
        public bool IsInvalidScene
        {
            get => isInvalidScene;
            protected set => SetProperty(ref isInvalidScene, value);
        }

        /// <inheritdoc />
        public DxModelSceneViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            ModelProject = projectControl.CreateModelProject();
            SceneHost = new DxViewportViewModel();
            SelectableCustomizations = new ObservableCollectionViewModel<ProjectCustomizationTemplate>();
            InvalidateSceneCommand = new AsyncRelayCommand(async () => await Task.Run(InvalidateSceneInternalAsync), () => CanInvalidateScene);
            CellReferencePositionCatalog = new SetList<KeyValuePair<Fractional3D, CellReferencePositionData>>(CreatePositionCatalogComparer());
            PositionItemConfigs = new ObservableCollectionViewModel<IDxMeshItemConfig>();
            TransitionItemConfigs = new ObservableCollectionViewModel<IDxMeshItemConfig>();
            PairInteractionItemConfigs = new ObservableCollectionViewModel<IDxMeshItemConfig>();
            GroupInteractionItemConfigs = new ObservableCollectionViewModel<IDxMeshItemConfig>();
            LineItemConfigs = new ObservableCollectionViewModel<IDxLineItemConfig>();
            ModelControlViewModel = new DxModelControlViewModel(projectControl, this);
            CustomizationControlViewModel = new DxCustomizationControlViewModel(projectControl, this);
            SceneHost.AttachController(this);
        }

        /// <inheritdoc />
        public IEnumerable<VvmContainer> GetControlContainers()
        {
            yield return new VvmContainer(new DxModelControlView(), ModelControlViewModel, "Model Control");
            yield return new VvmContainer(new DxCustomizationControlView(), CustomizationControlViewModel, "Customization Control");
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            ClearContent();
            ModelControlViewModel.Dispose();
            CustomizationControlViewModel.Dispose();
            SceneHost.DetachController();
            SceneHost.Dispose();
            base.Dispose();
        }

        /// <inheritdoc />
        public override async void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            await ExecuteIfContentSourceUnchanged(() => Task.Run(ChangeContentSourceInternal), TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        ///     Performs all tasks associated with invalidating and rebuilding the scene
        /// </summary>
        public void InvalidateScene()
        {
            if (!CanInvalidateScene) return;
            InvalidateSceneCommand.Execute(null);
        }

        /// <summary>
        ///     Internal implementation of content source change that updates the view model if a new
        ///     <see cref="MocassinProject" /> is set
        /// </summary>
        /// <returns></returns>
        public void ChangeContentSourceInternal()
        {
            if (ContentSource == null)
            {
                ClearContent();
                return;
            }

            ModelControlViewModel.ChangeContentSource(ContentSource);
            CustomizationControlViewModel.ChangeContentSource(ContentSource);
            RebuildContentAccess();
            InvalidateScene();
        }

        /// <summary>
        ///     Invalidates and clears all visualization content sources and resets the viewport view model
        /// </summary>
        private void ClearContent()
        {
            ContentSource = null;
            SelectedCustomization = null;
            ModelControlViewModel.ChangeContentSource(null);
            CustomizationControlViewModel.ChangeContentSource(null);
            ClearSceneConfigs();

            SelectableCustomizations.Clear();
            CellReferencePositionCatalog.Clear();
            SceneHost.ResetScene(false);
        }

        /// <summary>
        ///     Disposes and clears all <see cref="IDxSceneItemConfig" /> containers
        /// </summary>
        private void ClearSceneConfigs()
        {
            PositionItemConfigs.DisposeAllAndClear();
            TransitionItemConfigs.DisposeAllAndClear();
            PairInteractionItemConfigs.DisposeAllAndClear();
            GroupInteractionItemConfigs.DisposeAllAndClear();
            LineItemConfigs.DisposeAllAndClear();
        }

        /// <summary>
        ///     Detaches all <see cref="SceneNode" /> instances from their affiliated <see cref="IDxSceneItemConfig" />
        /// </summary>
        private void DetachSceneNodesFromConfigs()
        {
            PositionItemConfigs.ObservableItems.Action(x => x.DetachAll()).Load();
            TransitionItemConfigs.ObservableItems.Action(x => x.DetachAll()).Load();
            PairInteractionItemConfigs.ObservableItems.Action(x => x.DetachAll()).Load();
            GroupInteractionItemConfigs.ObservableItems.Action(x => x.DetachAll()).Load();
            LineItemConfigs.ObservableItems.Action(x => x.DetachAll()).Load();
        }

        /// <summary>
        ///     Rebuilds and synchronizes the data access structure after a <see cref="MocassinProject" /> content source
        ///     change
        /// </summary>
        private void RebuildContentAccess()
        {
            RebuildModelAccess();
            RebuildCustomizationAccess();
            RebuildRenderResourceAccess();
        }

        /// <summary>
        ///     Rebuilds and synchronizes the <see cref="ProjectModelData" /> content access after the primary content source has
        ///     changed
        /// </summary>
        private void RebuildModelAccess()
        {
            ModelProject.ResetProject();
            if (ContentSource?.ProjectModelData == null) return;

            var spaceGroupData = ContentSource.ProjectModelData.StructureModelData.SpaceGroupInfo.GetInputObject();
            var cellData = ContentSource.ProjectModelData.StructureModelData.CellParameters.GetInputObject();
            ModelProject.InputPipeline.PushToProject(spaceGroupData);
            ModelProject.InputPipeline.PushToProject(cellData);
        }

        /// <summary>
        ///     Rebuilds and synchronizes the <see cref="ProjectCustomizationTemplate" /> content access after the primary content
        ///     source has changed
        /// </summary>
        private void RebuildCustomizationAccess()
        {
            CustomizationLinkDisposable?.Dispose();
            SelectableCustomizations.Clear();
            SelectableCustomizations.AddItem(ProjectCustomizationTemplate.Empty);
            if (ContentSource == null) return;
            SelectableCustomizations.AddItems(ContentSource.CustomizationTemplates);
            CustomizationLinkDisposable = SelectableCustomizations.ObservableItems.SubscribeToContentChanges(ContentSource.CustomizationTemplates);
            if (SelectableCustomizations.ObservableItems.Contains(SelectedCustomization)) return;
            SelectedCustomization = ProjectCustomizationTemplate.Empty;
        }

        /// <summary>
        ///     Rebuilds and synchronizes all render resource content access objects after the primary content source has changed
        /// </summary>
        private void RebuildRenderResourceAccess()
        {
            RebuildModelRenderResourceAccess();
            RebuildCustomizationRenderResourceAccess();
            RebuildHitTestResources();
        }

        /// <summary>
        ///     Rebuilds and synchronizes all render resource content access objects affiliated with the model source
        /// </summary>
        private void RebuildModelRenderResourceAccess()
        {
            PositionItemConfigs.DisposeAllAndClear();
            TransitionItemConfigs.DisposeAllAndClear();
            LineItemConfigs.DisposeAllAndClear();
            if (ContentSource == null) return;
            var source = ContentSource.ProjectModelData;

            GetLineItemConfigLazy(source.StructureModelData.StructureInfo);
            source.StructureModelData.CellReferencePositions.Select(GetMeshItemConfigLazy).Load();
            source.TransitionModelData.KineticTransitions.Select(GetMeshItemConfigLazy).Load();

            RebuildHitTestResources();
        }

        /// <summary>
        ///     Rebuilds and synchronizes all hit test resources affiliated with the model source
        /// </summary>
        private void RebuildHitTestResources()
        {
            CellReferencePositionCatalog.Clear();
            if (ContentSource == null) return;

            foreach (var cellPosition in ContentSource.ProjectModelData.StructureModelData.CellReferencePositions)
            {
                foreach (var vector in SpaceGroupService.GetUnitCellP1PositionExtension(new Fractional3D(cellPosition.A, cellPosition.B, cellPosition.C)))
                    CellReferencePositionCatalog.Add(new KeyValuePair<Fractional3D, CellReferencePositionData>(vector, cellPosition));
            }
        }

        /// <summary>
        ///     Rebuilds and synchronizes all render resource content access objects affiliated with the customization source
        /// </summary>
        private void RebuildCustomizationRenderResourceAccess()
        {
            PairInteractionItemConfigs.DisposeAllAndClear();
            GroupInteractionItemConfigs.DisposeAllAndClear();
            if (ContentSource == null || SelectedCustomization == null || ReferenceEquals(SelectedCustomization, ProjectCustomizationTemplate.Empty)) return;

            var source = SelectedCustomization.EnergyModelCustomization;
            source.StablePairEnergyParameterSets.Select(GetMeshItemConfigLazy).Load();
            source.UnstablePairEnergyParameterSets.Select(GetMeshItemConfigLazy).Load();
            source.GroupEnergyParameterSets.Select(GetMeshItemConfigLazy).Load();
        }

        /// <summary>
        ///     Gets the <see cref="IDxMeshItemConfig" /> for the provided <see cref="ExtensibleProjectDataObject" /> from the
        ///     correct container or creates one if required
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        private IDxMeshItemConfig GetMeshItemConfigLazy(ExtensibleProjectDataObject dataObject)
        {
            IDxMeshItemConfig CreateNew() =>
                new DxProjectMeshObjectSceneConfig(dataObject, GetVisualObjectCategory(dataObject))
                {
                    OnChangeInvalidatesNode = MarkSceneAsInvalid,
                    CanResizeMeshAtOrigin = dataObject is CellReferencePositionData
                };

            bool CheckMatch(IDxSceneItemConfig config) => config.CheckAccess(dataObject);

            return dataObject switch
            {
                CellReferencePositionData _ => PositionItemConfigs.FirstOrNew(CheckMatch, CreateNew),
                PairEnergySetData _ => PairInteractionItemConfigs.FirstOrNew(CheckMatch, CreateNew),
                KineticTransitionData _ => TransitionItemConfigs.FirstOrNew(CheckMatch, CreateNew),
                GroupEnergySetData _ => GroupInteractionItemConfigs.FirstOrNew(CheckMatch, CreateNew),
                _ => throw new InvalidOperationException("Provided object does not support a mesh config.")
            };
        }

        /// <summary>
        ///     Gets the <see cref="IDxLineItemConfig" /> for the provided <see cref="ExtensibleProjectDataObject" /> from the
        ///     correct container or creates one if required
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        private IDxLineItemConfig GetLineItemConfigLazy(ExtensibleProjectDataObject dataObject)
        {
            IDxLineItemConfig CreateNew() => new DxProjectLineObjectSceneConfig(dataObject, GetVisualObjectCategory(dataObject))
                {OnChangeInvalidatesNode = MarkSceneAsInvalid};

            bool CheckMatch(IDxSceneItemConfig config) => config.CheckAccess(dataObject);

            return dataObject switch
            {
                StructureInfoData _ => LineItemConfigs.FirstOrNew(CheckMatch, CreateNew),
                _ => throw new InvalidOperationException("Provided object does not support a line config.")
            };
        }

        /// <summary>
        ///     Gets the <see cref="VisualObjectCategory" /> for the provided <see cref="ExtensibleProjectDataObject" />
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        private VisualObjectCategory GetVisualObjectCategory(ExtensibleProjectDataObject dataObject)
        {
            return dataObject switch
            {
                StructureInfoData _ => VisualObjectCategory.LineGrid,
                KineticTransitionData _ => VisualObjectCategory.DoubleArrow,
                CellReferencePositionData _ => VisualObjectCategory.Sphere,
                PairEnergySetData _ => VisualObjectCategory.Cylinder,
                GroupEnergySetData _ => VisualObjectCategory.PolygonSet,
                _ => VisualObjectCategory.Unknown
            };
        }

        /// <summary>
        ///     Handles the required actions if the target <see cref="ProjectCustomizationTemplate" /> was changed
        /// </summary>
        private void OnSelectedCustomizationChanged()
        {
            RebuildCustomizationRenderResourceAccess();
            IsInvalidScene = true;
        }

        /// <summary>
        ///     Action that handles a render <see cref="Exception" />
        /// </summary>
        /// <param name="e"></param>
        /// <param name="callerMemberName"></param>
        private void OnRenderError(Exception e, [CallerMemberName] string callerMemberName = null)
        {
            PushErrorMessage(e, callerMemberName);
            MessageBox.Show(Properties.Resources.Viewer3D_Error_Visual_Generation, Properties.Resources.Viewer3D_Error_Box_Caption, MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        ///     Marks the currently supplied scene as invalid without triggering invalidation actions
        /// </summary>
        public void MarkSceneAsInvalid()
        {
            IsInvalidScene = true;
        }

        /// <summary>
        ///     Asynchronously clears the scene data, rebuilds the scene and passes the provided data to the scene host
        /// </summary>
        /// <returns></returns>
        private async Task InvalidateSceneInternalAsync()
        {
            if (!CanInvalidateScene) return;
            CanInvalidateScene = false;
            try
            {
                var watch = Stopwatch.StartNew();
                DetachSceneNodesFromConfigs();
                SceneHost.ResetScene(false);
                var cleanTime = watch.Elapsed.TotalMilliseconds;
                watch.Restart();

                var sceneModel = await BuildSceneAsync();
                var buildTime = watch.Elapsed.TotalMilliseconds;
                watch.Restart();

                SceneHost.AddSceneItem(sceneModel);
                var setupTime = watch.Elapsed.TotalMilliseconds;
                PushInfoMessage(
                    $"Scene Rebuilt Report: {cleanTime:0.0} ms for cleanup; {buildTime:0.0} ms for building new models; {setupTime:0.0} ms for viewport setup;");
                IsInvalidScene = false;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                IsInvalidScene = true;
                OnRenderError(exception);
            }
            finally
            {
                CanInvalidateScene = true;
            }
        }

        /// <summary>
        ///     Asynchronously builds the 3D scene <see cref="SceneNodeGroupModel3D" /> instance that holds the complete scene
        /// </summary>
        /// <returns></returns>
        private async Task<SceneNodeGroupModel3D> BuildSceneAsync()
        {
            var sceneBuilder = StartSceneBuilder();
            var result = await sceneBuilder.ToModelAsync();
            return result;
        }

        /// <summary>
        ///     Starts the parallel build process of the scene components and provides the <see cref="DxSceneBuilder" /> to await
        ///     completion
        /// </summary>
        /// <returns></returns>
        private DxSceneBuilder StartSceneBuilder()
        {
            ModelControlViewModel.LoadSourceData();
            var renderBox = ModelControlViewModel.GetRenderBox3D();
            var sceneBuilder = new DxSceneBuilder(100);
            StartCellFrameSceneNodeBuilding(sceneBuilder, renderBox);
            StartCellPositionSceneNodeBuilding(sceneBuilder, renderBox);
            StartTransitionSceneNodeBuilding(sceneBuilder, renderBox);
            StartPairInteractionSceneNodeBuilding(sceneBuilder, renderBox);
            StartGroupInteractionSceneNodeBuilding(sceneBuilder, renderBox);
            return sceneBuilder;
        }

        /// <summary>
        ///     Builds the scene component that holds the cell frame to the provided <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void AddCellFrameSceneNode(DxSceneBuilder sceneBuilder, in FractionalBox3D renderBox)
        {
            var itemConfig = GetLineItemConfigLazy(ContentSource.ProjectModelData.StructureModelData.StructureInfo);
            if (itemConfig.IsInactive) return;

            var material = itemConfig.CreateMaterial();
            var matrix = VectorTransformer.FractionalSystem.ToCartesianMatrix.ToDxMatrix();

            var lineBuilder = new LineBuilder();
            var (aMax, bMax, cMax) = (renderBox.End.A.FloorToInt(), renderBox.End.B.FloorToInt(), renderBox.End.C.FloorToInt());
            var (aMin, bMin, cMin) = (renderBox.Start.A.CeilToInt(), renderBox.Start.B.CeilToInt(), renderBox.Start.C.CeilToInt());
            for (var a = aMin; a <= aMax; a++)
            {
                for (var b = bMin; b <= bMax; b++)
                {
                    for (var c = cMin; c <= cMax; c++)
                    {
                        var origin = new Vector3(a, b, c);
                        if (a < aMax) lineBuilder.AddLine(origin, new Vector3(a + 1, b, c));
                        if (b < bMax) lineBuilder.AddLine(origin, new Vector3(a, b + 1, c));
                        if (c < cMax) lineBuilder.AddLine(origin, new Vector3(a, b, c + 1));
                    }
                }
            }

            var configurator = GetSceneNodeConfigurator(itemConfig);
            sceneBuilder.AddLineGeometry(lineBuilder.ToLineGeometry3D(), material, matrix, configurator);
        }

        /// <summary>
        ///     Starts the scene build task for the cell wire-frame in model of the current content source using the provided
        ///     <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void StartCellFrameSceneNodeBuilding(DxSceneBuilder sceneBuilder, FractionalBox3D renderBox)
        {
            sceneBuilder.AttachCustomTask(Task.Run(() => AddCellFrameSceneNode(sceneBuilder, renderBox)));
        }


        /// <summary>
        ///     Starts the scene build tasks for all unit cell position in model of the current content source using the provided
        ///     <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void StartCellPositionSceneNodeBuilding(DxSceneBuilder sceneBuilder, FractionalBox3D renderBox)
        {
            foreach (var item in ContentSource.ProjectModelData.StructureModelData.CellReferencePositions)
                sceneBuilder.AttachCustomTask(Task.Run(() => AddCellPositionNodeToScene(sceneBuilder, item, renderBox)));
        }

        /// <summary>
        ///     Prepares the scene elements for the provided <see cref="CellReferencePositionData" /> and adds the node one the
        ///     <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="referencePositionData"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void AddCellPositionNodeToScene(DxSceneBuilder sceneBuilder, CellReferencePositionData referencePositionData, in FractionalBox3D renderBox)
        {
            var itemConfig = GetMeshItemConfigLazy(referencePositionData);
            if (itemConfig.IsInactive) return;

            var matrices = GetPositionSceneItemTransforms(new Fractional3D(referencePositionData.A, referencePositionData.B, referencePositionData.C),
                renderBox);
            var geometry = CreateAtomGeometry(itemConfig);
            var material = itemConfig.CreateMaterial();
            var configurator = GetSceneNodeConfigurator(itemConfig, true);
            AddMeshElementsToScene(sceneBuilder, geometry, material, matrices, configurator);
        }

        /// <summary>
        ///     Creates an array of <see cref="Matrix" /> transforms that represent the symmetry extension of
        ///     the origin point into the provided <see cref="FractionalBox3D" /> when applied to a (0,0,0) cartesian vector
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private Matrix[] GetPositionSceneItemTransforms(in Fractional3D origin, in FractionalBox3D renderBox)
        {
            var positions = SpaceGroupService.GetPositionsInCuboid(origin, renderBox);
            var result = new Matrix[positions.Count];
            for (var i = 0; i < positions.Count; i++)
            {
                var cartesian3D = VectorTransformer.ToCartesian(positions[i]);
                result[i] = Matrix.Translation((float) cartesian3D.X, (float) cartesian3D.Y, (float) cartesian3D.Z);
            }

            return result;
        }

        /// <summary>
        ///     Prepares the scene elements for the provided <see cref="KineticTransitionData" /> and adds the node one the
        ///     <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="transitionData"></param>
        /// <param name="renderBox"></param>
        private void AddTransitionNodeToScene(DxSceneBuilder sceneBuilder, KineticTransitionData transitionData, in FractionalBox3D renderBox)
        {
            var itemConfig = GetMeshItemConfigLazy(transitionData);
            if (itemConfig.IsInactive) return;

            var path = transitionData.PathVectors.Select(x => new Fractional3D(x.A, x.B, x.C)).ToList();
            var matrices = GetPathSceneItemTransforms(path, renderBox, false, out var anyFlipsOrientation);
            var geometry = CreateTransitionGeometry(itemConfig, path);
            var material = itemConfig.CreateMaterial();
            var configurator = GetSceneNodeConfigurator(itemConfig, !anyFlipsOrientation);
            AddMeshElementsToScene(sceneBuilder, geometry, material, matrices, configurator);
        }

        /// <summary>
        ///     Creates the <see cref="IList{T}" /> of <see cref="Matrix" /> transforms that represent the symmetry extension of
        ///     the path vectors into the provided <see cref="FractionalBox3D" /> when applied to the original path in cartesian
        ///     coordinates
        /// </summary>
        /// <param name="path"></param>
        /// <param name="renderBox"></param>
        /// <param name="filterGeometricDuplicates"></param>
        /// <param name="anyFlipsOrientation"></param>
        /// <returns></returns>
        private Matrix[] GetPathSceneItemTransforms(IEnumerable<Fractional3D> path, in FractionalBox3D renderBox, bool filterGeometricDuplicates,
            out bool anyFlipsOrientation)
        {
            anyFlipsOrientation = false;
            return ModelControlViewModel.PathSymmetryExtensionMode switch
            {
                PathSymmetryExtensionMode.None => new[] {Matrix.Identity},
                PathSymmetryExtensionMode.Local => GetPathSceneItemTransformsLocal(path, filterGeometricDuplicates, out anyFlipsOrientation),
                PathSymmetryExtensionMode.Full => GetPathSceneItemTransformsFull(path, renderBox, out anyFlipsOrientation),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        ///     Creates the <see cref="IList{T}" /> of <see cref="Matrix" /> transforms that represent the symmetry extension of
        ///     the path vectors on the start position (Local extension)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filterGeometricDuplicates"></param>
        /// <param name="anyFlipsOrientation"></param>
        /// <returns></returns>
        private Matrix[] GetPathSceneItemTransformsLocal(IEnumerable<Fractional3D> path, bool filterGeometricDuplicates, out bool anyFlipsOrientation)
        {
            var pathList = path.AsList();
            var cartesianMatrix = VectorTransformer.FractionalSystem.ToCartesianMatrix.ToDxMatrix();
            var fractionalMatrix = VectorTransformer.FractionalSystem.ToFractionalMatrix.ToDxMatrix();
            var firstVector = pathList[0];
            var vectorComparer = new VectorComparer3D<Fractional3D>(ModelProject.GeometryNumeric.RangeComparer);
            var result = SpaceGroupService.GetMinimalUnitCellP1PathExtensionOperations(pathList, filterGeometricDuplicates)
                                          .Where(x => vectorComparer.Compare(x.Transform(firstVector).TrimToUnitCell(vectorComparer.ValueComparer),
                                              firstVector) == 0)
                                          .Select(x => fractionalMatrix * x.ToDxMatrix() * cartesianMatrix)
                                          .ToArray();

            anyFlipsOrientation = result.Any(x => x.FlipsOrientation());
            return result;
        }

        /// <summary>
        ///     Creates the <see cref="IList{T}" /> of <see cref="Matrix" /> transforms that represent the symmetry extension of
        ///     the path vectors into the provided <see cref="FractionalBox3D" /> when applied to the original path in cartesian
        ///     coordinates (Full extension)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="renderBox"></param>
        /// <param name="anyFlipsOrientation"></param>
        /// <returns></returns>
        private Matrix[] GetPathSceneItemTransformsFull(IEnumerable<Fractional3D> path, in FractionalBox3D renderBox, out bool anyFlipsOrientation)
        {
            var toCartesianMatrix = VectorTransformer.FractionalSystem.ToCartesianMatrix.ToDxMatrix();
            var toFractionalMatrix = VectorTransformer.FractionalSystem.ToFractionalMatrix.ToDxMatrix();
            var baseMatrices = SpaceGroupService.GetMinimalUnitCellP1PathExtensionOperations(path, true).Select(x => x.ToDxMatrix()).ToList();
            var baseSetSize = baseMatrices.Count;

            var (aMax, bMax, cMax) = (renderBox.End.A.FloorToInt(), renderBox.End.B.FloorToInt(), renderBox.End.C.FloorToInt());
            var (aMin, bMin, cMin) = (renderBox.Start.A.CeilToInt(), renderBox.Start.B.CeilToInt(), renderBox.Start.C.CeilToInt());
            var result = new Matrix[baseSetSize * (aMax - aMin) * (bMax - bMin) * (cMax - cMin)];
            var index = 0;
            for (var baseIndex = 0; baseIndex < baseSetSize; baseIndex++)
            {
                for (var a = aMin; a < aMax; a++)
                {
                    for (var b = bMin; b < bMax; b++)
                    {
                        for (var c = cMin; c < cMax; c++)
                        {
                            var baseMatrix = baseMatrices[baseIndex];
                            baseMatrix.M41 += a;
                            baseMatrix.M42 += b;
                            baseMatrix.M43 += c;
                            result[index] = toFractionalMatrix * baseMatrix * toCartesianMatrix;
                            index++;
                        }
                    }
                }
            }

            anyFlipsOrientation = baseMatrices.Any(x => x.FlipsOrientation());
            return result;
        }

        /// <summary>
        ///     Starts the scene build tasks for all kinetic transitions in model of the current content source using the provided
        ///     <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void StartTransitionSceneNodeBuilding(DxSceneBuilder sceneBuilder, FractionalBox3D renderBox)
        {
            foreach (var transitionGraph in ContentSource.ProjectModelData.TransitionModelData.KineticTransitions)
                sceneBuilder.AttachCustomTask(Task.Run(() => AddTransitionNodeToScene(sceneBuilder, transitionGraph, renderBox)));
        }

        /// <summary>
        ///     Prepares the scene elements for the provided <see cref="PairEnergySetData" /> and adds the node one the
        ///     <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="pairEnergySet"></param>
        /// <param name="renderBox"></param>
        private void AddPairInteractionNodeToScene(DxSceneBuilder sceneBuilder, PairEnergySetData pairEnergySet, in FractionalBox3D renderBox)
        {
            var itemConfig = GetMeshItemConfigLazy(pairEnergySet);
            if (itemConfig.IsInactive) return;

            var path = pairEnergySet.AsVectorPath().ToList();
            var middle = (path[1] + path[0]) * 0.5;
            var geometry1 = CreateCylinderGeometry(itemConfig, path[0], middle);
            var geometry2 = CreateCylinderGeometry(itemConfig, middle, path[1]);
            var matrices = GetPathSceneItemTransforms(new[] {path[0], path[1]}, renderBox, false, out var anyOrientationFlips);

            CheckCellPositionHit(path[0], out IDxMeshItemConfig atomConfig1);
            CheckCellPositionHit(path[1], out IDxMeshItemConfig atomConfig2);

            var material1 = IsMatchInteractionToHit ? atomConfig1.CreateMaterial() : itemConfig.CreateMaterial();
            var material2 = !IsMatchInteractionToHit || ReferenceEquals(atomConfig1, atomConfig2) ? material1 : atomConfig2.CreateMaterial();

            var configurator1 = GetSceneNodeConfigurator(itemConfig, !anyOrientationFlips);
            var configurator2 = GetSceneNodeConfigurator(itemConfig, !anyOrientationFlips);

            AddMeshElementsToScene(sceneBuilder, geometry1, material1, matrices, configurator1);
            AddMeshElementsToScene(sceneBuilder, geometry2, material2, matrices, configurator2);
        }

        /// <summary>
        ///     Starts the scene build tasks for all pair interactions in the selected customization of the current content source
        ///     using the provided <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void StartPairInteractionSceneNodeBuilding(DxSceneBuilder sceneBuilder, FractionalBox3D renderBox)
        {
            if (ReferenceEquals(SelectedCustomization, ProjectCustomizationTemplate.Empty) || SelectedCustomization?.EnergyModelCustomization == null) return;

            foreach (var pairGraph in SelectedCustomization.EnergyModelCustomization.StablePairEnergyParameterSets)
                sceneBuilder.AttachCustomTask(Task.Run(() => AddPairInteractionNodeToScene(sceneBuilder, pairGraph, renderBox)));
            foreach (var pairGraph in SelectedCustomization.EnergyModelCustomization.UnstablePairEnergyParameterSets)
                sceneBuilder.AttachCustomTask(Task.Run(() => AddPairInteractionNodeToScene(sceneBuilder, pairGraph, renderBox)));
        }

        /// <summary>
        ///     Prepares the scene elements for the provided <see cref="GroupEnergySetData" /> and adds the node one the
        ///     <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="groupEnergySet"></param>
        /// <param name="renderBox"></param>
        private void AddGroupInteractionNodeToScene(DxSceneBuilder sceneBuilder, GroupEnergySetData groupEnergySet, in FractionalBox3D renderBox)
        {
            var itemConfig = GetMeshItemConfigLazy(groupEnergySet);
            if (itemConfig.IsInactive) return;

            var positions = groupEnergySet.AsVectorPath(true).ToList();
            var geometry = CreatePolygonFanGeometry(positions);
            var matrices = GetPathSceneItemTransforms(positions, renderBox, true, out var anyFlipsOrientation);
            var material = itemConfig.CreateMaterial();
            var configurator = GetSceneNodeConfigurator(itemConfig, !anyFlipsOrientation);

            AddMeshElementsToScene(sceneBuilder, geometry, material, matrices, configurator);
        }

        /// <summary>
        ///     Starts the scene build tasks for all group interactions in the selected customization of the current content source
        ///     using the provided <see cref="DxSceneBuilder" />
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void StartGroupInteractionSceneNodeBuilding(DxSceneBuilder sceneBuilder, FractionalBox3D renderBox)
        {
            if (ReferenceEquals(SelectedCustomization, ProjectCustomizationTemplate.Empty) || SelectedCustomization?.EnergyModelCustomization == null) return;
            foreach (var groupEnergySet in SelectedCustomization.EnergyModelCustomization.GroupEnergyParameterSets)
                sceneBuilder.AttachCustomTask(Task.Run(() => AddGroupInteractionNodeToScene(sceneBuilder, groupEnergySet, renderBox)));
        }

        /// <summary>
        ///     Creates a new atom <see cref="MeshGeometry3D" /> around (0,0,0) using the settings on the provided
        ///     <see cref="IDxMeshItemConfig" />. At very low mesh quality levels, a cube mesh will be returned instead
        /// </summary>
        /// <param name="itemConfig"></param>
        /// <returns></returns>
        private MeshGeometry3D CreateAtomGeometry(IDxMeshItemConfig itemConfig)
        {
            var meshBuilder = new MeshBuilder();
            var radius = itemConfig.MeshScaling;
            var thetaDiv = (Settings.Default.Default_Render_Sphere_ThetaDiv * itemConfig.MeshQuality).RoundToInt();
            var phiDiv = (Settings.Default.Default_Render_Sphere_PhiDiv * itemConfig.MeshQuality).RoundToInt();
            if (thetaDiv < 4 || phiDiv < 4)
                meshBuilder.AddBox(Vector3.Zero, radius, radius, radius, BoxFaces.All);
            else
                meshBuilder.AddSphere(Vector3.Zero, radius, thetaDiv, phiDiv);

            return meshBuilder.ToMesh();
        }

        /// <summary>
        ///     Creates a new transition <see cref="MeshGeometry3D" /> using the settings on the provided
        ///     <see cref="IDxMeshItemConfig" />. By default the path is size corrected for hit atom geometries
        /// </summary>
        /// <param name="itemConfig"></param>
        /// <param name="path"></param>
        /// <param name="checkAtoms"></param>
        /// <returns></returns>
        private MeshGeometry3D CreateTransitionGeometry(IDxMeshItemConfig itemConfig, IList<Fractional3D> path, bool checkAtoms = true)
        {
            var meshBuilder = new MeshBuilder();
            var diameter = itemConfig.MeshScaling;
            var headLength = Settings.Default.Default_Render_Arrow_HeadLength;
            var thetaDiv = (Settings.Default.Default_Render_Arrow_ThetaDiv * itemConfig.MeshQuality).RoundToInt();
            var sizeCorrections = checkAtoms ? CreatePathAtomSizeCorrections(path) : null;
            for (var i = 1; i < path.Count; i++)
            {
                var point1 = VectorTransformer.ToCartesianDx(path[i - 1]);
                var point2 = VectorTransformer.ToCartesianDx(path[i]);
                if (checkAtoms)
                {
                    var directionToSecond = (point2 - point1).Normalized();
                    point1 += (float) sizeCorrections[i - 1] * directionToSecond;
                    point2 -= (float) sizeCorrections[i] * directionToSecond;
                }

                meshBuilder.AddTwoHeadedArrow(point1, point2, diameter, headLength, thetaDiv);
            }

            return meshBuilder.ToMesh();
        }

        /// <summary>
        ///     Creates a size correction for each position of a path to prevent clipping with the target atom geometries
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private double[] CreatePathAtomSizeCorrections(ICollection<Fractional3D> path)
        {
            var sceneConfigs = path.Select(x => CheckCellPositionHit(x, out IDxMeshItemConfig y) ? y : null).ToArray(path.Count);
            var result = new double[sceneConfigs.Length];
            for (var i = 0; i < sceneConfigs.Length; i++) result[i] = sceneConfigs[i]?.MeshScaling ?? 0;
            return result;
        }

        /// <summary>
        ///     Creates a new cylinder <see cref="MeshGeometry3D" /> using the settings on the provided
        ///     <see cref="IDxMeshItemConfig" />
        /// </summary>
        /// <param name="itemConfig"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private MeshGeometry3D CreateCylinderGeometry(IDxMeshItemConfig itemConfig, in Fractional3D start, in Fractional3D end)
        {
            var meshBuilder = new MeshBuilder();
            var radius = itemConfig.MeshScaling;
            var thetaDiv = Settings.Default.Default_Render_Arrow_ThetaDiv;
            var (point1, point2) = (VectorTransformer.ToCartesianDx(start), VectorTransformer.ToCartesianDx(end));
            meshBuilder.AddCylinder(point1, point2, radius, thetaDiv, false, false);
            return meshBuilder.ToMesh();
        }

        /// <summary>
        ///     Creates a new polygon fan <see cref="MeshGeometry3D" /> using a set of <see cref="Fractional3D" /> positions. If in
        ///     surround mode then the center will not be considered unless required to reach at least one triangle
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="surroundMode"></param>
        /// <returns></returns>
        private MeshGeometry3D CreatePolygonFanGeometry(ICollection<Fractional3D> positions, bool surroundMode = true)
        {
            if (positions.Count < 3) throw new ArgumentException("At least three positions required.");
            var meshBuilder = new MeshBuilder();
            var dxVectors = positions.Select(x => VectorTransformer.ToCartesianDx(x)).ToList(positions.Count);
            var startIndex = surroundMode && positions.Count <= 3 ? 0 : 1;
            for (var i = startIndex; i < positions.Count - 2; i++)
            {
                for (var j = i + 1; j < positions.Count - 1; j++)
                {
                    for (var k = j + 1; k < positions.Count; k++) meshBuilder.AddTriangle(dxVectors[i], dxVectors[j], dxVectors[k]);
                }
            }

            return meshBuilder.ToMesh();
        }

        /// <summary>
        ///     Adds multiple mesh transforms to a <see cref="DxSceneBuilder" /> and performs batching based on the
        ///     <see cref="DxSceneBatchingMode" /> preference of the scene host
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="matrices"></param>
        /// <param name="callback"></param>
        protected virtual void AddMeshElementsToScene(DxSceneBuilder sceneBuilder, MeshGeometry3D geometry, MaterialCore material, IList<Matrix> matrices,
            Action<SceneNode> callback = null)
        {
            if (matrices.Count == 1)
            {
                sceneBuilder.AddMeshTransforms(geometry, material, matrices, callback);
                return;
            }

            var batchSize = GetMeshBatchSize(SceneHost.SceneBatchingMode);
            switch (batchSize)
            {
                case 1:
                    sceneBuilder.AddMeshTransforms(geometry, material, matrices, callback);
                    return;
                case int.MaxValue:
                    sceneBuilder.AddBatchedMeshTransforms(geometry, material, matrices, callback);
                    return;
            }

            var tmpMatrices = new List<Matrix>(batchSize);
            for (var i = 0; i < matrices.Count; i += batchSize)
            {
                for (var j = i; j < matrices.Count && tmpMatrices.Count < batchSize; j++) tmpMatrices.Add(matrices[j]);
                sceneBuilder.AddBatchedMeshTransforms(geometry, material, tmpMatrices, callback);
                tmpMatrices.Clear();
            }
        }

        /// <summary>
        ///     Translates the <see cref="DxSceneBatchingMode" /> setting of the scene host to an actual model count
        /// </summary>
        /// <param name="batchingMode"></param>
        /// <returns></returns>
        protected virtual int GetMeshBatchSize(DxSceneBatchingMode batchingMode)
        {
            return batchingMode switch
            {
                DxSceneBatchingMode.None => 1,
                DxSceneBatchingMode.Low => 64,
                DxSceneBatchingMode.Moderate => 256,
                DxSceneBatchingMode.High => 1024,
                DxSceneBatchingMode.Extreme => 2048,
                DxSceneBatchingMode.Unlimited => int.MaxValue,
                _ => throw new ArgumentOutOfRangeException(nameof(batchingMode), batchingMode, null)
            };
        }

        /// <summary>
        ///     Get a configuration and attachment callback <see cref="Action{T}" /> for a <see cref="SceneNode" /> using the
        ///     provided <see cref="IDxSceneItemConfig" />
        /// </summary>
        /// <param name="itemConfig"></param>
        protected virtual Action<SceneNode> GetSceneNodeConfigurator(IDxSceneItemConfig itemConfig)
        {
            var isVisible = itemConfig.IsVisible;

            void ConfigNodeAndAttach(SceneNode sceneNode)
            {
                sceneNode.IsHitTestVisible = false;
                sceneNode.Visible = isVisible;
                itemConfig.AttachNode(sceneNode, true);
            }

            return ConfigNodeAndAttach;
        }

        /// <summary>
        ///     Get a configuration and attachment callback <see cref="Action{T}" /> for a <see cref="SceneNode" /> using the
        ///     provided <see cref="IDxMeshItemConfig" />
        /// </summary>
        /// <param name="itemConfig"></param>
        /// <param name="noOrientationFlips"></param>
        protected virtual Action<SceneNode> GetSceneNodeConfigurator(IDxMeshItemConfig itemConfig, bool noOrientationFlips = false)
        {
            var baseConfigurator = GetSceneNodeConfigurator((IDxSceneItemConfig) itemConfig);
            var cullMode = GetOptimalCullMode(itemConfig.VisualCategory, noOrientationFlips);
            var isWireFrameVisible = itemConfig.IsWireframeVisible;
            var wireframeColor = itemConfig.WireframeColor.ToColor4();

            void ConfigNodeAndAttach(SceneNode sceneNode)
            {
                switch (sceneNode)
                {
                    case BatchedMeshNode batchedMeshNode:
                        batchedMeshNode.CullMode = cullMode;
                        batchedMeshNode.WireframeColor = wireframeColor;
                        batchedMeshNode.RenderWireframe = isWireFrameVisible;
                        break;
                    case MeshNode meshNode:
                        meshNode.CullMode = cullMode;
                        meshNode.WireframeColor = wireframeColor;
                        meshNode.RenderWireframe = isWireFrameVisible;
                        break;
                }

                baseConfigurator.Invoke(sceneNode);
            }

            return ConfigNodeAndAttach;
        }

        /// <summary>
        ///     Get the optimal <see cref="CullMode" /> for a <see cref="VisualObjectCategory" /> when using the basic
        ///     <see cref="MeshBuilder" />
        /// </summary>
        /// <param name="objectCategory"></param>
        /// <param name="noOrientationFlips"></param>
        /// <returns></returns>
        /// <remarks>
        ///     More optimal values can be provided if it is ensured that triangles orientation does not change due to the
        ///     model transform
        /// </remarks>
        protected virtual CullMode GetOptimalCullMode(VisualObjectCategory objectCategory, bool noOrientationFlips = false)
        {
            if (!noOrientationFlips) return CullMode.None;
            return objectCategory switch
            {
                VisualObjectCategory.Sphere => CullMode.Back,
                VisualObjectCategory.Cube => CullMode.Back,
                VisualObjectCategory.DoubleArrow => CullMode.Back,
                VisualObjectCategory.SingleArrow => CullMode.Back,
                VisualObjectCategory.Cylinder => CullMode.Back,
                _ => CullMode.None
            };
        }

        /// <summary>
        ///     Tests if a <see cref="Fractional3D" /> hits an atom and provides the affiliated <see cref="IDxMeshItemConfig" />
        ///     if the test is positive
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        protected bool CheckCellPositionHit(in Fractional3D vector, out IDxMeshItemConfig config)
        {
            var result = CheckCellPositionHit(vector, out CellReferencePositionData positionGraph);
            config = result ? GetMeshItemConfigLazy(positionGraph) : null;
            return result;
        }

        /// <summary>
        ///     Tests if a <see cref="Fractional3D" /> hits an atom and provides the affiliated
        ///     <see cref="CellReferencePositionData" /> if true
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="referencePositionData"></param>
        /// <returns></returns>
        protected bool CheckCellPositionHit(in Fractional3D vector, out CellReferencePositionData referencePositionData)
        {
            var trimmedVector = vector.TrimToUnitCell(VectorTransformer.FractionalSystem.Comparer);
            var index = CellReferencePositionCatalog.IndexOf(new KeyValuePair<Fractional3D, CellReferencePositionData>(trimmedVector, null));
            var isHit = index >= 0 && index < CellReferencePositionCatalog.Count;
            referencePositionData = isHit ? CellReferencePositionCatalog[index].Value : null;
            return isHit;
        }

        /// <summary>
        ///     Creates a <see cref="IComparer{T}" /> to sort and search the position catalog
        /// </summary>
        /// <returns></returns>
        private IComparer<KeyValuePair<Fractional3D, CellReferencePositionData>> CreatePositionCatalogComparer()
        {
            var vectorComparer = new VectorComparer3D<Fractional3D>(ModelProject.GeometryNumeric.RangeComparer);
            return Comparer<KeyValuePair<Fractional3D, CellReferencePositionData>>.Create((a, b) => vectorComparer.Compare(a.Key, b.Key));
        }
    }
}