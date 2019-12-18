using HelixToolkit.Wpf.SharpDX.Model;
using SharpDX;
using Color = System.Windows.Media.Color;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Objects
{
    /// <summary>
    ///     Represents a view model for manipulation and configuration of display/generation of DX scene mesh items
    /// </summary>
    public interface IDxMeshItemConfig : IDxSceneItemConfig
    {
        /// <summary>
        ///     Get or set the <see cref="MaterialCore" /> of the mesh
        /// </summary>
        MaterialCore Material { get; set; }

        /// <summary>
        ///     Get or set the diffuse color as a <see cref="Color4" />
        /// </summary>
        Color4 DxDiffuseColor { get; set; }

        /// <summary>
        ///     Get or set the diffuse color as a <see cref="System.Windows.Media.Color" />
        /// </summary>
        Color DiffuseColor { get; set; }

        /// <summary>
        ///     Get or set a factor for the mesh quality (1.0 is the default quality)
        /// </summary>
        double MeshQuality { get; set; }

        /// <summary>
        ///     Get or set a factor for the uniform mesh scaling (1.0 is the default quality)
        /// </summary>
        double UniformScaling { get; set; }

        /// <summary>
        ///     Get a boolean flag if the scene node is a batched geometry
        /// </summary>
        bool IsBatched { get; }
    }
}