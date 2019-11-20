using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Helper
{
    /// <summary>
    ///     Static class that provides helper methods for dealing with <see cref="Light3D"/>
    /// </summary>
    public static class LightHelper
    {
        /// <summary>
        ///     Builds the default scene <see cref="Light3DCollection"/> using the provided base <see cref="Color"/>
        /// </summary>
        /// <param name="baseColor"></param>
        /// <returns></returns>
        public static Light3DCollection GetDefaultLight(Color baseColor)
        {
            var lightGroup = new Light3DCollection();
            lightGroup.Children.Add(new DirectionalLight3D {Color = baseColor.ChangeIntensity(180.0 / byte.MaxValue), Direction = new Vector3D(-1.0, -1.0, -1.0)});
            lightGroup.Children.Add(new DirectionalLight3D {Color = baseColor.ChangeIntensity(120.0 / byte.MaxValue), Direction = new Vector3D(-1.0, -1.0, -0.1)});
            lightGroup.Children.Add(new DirectionalLight3D {Color = baseColor.ChangeIntensity(60.0 / byte.MaxValue), Direction = new Vector3D(0.1, 1.0, -1.0)});
            lightGroup.Children.Add(new DirectionalLight3D {Color = baseColor.ChangeIntensity(50.0 / byte.MaxValue), Direction = new Vector3D(0.1, 0.1, 1.0)});
            lightGroup.Children.Add(new AmbientLight3D {Color = baseColor.ChangeIntensity(30.0 / byte.MaxValue)});
            return lightGroup;
        }

        /// <summary>
        ///     Builds the default scene <see cref="Light3DCollection"/> using base <see cref="Colors.White"/> as the base color
        /// </summary>
        public static Light3DCollection GetDefaultLight()
        {
            return GetDefaultLight(Colors.White);
        }
    }
}