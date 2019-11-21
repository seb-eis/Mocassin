using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Helper
{
    /// <summary>
    ///     Static class that provides helper methods for dealing with <see cref="Light3D"/>
    /// </summary>
    public static class Light3DHelper
    {
        /// <summary>
        ///     Builds the default  <see cref="GroupModel3D"/> of <see cref="Light3D"/> instances using the provided base <see cref="Color"/>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static GroupModel3D CreateDefaultLightModel(in Color color)
        {
            var lightModel = new GroupModel3D() {IsHitTestVisible = false};
            lightModel.Children.Add(CreateDirectionalLight(color.ChangeIntensity(180.0 / byte.MaxValue), -1, -1, -1));
            lightModel.Children.Add(CreateDirectionalLight(color.ChangeIntensity(120.0 / byte.MaxValue), -1, -1, -.1));
            lightModel.Children.Add(CreateDirectionalLight(color.ChangeIntensity(60.0 / byte.MaxValue), .1, 1, -1));
            lightModel.Children.Add(CreateDirectionalLight(color.ChangeIntensity(500.0 / byte.MaxValue), .1, .1, 1));
            lightModel.Children.Add(new AmbientLight3D {Color = color.ChangeIntensity(30.0 / byte.MaxValue)});
            return lightModel;
        }

        /// <summary>
        ///     Builds the default  <see cref="GroupModel3D"/> of <see cref="Light3D"/> instances using <see cref="Colors.White"/> as base
        /// </summary>
        public static GroupModel3D CreateDefaultLightModel()
        {
            return CreateDefaultLightModel(Colors.White);
        }

        /// <summary>
        ///     Creates a new <see cref="DirectionalLight3D"/> from a <see cref="Color"/> and x,y,z direction information
        /// </summary>
        /// <param name="color"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static DirectionalLight3D CreateDirectionalLight(in Color color, double x, double y, double z)
        {
            return CreateDirectionalLight(color, new Vector3D(x, y, z));
        }

        /// <summary>
        ///     Creates a new <see cref="DirectionalLight3D"/> from a <see cref="Color"/> and a direction <see cref="Vector3D"/>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static DirectionalLight3D CreateDirectionalLight(in Color color, in Vector3D direction)
        {
            return new DirectionalLight3D {Color = color, Direction = direction};
        }
    }
}