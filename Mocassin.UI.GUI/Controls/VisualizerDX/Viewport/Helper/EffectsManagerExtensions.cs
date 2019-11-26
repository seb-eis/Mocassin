using System;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Helper
{
    /// <summary>
    ///      Provides extensions methods for the <see cref="EffectsManager"/>
    /// </summary>
    public static class EffectsManagerExtensions
    {
        /// <summary>
        ///     Determine ta maximal supported texture dimension based on the hardware feature level
        /// </summary>
        /// <param name="effectsManager"></param>
        /// <returns></returns>
        public static int GetMaxHardwareTextureDimension(this EffectsManager effectsManager)
        {
            return effectsManager.Device.FeatureLevel switch
            {
                FeatureLevel.Level_9_1 => 2048,
                FeatureLevel.Level_9_2 => 2048,
                FeatureLevel.Level_9_3 => 8192,
                FeatureLevel.Level_10_0 => 8192,
                FeatureLevel.Level_10_1 => 8192,
                FeatureLevel.Level_11_0 => 16384,
                FeatureLevel.Level_11_1 => 16384,
                FeatureLevel.Level_12_0 => 16384,
                FeatureLevel.Level_12_1 => 16384,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}