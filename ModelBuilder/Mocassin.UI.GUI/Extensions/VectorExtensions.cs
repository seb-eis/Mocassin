using System.Windows.Media.Media3D;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.UI.GUI.Extensions
{
    /// <summary>
    ///     Provides extension methods for transforming Mocassin math value types into geometry data for display
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        ///     Get a <see cref="Point3D" /> with the same dat as the passed <see cref="Cartesian3D" />
        /// </summary>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public static Point3D AsPoint3D(this Cartesian3D cartesian) => new Point3D(cartesian.X, cartesian.Y, cartesian.Z);

        /// <summary>
        ///     Get a <see cref="Vector3D" /> with the same dat as the passed <see cref="Cartesian3D" />
        /// </summary>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public static Vector3D AsVector3D(this Cartesian3D cartesian) => new Vector3D(cartesian.X, cartesian.Y, cartesian.Z);
    }
}