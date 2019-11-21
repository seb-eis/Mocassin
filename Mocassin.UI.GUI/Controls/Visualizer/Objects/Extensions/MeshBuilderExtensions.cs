using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     Provides extension methods for the <see cref="MeshBuilder" /> of the helix toolkit
    /// </summary>
    public static class MeshBuilderExtensions
    {
        /// <summary>
        ///     Adds a two headed arrow shape mesh to the <see cref="MeshBuilder" />
        /// </summary>
        /// <param name="meshBuilder"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="diameter"></param>
        /// <param name="headLength"></param>
        /// <param name="thetaDiv"></param>
        public static void AddTwoHeadedArrow(this MeshBuilder meshBuilder, in Point3D startPoint, in Point3D endPoint, double diameter, double headLength,
            int thetaDiv)
        {
            var direction = endPoint - startPoint;
            var length = direction.Length;
            var radius = diameter / 2.0;
            var revolvePoints = new PointCollection
            {
                new Point(0.0, 0.0),
                new Point(diameter * headLength, diameter),
                new Point(diameter * headLength, radius),
                new Point(length - diameter * headLength, radius),
                new Point(length - diameter * headLength, diameter),
                new Point(length, 0.0)
            };
            meshBuilder.AddRevolvedGeometry(revolvePoints, null, startPoint, direction, thetaDiv);
        }
    }
}