using HelixToolkit.Wpf.SharpDX.Model;
using SharpDX;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Objects
{
    /// <summary>
    ///     Represents a view model for manipulation and configuration of display/generation of DX scene line items
    /// </summary>
    public interface IDxLineItemConfig : IDxSceneItemConfig
    {
        /// <summary>
        ///     Get or set the <see cref="LineMaterialCore"/> for the geometry
        /// </summary>
        LineMaterialCore Material { get; set; }

        /// <summary>
        ///     Get or set the line color as a <see cref="Color4"/>
        /// </summary>
        Color4 DxColor { get; set; }

        /// <summary>
        ///     Get or set the line color as a <see cref="System.Windows.Media.Color"/>
        /// </summary>
        System.Windows.Media.Color Color { get; set; }

        /// <summary>
        ///     Get or set the thickness of the line geometry
        /// </summary>
        double LineThickness { get; set; }
    }
}