namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     Enum for defining a specific visual category types for 3D scene item
    /// </summary>
    public enum VisualObjectCategory
    {
        /// <summary>
        ///     Category for unknown objects
        /// </summary>
        Unknown,
        
        /// <summary>
        ///     Category for set of 2D lines
        /// </summary>
        LineGrid,

        /// <summary>
        ///     Category for spherical meshes
        /// </summary>
        Sphere,

        /// <summary>
        ///     Category for cubic or rectangular meshes
        /// </summary>
        Cube,

        /// <summary>
        ///     Category for double-sided arrows
        /// </summary>
        DoubleArrow,

        /// <summary>
        ///     Category for a single-sided arrow
        /// </summary>
        SingleArrow,

        /// <summary>
        ///     Category for a single 2D line
        /// </summary>
        Line,

        /// <summary>
        ///     Category for cylinder meshes
        /// </summary>
        Cylinder,

        /// <summary>
        ///     Category for arbitrary polygon sets
        /// </summary>
        PolygonSet
    }
}