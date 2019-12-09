﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Model.ModelProject;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Visualizer.DataControl;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;
using Mocassin.UI.Xml.StructureModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel"/> that controls object generation and supply for model components to a <see cref="DX3DViewportViewModel"/>
    /// </summary>
    public class DX3DModelVisualizationViewModel : ProjectGraphControlViewModel
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
        ///     Get the <see cref="DX3DViewportViewModel"/> tat controls the 3D scene
        /// </summary>
        public DX3DViewportViewModel ViewportViewModel { get; }

        /// <summary>
        ///     Get the <see cref="Visualizer.DataControl.ModelRenderResourcesViewModel"/> that controls the model render resources
        /// </summary>
        public ModelRenderResourcesViewModel ModelRenderResourcesViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}"/> of <see cref="ObjectRenderResourcesViewModel"/> that manages the model object render settings
        /// </summary>
        public ObservableCollectionViewModel<ObjectRenderResourcesViewModel> ModelRenderResourcesViewModels { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}"/> of <see cref="ObjectRenderResourcesViewModel"/> that manages the customization object render settings
        /// </summary>
        public ObservableCollectionViewModel<ObjectRenderResourcesViewModel> CustomizationRenderResourcesViewModels { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}"/> of selectable <see cref="ProjectCustomizationGraph"/> instances
        /// </summary>
        public ObservableCollectionViewModel<ProjectCustomizationGraph> CustomizationCollectionViewModel { get; }

        /// <summary>
        ///     Get or set the selected <see cref="ProjectCustomizationGraph"/>
        /// </summary>
        public ProjectCustomizationGraph SelectedCustomization
        {
            get => selectedCustomization;
            set => SetProperty(ref selectedCustomization, value);
        }

        /// <inheritdoc />
        public DX3DModelVisualizationViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ModelProject = projectControl.CreateModelProject();
            ViewportViewModel = new DX3DViewportViewModel();
            ModelRenderResourcesViewModel = new ModelRenderResourcesViewModel();
            ModelRenderResourcesViewModels = new ObservableCollectionViewModel<ObjectRenderResourcesViewModel>();
            CustomizationRenderResourcesViewModels = new ObservableCollectionViewModel<ObjectRenderResourcesViewModel>();
            CustomizationCollectionViewModel = new ObservableCollectionViewModel<ProjectCustomizationGraph>();
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
        }

        /// <summary>
        ///     Invalidates and clears all visualization content sources and resets the viewport view model
        /// </summary>
        private void ClearContent()
        {
            ContentSource = null;
            SelectedCustomization = null;
            ModelRenderResourcesViewModels.ClearCollection();
            CustomizationRenderResourcesViewModels.ClearCollection();
            CustomizationCollectionViewModel.ClearCollection();
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
            CustomizationCollectionViewModel.ClearCollection();
            if (ContentSource == null) return;
            CustomizationCollectionViewModel.AddCollectionItems(ProjectCustomizationGraph.Empty.AsSingleton().Concat(ContentSource.ProjectCustomizationGraphs));
            CustomizationLinkDisposable = CustomizationCollectionViewModel.ObservableItems.ListenToContentChanges(ContentSource.ProjectCustomizationGraphs);
            if (CustomizationCollectionViewModel.ObservableItems.Contains(SelectedCustomization)) return;
            SelectedCustomization = ProjectCustomizationGraph.Empty;
        }

        /// <summary>
        ///     Rebuilds and synchronizes all render resource content access objects after the primary content source has changed
        /// </summary>
        private void RebuildRenderResourceAccess()
        {
            ModelRenderResourcesViewModel.ChangeDataSource(ContentSource?.Resources);
            ModelRenderResourcesViewModels.ClearCollection();
            CustomizationRenderResourcesViewModels.ClearCollection();
            if (ContentSource == null) return;


        }

        /// <summary>
        ///     Ensures that all required <see cref="ObjectRenderResourcesViewModel" /> instances are created
        /// </summary>
        private void EnsureRenderResourcesAccessorsLoaded()
        {
            GetModelRenderResourcesViewModel(ContentSource.ProjectModelGraph.StructureModelGraph.StructureInfo);
            foreach (var item in ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions) GetModelRenderResourcesViewModel(item);
            foreach (var item in ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions) GetModelRenderResourcesViewModel(item);
        }

        /// <summary>
        ///     Get the <see cref="ObjectRenderResourcesViewModel"/> for the provided <see cref="ExtensibleProjectObjectGraph"/> or creates the instance if required
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private ObjectRenderResourcesViewModel GetModelRenderResourcesViewModel(ExtensibleProjectObjectGraph objectGraph)
        {
            return GetObjectRenderViewModelAny(objectGraph, ModelRenderResourcesViewModels.ObservableItems);
        }

        /// <summary>
        ///     Get the <see cref="ObjectRenderResourcesViewModel"/> for the provided <see cref="ExtensibleProjectObjectGraph"/> or creates the instance if required
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private ObjectRenderResourcesViewModel GetCustomizationRenderResourcesViewModel(ExtensibleProjectObjectGraph objectGraph)
        {
            return GetObjectRenderViewModelAny(objectGraph, CustomizationRenderResourcesViewModels.ObservableItems);
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
    }
}