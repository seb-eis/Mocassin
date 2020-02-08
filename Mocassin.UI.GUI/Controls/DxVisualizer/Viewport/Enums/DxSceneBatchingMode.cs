namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Enums
{
    /// <summary>
    ///     Batching mode for DX 3D scenes that indicates how much batching of scene elements is allowed to increase render and
    ///     setup performance
    /// </summary>
    public enum DxSceneBatchingMode
    {
        /// <summary>
        ///     No mesh batching allowed. Very high CPU cost but drastically reduces the RAM and VRAM cost
        /// </summary>
        None,

        /// <summary>
        ///     Low amount of mesh batching allowed. Slightly reduces CPU cost by slightly increasing RAM/VRAM cost
        /// </summary>
        Low,

        /// <summary>
        ///     Moderate amount of mesh batching allowed. Moderately reduces CPU cost by moderately increasing RAM/VRAM cost
        /// </summary>
        Moderate,

        /// <summary>
        ///     High amount of mesh batching allowed. Highly reduces CPU cost by highly increasing RAM/VRAM cost
        /// </summary>
        High,

        /// <summary>
        ///     Very high amount of mesh batching allowed. Drastically reduces CPU cost by drastically increasing RAM/VRAM cost
        /// </summary>
        Extreme,

        /// <summary>
        ///     No limit on mesh batching. Keeps CPU demand minimal to maximize GPU load but may case models that no longer fit
        ///     into VRAM
        /// </summary>
        Unlimited
    }
}