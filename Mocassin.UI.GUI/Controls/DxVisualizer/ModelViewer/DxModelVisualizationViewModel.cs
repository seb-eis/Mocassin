using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
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

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel"/> that controls object generation and supply for model components to a <see cref="DxViewportViewModel"/>
    /// </summary>
    public class DxModelVisualizationViewModel : ProjectGraphControlViewModel
    {
        private ProjectCustomizationGraph selectedCustomization;

        /// <summary>
        ///     Get or set the <see cref="IDisposable"/> that cancels the content synchronization for selectable <see cref="ProjectCustomizationGraph"/> intances
        /// </summary>
        private IDisposable CustomizationLinkDisposable { get; set; }

        /// <summary>
        ///     Get the internal <see cref="IModelProject"/> that handles model data processing
        /// </summary>
        private IModelProject ModelProject { get; }

        /// <summary>
        ///     Get the <see cref="IVectorTransformer"/> that manages the transformations of the coordinate context
        /// </summary>
        private IVectorTransformer Transformer => ModelProject.CrystalSystemService.VectorTransformer;

        /// <summary>
        ///     Get the <see cref="ISpaceGroupService"/> that provided the space group operations and functionality
        /// </summary>
        private ISpaceGroupService SpaceGroupService => ModelProject.SpaceGroupService;

        /// <summary>
        ///     Get the <see cref="DxViewportViewModel"/> that controls the 3D scene
        /// </summary>
        public DxViewportViewModel ViewportViewModel { get; }

        /// <summary>
        ///     Get the <see cref="Visualizer.DataControl.ModelRenderResourcesViewModel"/> that controls the model render resources
        /// </summary>
        public ModelRenderResourcesViewModel ModelRenderResourcesViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}"/> of <see cref="ObjectRenderResourcesViewModel"/> that manages the model object render settings
        /// </summary>
        public ObservableCollectionViewModel<ObjectRenderResourcesViewModel> ModelObjectResourcesViewModels { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}"/> of <see cref="ObjectRenderResourcesViewModel"/> that manages the customization object render settings
        /// </summary>
        public ObservableCollectionViewModel<ObjectRenderResourcesViewModel> CustomizationObjectResourcesViewModels { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}"/> of selectable <see cref="ProjectCustomizationGraph"/> instances
        /// </summary>
        public ObservableCollectionViewModel<ProjectCustomizationGraph> CustomizationCollectionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="AsyncCommand"/> to invalidate and rebuild the entire scene
        /// </summary>
        public AsyncCommand InvalidateSceneCommand { get; }

        /// <summary>
        ///     Get or set the selected <see cref="ProjectCustomizationGraph"/>
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
        ///     Internal implementation of content source change that updates the view model if a new <see cref="MocassinProjectGraph"/> is set
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
            var (count, batch) = (5000, true);
            var time = ViewportViewModel.LoadTestScene(count, batch);
            SendCallInfoMessage($"Test scene loaded: {count} elements in {time}.");
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
        ///     Rebuilds and synchronizes the data access structure after a <see cref="MocassinProjectGraph"/> content source change
        /// </summary>
        private void RebuildContentAccess()
        {
            RebuildModelAccess();
            RebuildCustomizationAccess();
            RebuildRenderResourceAccess();
        }

        /// <summary>
        ///     Rebuilds and synchronizes the <see cref="ProjectModelGraph"/> content access after the primary content source has changed
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
        ///     Rebuilds and synchronizes the <see cref="ProjectCustomizationGraph"/> content access after the primary content source has changed
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
        ///    Rebuilds and synchronizes all render resource content access objects affiliated with the model source
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
        ///    Rebuilds and synchronizes all render resource content access objects affiliated with the customization source
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
        ///     Get the <see cref="ObjectRenderResourcesViewModel"/> for the provided <see cref="ExtensibleProjectObjectGraph"/> or creates the instance if required
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private ObjectRenderResourcesViewModel GetModelRenderResourcesViewModel(ExtensibleProjectObjectGraph objectGraph)
        {
            return GetObjectRenderViewModelAny(objectGraph, ModelObjectResourcesViewModels.ObservableItems);
        }

        /// <summary>
        ///     Get the <see cref="ObjectRenderResourcesViewModel"/> for the provided <see cref="ExtensibleProjectObjectGraph"/> or creates the instance if required
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
        private ObjectRenderResourcesViewModel GetObjectRenderViewModelAny(ExtensibleProjectObjectGraph objectGraph, ICollection<ObjectRenderResourcesViewModel> source)
        {
            var result = source.FirstOrDefault(x => ReferenceEquals(x.ObjectGraph, objectGraph));
            if (result != null) return result;
            result = new ObjectRenderResourcesViewModel(objectGraph, GetVisualObjectCategory(objectGraph));
            source.Add(result);
            return result;
        }

        /// <summary>
        ///     Gets the <see cref="VisualObjectCategory"/> for the provided <see cref="ExtensibleProjectObjectGraph"/>
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private VisualObjectCategory GetVisualObjectCategory(ExtensibleProjectObjectGraph objectGraph)
        {
            return objectGraph switch
            {
                StructureInfoGraph _ => VisualObjectCategory.Frame,
                KineticTransitionGraph _ => VisualObjectCategory.Transition,
                UnitCellPositionGraph _ => VisualObjectCategory.Position,
                PairEnergySetGraph _ => VisualObjectCategory.Interaction,
                GroupEnergySetGraph _ => VisualObjectCategory.Cluster,
                _ => VisualObjectCategory.Unknown
            };
        }

        /// <summary>
        ///     Handles the required actions if the target <see cref="ProjectCustomizationGraph"/> was changed
        /// </summary>
        private void OnSelectedCustomizationChanged()
        {
            RebuildCustomizationAccess();
        }

        /// <summary>
        ///     Action that handles a render <see cref="Exception"/>
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
            ViewportViewModel.Reset();
            var sceneModels = await BuildSceneComponentsAsync();
            foreach (var model in sceneModels)
            {
                ViewportViewModel.AddElement3D(model);
            }
        }

        /// <summary>
        ///     Asynchronously builds the 3D scene <see cref="SceneNodeGroupModel3D"/> instances
        /// </summary>
        /// <returns></returns>
        private Task<IList<SceneNodeGroupModel3D>> BuildSceneComponentsAsync()
        {
            var sceneBuilder = new DxSceneBuilder();
            var renderBox = ModelRenderResourcesViewModel.GetRenderBox3D();
            return null;
        }


    }
}