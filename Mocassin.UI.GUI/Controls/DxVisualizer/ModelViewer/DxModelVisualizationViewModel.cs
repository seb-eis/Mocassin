using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
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
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Base;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Enums;
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
    public class DxModelVisualizationViewModel : ProjectGraphControlViewModel, IDxSceneController
    {
        private ProjectCustomizationGraph selectedCustomization;
        private bool canInvalidateScene = true;

        /// <summary>
        ///     Get or set the <see cref="IDisposable" /> that cancels the content synchronization for selectable
        ///     <see cref="ProjectCustomizationGraph" /> instances
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
        ///     Get the <see cref="IDxSceneHost" /> that hosts the created scene elements
        /// </summary>
        public IDxSceneHost SceneHost { get; }

        /// <summary>
        ///     Get the <see cref="Visualizer.DataControl.ModelRenderResourcesViewModel" /> that controls the model render
        ///     resources
        /// </summary>
        public ModelRenderResourcesViewModel ModelRenderResourcesViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of <see cref="IObjectSceneConfig" /> for model objects
        /// </summary>
        public ObservableCollectionViewModel<IObjectSceneConfig> ModelObjectSceneConfigs { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of <see cref="IObjectSceneConfig" /> for customization objects
        /// </summary>
        public ObservableCollectionViewModel<IObjectSceneConfig> CustomizationObjectSceneConfigs { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of selectable <see cref="ProjectCustomizationGraph" />
        ///     instances
        /// </summary>
        public ObservableCollectionViewModel<ProjectCustomizationGraph> CustomizationCollectionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to invalidate and rebuild the entire scene
        /// </summary>
        public ICommand InvalidateSceneCommand { get; }

        /// <summary>
        ///     Get or set the selected <see cref="ProjectCustomizationGraph" />
        /// </summary>
        public ProjectCustomizationGraph SelectedCustomization
        {
            get => selectedCustomization;
            set => SetProperty(ref selectedCustomization, value, OnSelectedCustomizationChanged);
        }

        /// <inheritdoc />
        public bool CanInvalidateScene
        {
            get => canInvalidateScene && ContentSource != null;
            protected set => SetProperty(ref canInvalidateScene, value);
        }

        /// <inheritdoc />
        public DxModelVisualizationViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ModelProject = projectControl.CreateModelProject();
            SceneHost = new DxViewportViewModel();
            ModelRenderResourcesViewModel = new ModelRenderResourcesViewModel();
            ModelObjectSceneConfigs = new ObservableCollectionViewModel<IObjectSceneConfig>();
            CustomizationObjectSceneConfigs = new ObservableCollectionViewModel<IObjectSceneConfig>();
            CustomizationCollectionViewModel = new ObservableCollectionViewModel<ProjectCustomizationGraph>();
            InvalidateSceneCommand = new AsyncRelayCommand(InvalidateSceneAsync, () => CanInvalidateScene);
            SceneHost.AttachController(this);
        }

        /// <inheritdoc />
        public override async void ChangeContentSource(MocassinProjectGraph contentSource)
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
            InvalidateScene();
        }

        /// <summary>
        ///     Invalidates and clears all visualization content sources and resets the viewport view model
        /// </summary>
        private void ClearContent()
        {
            ContentSource = null;
            SelectedCustomization = null;
            ModelRenderResourcesViewModel.SetDataSource(null);
            ModelObjectSceneConfigs.Clear();
            CustomizationObjectSceneConfigs.Clear();
            CustomizationCollectionViewModel.Clear();
            SceneHost.ResetScene(false);
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
            ModelObjectSceneConfigs.Clear();
            if (ContentSource == null) return;
            var source = ContentSource.ProjectModelGraph;
            ModelObjectSceneConfigs.AddItem(GetModelObjectSceneConfig(source.StructureModelGraph.StructureInfo));
            ModelObjectSceneConfigs.AddItems(source.StructureModelGraph.UnitCellPositions.Select(GetModelObjectSceneConfig));
            ModelObjectSceneConfigs.AddItems(source.TransitionModelGraph.KineticTransitions.Select(GetModelObjectSceneConfig));
        }

        /// <summary>
        ///     Rebuilds and synchronizes all render resource content access objects affiliated with the customization source
        /// </summary>
        private void RebuildCustomizationRenderResourceAccess()
        {
            CustomizationObjectSceneConfigs.Clear();
            if (ContentSource == null || SelectedCustomization == null || ReferenceEquals(SelectedCustomization, ProjectCustomizationGraph.Empty)) return;
            var source = SelectedCustomization.EnergyModelCustomization;
            CustomizationObjectSceneConfigs.AddItems(source.StablePairEnergyParameterSets.Select(GetCustomizationObjectSceneConfig));
            CustomizationObjectSceneConfigs.AddItems(source.UnstablePairEnergyParameterSets.Select(GetCustomizationObjectSceneConfig));
            CustomizationObjectSceneConfigs.AddItems(source.GroupEnergyParameterSets.Select(GetCustomizationObjectSceneConfig));
        }

        /// <summary>
        ///     Get the <see cref="IObjectSceneConfig" /> for the provided <see cref="ExtensibleProjectObjectGraph" /> or creates the instance if required
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private IObjectSceneConfig GetModelObjectSceneConfig(ExtensibleProjectObjectGraph objectGraph)
        {
            return GetObjectRenderViewModelAny(objectGraph, ModelObjectSceneConfigs.ObservableItems);
        }

        /// <summary>
        ///     Get the <see cref="IObjectSceneConfig" /> for the provided <see cref="ExtensibleProjectObjectGraph" /> or creates the instance if required
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private IObjectSceneConfig GetCustomizationObjectSceneConfig(ExtensibleProjectObjectGraph objectGraph)
        {
            return GetObjectRenderViewModelAny(objectGraph, CustomizationObjectSceneConfigs.ObservableItems);
        }

        /// <summary>
        ///     Get the <see cref="IObjectSceneConfig" /> for the provided <see cref="ExtensibleProjectObjectGraph" /> from the provided source <see cref="ICollection{T}" /> or creates and adds a new one if required
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private IObjectSceneConfig GetObjectRenderViewModelAny(ExtensibleProjectObjectGraph objectGraph, ICollection<IObjectSceneConfig> source)
        {
            var result = source.FirstOrDefault(x => x.IsApplicable(objectGraph));
            if (result != null) return result;
            result = new ObjectVisualResourcesViewModel(objectGraph, GetVisualObjectCategory(objectGraph));
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
                StructureInfoGraph _ => VisualObjectCategory.Frame,
                KineticTransitionGraph _ => VisualObjectCategory.DoubleArrow,
                UnitCellPositionGraph _ => VisualObjectCategory.Sphere,
                PairEnergySetGraph _ => VisualObjectCategory.Line,
                GroupEnergySetGraph _ => VisualObjectCategory.PolygonSet,
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
        ///     Asynchronously clears the scene data, rebuilds the scene and passes the provided data to the scene host
        /// </summary>
        /// <returns></returns>
        private async Task InvalidateSceneAsync()
        {
            if (!CanInvalidateScene) return;
            CanInvalidateScene = false;
            try
            {
                var watch = Stopwatch.StartNew();
                SceneHost.ResetScene(false);
                var invalidationTime = watch.Elapsed.TotalMilliseconds;
                watch.Restart();
                var sceneModel = await BuildSceneAsync();
                var sceneBuildTime = watch.Elapsed.TotalMilliseconds;
                watch.Restart();
                SceneHost.AddSceneItem(sceneModel);
                var viewportPassTime = watch.Elapsed.TotalMilliseconds;
                SendCallInfoMessage($"Viewer CPU time info: {invalidationTime:0.0} ms (scene-clean); {sceneBuildTime:0.0} ms (scene-build); {viewportPassTime:0.0} ms (scene-setup);");
            }
            catch (Exception e)
            {
                OnRenderError(e);
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
            return await sceneBuilder.ToModelAsync();
        }

        /// <summary>
        ///     Starts the parallel build process of the scene components and provides the <see cref="DxSceneBuilder"/> to await completion
        /// </summary>
        /// <returns></returns>
        private DxSceneBuilder StartSceneBuilder()
        {
            var renderBox = ModelRenderResourcesViewModel.GetRenderBox3D();
            var sceneBuilder = new DxSceneBuilder(100);
            StartCellFrameSceneNodeBuilding(sceneBuilder, renderBox);
            StartCellPositionSceneNodeBuilding(sceneBuilder, renderBox);
            return sceneBuilder;
        }

        /// <summary>
        ///     Builds the scene component that holds the cell frame to the provided <see cref="DxSceneBuilder"/>
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void BuildCellFrameSceneNode(DxSceneBuilder sceneBuilder, in FractionalBox3D renderBox)
        {
            var sceneConfig = GetModelObjectSceneConfig(ContentSource.ProjectModelGraph.StructureModelGraph.StructureInfo);
            var material = new LineMaterialCore {LineColor = sceneConfig.Color.ToColor4(), Thickness = (float) sceneConfig.Scaling};
            var matrix = VectorTransformer.FractionalSystem.ToCartesianMatrix.ToDxMatrix();

            var lineBuilder = new LineBuilder();
            var (sizeA, sizeB, sizeC) = (renderBox.Size.A.FloorToInt(), renderBox.Size.B.FloorToInt(), renderBox.Size.C.FloorToInt());
            for (var a = 0; a <= sizeA; a++)
            {
                for (var b = 0; b <= sizeB; b++)
                {
                    for (var c = 0; c <= sizeC; c++)
                    {
                        var origin = new Vector3(a,b,c);
                        if (a < sizeA) lineBuilder.AddLine(origin, new Vector3(a + 1, b, c));
                        if (b < sizeB) lineBuilder.AddLine(origin, new Vector3(a, b + 1, c));
                        if (c < sizeC) lineBuilder.AddLine(origin, new Vector3(a, b, c + 1));
                    }
                }
            }
            sceneBuilder.AddLineGeometry(lineBuilder.ToLineGeometry3D(), material, matrix);
        }

        /// <summary>
        ///     Starts the scene build task for the cell wire-frame in model of the current content source using the provided <see cref="DxSceneBuilder"/>
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void StartCellFrameSceneNodeBuilding(DxSceneBuilder sceneBuilder, FractionalBox3D renderBox)
        {
            sceneBuilder.AttachAsBuildTask(Task.Run(() => BuildCellFrameSceneNode(sceneBuilder, renderBox)));
        }


        /// <summary>
        ///     Starts the scene build tasks for all unit cell position in model of the current content source using the provided <see cref="DxSceneBuilder"/>
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void StartCellPositionSceneNodeBuilding(DxSceneBuilder sceneBuilder, FractionalBox3D renderBox)
        {
            foreach (var item in ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions)
            {
                sceneBuilder.AttachAsBuildTask(Task.Run(() => AddCellPositionNodeToScene(sceneBuilder, item, renderBox)));
            }
        }

        /// <summary>
        ///     Prepares the scene elements for the provided <see cref="UnitCellPositionGraph" /> and adds the node one the <see cref="DxSceneBuilder"/>
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="positionGraph"></param>
        /// <param name="renderBox"></param>
        /// <returns></returns>
        private void AddCellPositionNodeToScene(DxSceneBuilder sceneBuilder, UnitCellPositionGraph positionGraph, in FractionalBox3D renderBox)
        {
            var transformMatrices = GetCellPositionSceneItemTransforms(new Fractional3D(positionGraph.A, positionGraph.B, positionGraph.C), renderBox);
            var sceneConfig = GetModelObjectSceneConfig(positionGraph);
            var geometry = CreateAtomGeometry(sceneConfig);
            var material = CreateFrozenMaterial(sceneConfig);
            AddToSceneBuilder(sceneBuilder, geometry, material, transformMatrices, x => x.IsHitTestVisible = false);
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
        ///     Creates a new frozen <see cref="MaterialCore" /> based on the settings in the provided <see cref="IObjectSceneConfig" />
        /// </summary>
        /// <param name="objectConfig"></param>
        /// <returns></returns>
        private MaterialCore CreateFrozenMaterial(IObjectSceneConfig objectConfig)
        {
            var color = objectConfig.Color.ToColor4();
            var result = ExecuteOnAppThread(() =>
            {
                var material = PhongMaterials.GetMaterial(objectConfig.MaterialName).CloneMaterial();
                material.DiffuseColor = color;
                material.Freeze();
                return material.Core;
            });
            return result;
        }

        /// <summary>
        ///     Creates a new atom <see cref="Geometry3D" /> around (0,0,0) using the settings on the provided <see cref="IObjectSceneConfig" />. If a sphere creation is not possible, a cube will be returned
        /// </summary>
        /// <param name="objectConfig"></param>
        /// <returns></returns>
        private MeshGeometry3D CreateAtomGeometry(IObjectSceneConfig objectConfig)
        {
            var meshBuilder = new MeshBuilder();
            var radius = objectConfig.Scaling;
            var thetaDiv = (Settings.Default.Default_Render_Sphere_ThetaDiv * objectConfig.Quality).RoundToInt();
            var phiDiv = (Settings.Default.Default_Render_Sphere_PhiDiv * objectConfig.Quality).RoundToInt();
            if (thetaDiv < 4 || phiDiv < 4)
                meshBuilder.AddBox(Vector3.Zero, radius, radius, radius, BoxFaces.All);
            else
                meshBuilder.AddSphere(Vector3.Zero, radius, thetaDiv, phiDiv);
            return meshBuilder.ToMesh();
        }

        /// <summary>
        ///     Adds prepared mesh information to a <see cref="DxSceneBuilder"/> utilizing the memory preferences of the attached secen host
        /// </summary>
        /// <param name="sceneBuilder"></param>
        /// <param name="geometry"></param>
        /// <param name="matrices"></param>
        /// <param name="callback"></param>
        protected virtual void AddToSceneBuilder(DxSceneBuilder sceneBuilder, MeshGeometry3D geometry, MaterialCore material, IList<Matrix> matrices,
            Action<SceneNode> callback = null)
        {
            var batchSize = GetBatchingSize(SceneHost.SceneMemoryCostPreference);
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
        ///     Translates the <see cref="DxSceneMemoryCost"/> preference to a mesh batching size preference
        /// </summary>
        /// <param name="memoryCost"></param>
        /// <returns></returns>
        protected virtual int GetBatchingSize(DxSceneMemoryCost memoryCost)
        {
            return memoryCost switch
            {
                DxSceneMemoryCost.Lowest => 1,
                DxSceneMemoryCost.Low => 16,
                DxSceneMemoryCost.Medium => 128,
                DxSceneMemoryCost.High => 256,
                DxSceneMemoryCost.Highest => 1024,
                DxSceneMemoryCost.Unlimited => int.MaxValue,
                _ => throw new ArgumentOutOfRangeException(nameof(memoryCost), memoryCost, null)
            };
        }
        
        /// <inheritdoc />
        public override void Dispose()
        {
            SceneHost.DetachController();
            SceneHost.Dispose();
            base.Dispose();
        }
    }
}