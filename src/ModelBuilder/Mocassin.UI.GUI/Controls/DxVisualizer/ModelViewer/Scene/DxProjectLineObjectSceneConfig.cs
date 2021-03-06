﻿using System;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.Mathematics.Extensions;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Scene;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.Scene
{
    /// <summary>
    ///     Implementation of the <see cref="DxProjectObjectSceneConfig" /> extended by the <see cref="IDxLineItemConfig" />
    ///     interface
    /// </summary>
    public class DxProjectLineObjectSceneConfig : DxProjectObjectSceneConfig, IDxLineItemConfig
    {
        private readonly LineMaterialCore material = new LineMaterialCore();
        private static string ColorKey => Properties.Resources.ResourceKey_ModelObject_RenderColor;
        private static string LineThicknessKey => Properties.Resources.ResourceKey_ModelObject_RenderScaling;

        /// <inheritdoc />
        public LineMaterialCore Material
        {
            get => material;
            set => throw new NotSupportedException("Setting the material to a custom value is currently not supported.");
        }

        /// <inheritdoc />
        public Color Color
        {
            get => DataObject.Resources.TryGetResource(ColorKey, x => VisualExtensions.ParseRgbaHexToColor(x), out var color) ? color : Colors.Gray;
            set
            {
                if (value.Equals(Color)) return;
                DataObject.Resources.SetResource(ColorKey, value, color => color.ToRgbaHex());
                OnColorChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public double LineThickness
        {
            get => DataObject.Resources.TryGetResource(LineThicknessKey, out double value) ? value : 1;
            set
            {
                if (value.AlmostEqualByRange(LineThickness)) return;
                DataObject.Resources.SetResource(LineThicknessKey, value);
                OnLineThicknessChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public DxProjectLineObjectSceneConfig(ExtensibleProjectDataObject dataObject, VisualObjectCategory visualCategory)
            : base(dataObject, visualCategory)
        {
        }

        /// <inheritdoc />
        public LineMaterialCore CreateMaterial() => new LineMaterialCore {LineColor = Color.ToColor4(), Thickness = (float) LineThickness};

        /// <inheritdoc />
        public sealed override bool CheckSupport(SceneNode node) => node is LineNode;

        /// <summary>
        ///     Action that is called when the <see cref="LineThickness" /> property changed
        /// </summary>
        protected virtual void OnLineThicknessChanged()
        {
            OnMaterialChanged();
        }

        /// <summary>
        ///     Action that is called when the <see cref="Color" /> property changed
        /// </summary>
        protected virtual void OnColorChanged()
        {
            OnMaterialChanged();
        }

        /// <summary>
        ///     Action that is called when the <see cref="Material" /> property changed
        /// </summary>
        protected virtual void OnMaterialChanged()
        {
            var lineMaterial = CreateMaterial();
            foreach (var sceneNode in SceneNodes) ChangeNodeMaterial((LineNode) sceneNode, lineMaterial);
        }

        /// <summary>
        ///     Changes the material of the provided <see cref="LineNode" /> to the provided <see cref="LineMaterialCore" />
        /// </summary>
        /// <param name="lineNode"></param>
        /// <param name="lineMaterial"></param>
        protected void ChangeNodeMaterial(LineNode lineNode, LineMaterialCore lineMaterial)
        {
            if (lineNode == null || material == null) return;
            lineNode.Material = lineMaterial;
        }

        /// <inheritdoc />
        protected override void CopyCurrentValuesToNode(SceneNode sceneNode)
        {
            ChangeNodeMaterial((LineNode) sceneNode, Material);
            base.CopyCurrentValuesToNode(sceneNode);
        }
    }
}