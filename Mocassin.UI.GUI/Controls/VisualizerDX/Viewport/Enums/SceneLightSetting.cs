﻿namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Enums
{
    /// <summary>
    ///     Enumerates the possible light types for the DX viewport view models
    /// </summary>
    public enum SceneLightSetting
    {
        /// <summary>
        ///     Defines no light, this is an internal default value
        /// </summary>
        None,

        /// <summary>
        ///     Defines the default 4 directional light with a weak ambient light
        /// </summary>
        Default,

        /// <summary>
        ///     Defines the omni-directional light
        /// </summary>
        OmniDirectional
    }
}