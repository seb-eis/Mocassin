﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using HelixToolkit.Wpf.SharpDX.Model;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.DataControl.Converter;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Scene;
using Mocassin.UI.GUI.Controls.Visualizer.DataControl;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> that provides the basic scene data control functionality for a
    ///     <see cref="DxModelSceneViewModel" />
    /// </summary>
    public class DxModelControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="DxModelSceneViewModel" /> parent that controls scene build and supply
        /// </summary>
        private DxModelSceneViewModel ParentModelSceneViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ModelRenderResourcesViewModel" /> that controls the basic render resources and settings
        /// </summary>
        public ModelRenderResourcesViewModel RenderResourcesViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of <see cref="IDxMeshItemConfig" /> for the position
        ///     visuals
        /// </summary>
        public ObservableCollectionViewModel<IDxMeshItemConfig> PositionItemConfigs => ParentModelSceneViewModel.PositionItemConfigs;

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of <see cref="IDxMeshItemConfig" /> for the transition
        ///     visuals
        /// </summary>
        public ObservableCollectionViewModel<IDxMeshItemConfig> TransitionItemConfigs => ParentModelSceneViewModel.TransitionItemConfigs;

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of <see cref="IDxLineItemConfig" /> for the line visuals
        /// </summary>
        public ObservableCollectionViewModel<IDxLineItemConfig> LineItemConfigs => ParentModelSceneViewModel.LineItemConfigs;

        /// <summary>
        ///     Get a boolean flag if the scene is invalid and requires rebuilding
        /// </summary>
        public bool IsInvalidScene => ParentModelSceneViewModel.IsInvalidScene;

        /// <summary>
        ///     Get a <see cref="IReadOnlyCollection{T}" /> of all supported <see cref="PhongMaterialCore" /> names
        /// </summary>
        public IReadOnlyCollection<string> MeshMaterialNames => PhongMaterialCoreToStringConverter.MaterialNameCollection;

        /// <inheritdoc />
        public DxModelControlViewModel(IMocassinProjectControl projectControl, DxModelSceneViewModel parentSceneViewModel)
            : base(projectControl)
        {
            RenderResourcesViewModel = new ModelRenderResourcesViewModel();
            ParentModelSceneViewModel = parentSceneViewModel ?? throw new ArgumentNullException(nameof(parentSceneViewModel));
            ParentModelSceneViewModel.PropertyChanged += OnParentPropertyChanged;
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            RenderResourcesViewModel.SetDataSource(contentSource?.Resources);
        }

        /// <summary>
        ///     Forces a reload of all source data into the view models
        /// </summary>
        public void LoadSourceData()
        {
            RenderResourcesViewModel.LoadSourceData();
        }

        /// <summary>
        ///     Get the <see cref="FractionalBox3D" /> that describes the render limits in fractional coordinates
        /// </summary>
        /// <returns></returns>
        public FractionalBox3D GetRenderBox3D()
        {
            return RenderResourcesViewModel.GetRenderBox3D();
        }

        /// <summary>
        ///     Action that is called to handle property changes on the parent <see cref="DxModelSceneViewModel" />
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(DxModelSceneViewModel.IsInvalidScene)) OnPropertyChanged(nameof(IsInvalidScene));
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            ParentModelSceneViewModel.PropertyChanged -= OnParentPropertyChanged;
            base.Dispose();
        }
    }
}