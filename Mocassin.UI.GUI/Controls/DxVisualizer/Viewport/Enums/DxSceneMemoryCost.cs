using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Base;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Enums
{
    /// <summary>
    ///     Qualitative memory cost indicator for DX 3D scenes between very low and unlimited
    /// </summary>
    /// <remarks> Primarily used to indicate how much memory footprint a <see cref="IDxSceneController"/> is allowed to create to increase scene performance </remarks>
    public enum DxSceneMemoryCost
    {
        /// <summary>
        ///     The memory cost should be as low (CPU limit)
        /// </summary>
        Lowest,

        /// <summary>
        ///     The memory footprint should be low (High CPU impact)
        /// </summary>
        Low,

        /// <summary>
        ///     The memory footprint can be medium (Medium CPU impact)
        /// </summary>
        Medium,

        /// <summary>
        ///     The memory footprint can be high (Low CPU impact)
        /// </summary>
        High,

        /// <summary>
        ///    The memory footprint can be very high (Very low CPU impact) 
        /// </summary>
        Highest,

        /// <summary>
        ///     No restrictions on the memory cost (Memory & GPU limit)
        /// </summary>
        Unlimited
    }
}