using System;
using System.Collections.Generic;
using System.ComponentModel;
using HelixToolkit.Wpf.SharpDX.Model;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.DataControl.Converter;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Scene;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> that provides the customization scene item control functionality
    ///     for a <see cref="DxModelSceneViewModel" />
    /// </summary>
    public class DxCustomizationControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="DxModelSceneViewModel" /> parent that controls scene build and supply
        /// </summary>
        private DxModelSceneViewModel ParentModelSceneViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of <see cref="IDxMeshItemConfig" /> for the pair
        ///     interaction visuals
        /// </summary>
        public ObservableCollectionViewModel<IDxMeshItemConfig> PairInteractionItemConfigs => ParentModelSceneViewModel.PairInteractionItemConfigs;

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of <see cref="IDxMeshItemConfig" /> for the group
        ///     interaction visuals
        /// </summary>
        public ObservableCollectionViewModel<IDxMeshItemConfig> GroupInteractionItemConfigs => ParentModelSceneViewModel.GroupInteractionItemConfigs;

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of currently selectable
        ///     <see cref="ProjectCustomizationTemplate" /> instances
        /// </summary>
        public ObservableCollectionViewModel<ProjectCustomizationTemplate> SelectableCustomizations => ParentModelSceneViewModel.SelectableCustomizations;

        /// <summary>
        ///     Get a boolean flag if the scene is invalid and requires rebuilding
        /// </summary>
        public bool IsInvalidScene => ParentModelSceneViewModel.IsInvalidScene;

        /// <summary>
        ///     Get or set a boolean flag if the scene is invalid and requires rebuilding
        /// </summary>
        public bool IsMatchInteractionToHit
        {
            get => ParentModelSceneViewModel.IsMatchInteractionToHit;
            set
            {
                if (IsMatchInteractionToHit == value) return;
                ParentModelSceneViewModel.IsMatchInteractionToHit = value;
            }
        }

        /// <summary>
        ///     Get a <see cref="IReadOnlyCollection{T}" /> of all supported <see cref="PhongMaterialCore" /> names
        /// </summary>
        public IReadOnlyCollection<string> MeshMaterialNames => PhongMaterialCoreToStringConverter.MaterialNameCollection;

        /// <summary>
        ///     Get or set the currently selected <see cref="ProjectCustomizationTemplate" />
        /// </summary>
        public ProjectCustomizationTemplate SelectedCustomization
        {
            get => ParentModelSceneViewModel.SelectedCustomization;
            set
            {
                if (ReferenceEquals(SelectedCustomization, value)) return;
                ParentModelSceneViewModel.SelectedCustomization = value;
            }
        }

        /// <inheritdoc />
        public DxCustomizationControlViewModel(IProjectAppControl projectControl, DxModelSceneViewModel parentModelSceneViewModel)
            : base(projectControl)
        {
            ParentModelSceneViewModel = parentModelSceneViewModel ?? throw new ArgumentNullException(nameof(parentModelSceneViewModel));
            ParentModelSceneViewModel.PropertyChanged += OnParentPropertyChanged;
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
        }

        /// <summary>
        ///     Action that is called to handle property changes on the parent <see cref="DxModelSceneViewModel" />
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(DxModelSceneViewModel.IsInvalidScene):
                    OnPropertyChanged(nameof(IsInvalidScene));
                    break;
                case nameof(DxModelSceneViewModel.SelectedCustomization):
                    OnPropertyChanged(nameof(SelectedCustomization));
                    break;
                case nameof(DxModelSceneViewModel.IsMatchInteractionToHit):
                    OnPropertyChanged(nameof(IsMatchInteractionToHit));
                    break;
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            ParentModelSceneViewModel.PropertyChanged -= OnParentPropertyChanged;
            base.Dispose();
        }
    }
}