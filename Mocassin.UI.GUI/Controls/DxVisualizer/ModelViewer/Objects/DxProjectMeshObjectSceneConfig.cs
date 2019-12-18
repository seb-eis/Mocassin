using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Objects;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;
using SharpDX;
using SharpDX.Direct2D1;
using Color = System.Windows.Media.Color;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.Objects
{
    /// <summary>
    ///     Extends the <see cref="DxProjectObjectSceneConfig"/> with <see cref="IDxMeshItemConfig"/> interface
    /// </summary>
    public class DxProjectMeshObjectSceneConfig : DxProjectObjectSceneConfig, IDxMeshItemConfig
    {
        private static string MaterialKey => Resources.ResourceKey_ModelObject_RenderMaterial;
        private static string ColorKey => Resources.ResourceKey_ModelObject_RenderColor;
        private static string MeshQualityKey => Resources.ResourceKey_ModelObject_MeshQuality;
        private static string UniformScalingKey => Resources.ResourceKey_ModelObject_RenderScaling;

        /// <inheritdoc />
        public override VisualObjectCategory VisualCategory { get; }

        /// <inheritdoc />
        public MaterialCore Material { get; set; }

        /// <inheritdoc />
        public Color4 DxDiffuseColor
        {
            get => DiffuseColor.ToColor4();
            set
            {
                if (value.Equals(DxDiffuseColor)) return;
                DiffuseColor = value.ToColor();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public Color DiffuseColor { get; set; }

        /// <inheritdoc />
        public double MeshQuality { get; set; }

        /// <inheritdoc />
        public double UniformScaling { get; set; }

        /// <inheritdoc />
        public bool IsBatched => SceneNode is BatchedMeshNode;

        /// <inheritdoc />
        public DxProjectMeshObjectSceneConfig(ExtensibleProjectObjectGraph objectGraph, VisualObjectCategory visualCategory)
            : base(objectGraph)
        {
            VisualCategory = visualCategory;
        }

        /// <inheritdoc />
        protected override bool CheckSupport(SceneNode node)
        {
            return node is MeshNode || node is BatchedMeshNode;
        }
    }
}