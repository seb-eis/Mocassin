using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Helper
{
    /// <summary>
    ///     Static class that provides helper methods for creating <see cref="Light3D"/> and <see cref="LightNode"/>
    /// </summary>
    public static class DxLightFactory
    {
        /// <summary>
        ///     Builds the default  <see cref="GroupModel3D"/> of <see cref="Light3D"/> instances using the provided base <see cref="Color"/>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GroupModel3D DefaultLightModel3D(in Color color, string name = null)
        {
            var lightModel = new GroupModel3D() {IsHitTestVisible = false, Name = name};
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(180.0 / byte.MaxValue), -1, -1, -1));
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(120.0 / byte.MaxValue), -1, -1, -.1));
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(60.0 / byte.MaxValue), .1, 1, -1));
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(500.0 / byte.MaxValue), .1, .1, 1));
            lightModel.Children.Add(DefaultAmbientLight3D(30.0 / byte.MaxValue));
            return lightModel;
        }

        /// <summary>
        ///     Builds the default  <see cref="GroupModel3D"/> of <see cref="Light3D"/> instances using <see cref="Colors.White"/> as base
        /// </summary>
        public static GroupModel3D DefaultLightModel3D()
        {
            return DefaultLightModel3D(Colors.White);
        }

        /// <summary>
        ///     Builds a default weak <see cref="AmbientLight3D"/> using the provided <see cref="Color"/>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static AmbientLight3D DefaultAmbientLight3D(in Color color, double intensity = 1.0)
        {
            return new AmbientLight3D {Color = color.ChangeIntensity(intensity)};
        }

        /// <summary>
        ///     Builds a default <see cref="AmbientLight3D"/> using <see cref="Colors.White"/> as a base
        /// </summary>
        /// <returns></returns>
        /// <param name="intensity"></param>
        public static AmbientLight3D DefaultAmbientLight3D(double intensity = 1.0)
        {
            return DefaultAmbientLight3D(Colors.White, intensity);
        }

        /// <summary>
        ///     Builds an omni-directional light model that supplies six direction lighting
        /// </summary>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        /// <returns></returns>
        public static GroupModel3D DefaultOmniDirectionalLightModel3D(in Color color, double intensity = 1.0)
        {
            var lightModel = new GroupModel3D() {IsHitTestVisible = false};
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(intensity), 1, 0, 0));
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(intensity), -1, 0, 0));
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(intensity), 0, 1, 0));
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(intensity), 0, -1, 0));
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(intensity), 0, 0, 1));
            lightModel.Children.Add(DirectionalLight3D(color.ChangeIntensity(intensity), 0, 0, -1));
            return lightModel;
        }

        /// <summary>
        ///     Creates a new <see cref="HelixToolkit.Wpf.SharpDX.DirectionalLight3D"/> from a <see cref="Color"/> and x,y,z direction information
        /// </summary>
        /// <param name="color"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static DirectionalLight3D DirectionalLight3D(in Color color, double x, double y, double z)
        {
            return DirectionalLight3D(color, new Vector3D(x, y, z));
        }

        /// <summary>
        ///     Creates a new <see cref="HelixToolkit.Wpf.SharpDX.DirectionalLight3D"/> from a <see cref="Color"/> and a direction <see cref="Vector3D"/>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static DirectionalLight3D DirectionalLight3D(in Color color, in Vector3D direction)
        {
            return new DirectionalLight3D {Color = color, Direction = direction};
        }
    }
}