using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.Mathematics.Extensions;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Objects;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;
using SharpDX;
using Color = System.Windows.Media.Color;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.Objects
{
    /// <summary>
    ///     Implementation of the <see cref="DxProjectObjectSceneConfig"/> extended by the <see cref="IDxLineItemConfig"/> interface
    /// </summary>
    public class DxProjectLineObjectSceneConfig : DxProjectObjectSceneConfig, IDxLineItemConfig
    {
        private LineMaterialCore material = new LineMaterialCore();
        private static string ColorKey => Resources.ResourceKey_ModelObject_RenderColor;
        private static string LineThicknessKey => Resources.ResourceKey_ModelObject_RenderScaling;

        /// <inheritdoc />
        public LineMaterialCore Material
        {
            get => material;
            set => SetProperty(ref material, value, OnMaterialChanged);
        }

        /// <inheritdoc />
        public Color4 DxColor
        {
            get => Color.ToColor4();
            set
            {
                if (value.Equals(DxColor)) return;
                OnDxColorChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public Color Color
        {
            get => ObjectGraph.Resources.TryGetResource(ColorKey, x => VisualExtensions.ParseRgbaHexToColor(x), out var color) ? color : Colors.Gray;
            set
            {
                if (value.Equals(Color)) return;
                ObjectGraph.Resources.SetResource(ColorKey, value, color => color.ToRgbaHex());
                OnColorChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public double LineThickness
        {
            get => ObjectGraph.Resources.TryGetResource(LineThicknessKey, out double value) ? value : 1;
            set
            {
                if (value.AlmostEqualByRange(LineThickness)) return;
                ObjectGraph.Resources.SetResource(LineThicknessKey, value);
                OnLineThicknessChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public DxProjectLineObjectSceneConfig(ExtensibleProjectObjectGraph objectGraph, VisualObjectCategory visualCategory)
            : base(objectGraph, visualCategory)
        {
        }

        /// <summary>
        ///     Action that is called when the <see cref="LineThickness"/> property changed
        /// </summary>
        protected virtual void OnLineThicknessChanged()
        {
            if (Material == null) return;
            Material.Thickness = (float) LineThickness;
        }

        /// <summary>
        ///     Action that is called when the <see cref="DxColor"/> property changed
        /// </summary>
        protected virtual void OnDxColorChanged()
        {
            Color = DxColor.ToColor();
        }

        /// <summary>
        ///     Action that is called when the <see cref="Color"/> property changed
        /// </summary>
        protected virtual void OnColorChanged()
        {
            if (Material == null) return;
            Material.LineColor = DxColor;
        }

        /// <summary>
        ///     Action that is called when the <see cref="Material"/> property changed
        /// </summary>
        protected virtual void OnMaterialChanged()
        {
            if (SceneNode == null || Material == null) return;
            Material.LineColor = DxColor;
            Material.Thickness = (float) LineThickness;
            ((LineNode) SceneNode).Material = Material;
        }

        /// <inheritdoc />
        public sealed override bool CheckSupport(SceneNode node)
        {
            return node is LineNode;
        }

        /// <inheritdoc />
        protected override void CopyValuesToSceneNode()
        {
            OnMaterialChanged();
            base.CopyValuesToSceneNode();
        }
    }
}