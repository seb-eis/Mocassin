using Mocassin.Mathematics.ValueTypes;
using SharpDX;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Extensions
{
    /// <summary>
    ///     Provides extensions methods to handle interaction between SharpDX single precision vectors and Mocassin.Mathematics double precision vectors
    /// </summary>
    public static class DxVectorExtensions
    {
        /// <summary>
        ///     Narrowing conversion of a <see cref="Coordinates3D"/> 192-bit source vector to a SharpDX 96-bit <see cref="Vector3"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Vector3 ToDxVector(this Cartesian3D source)
        {
            return new Vector3((float) source.X, (float) source.Y, (float) source.Z);
        }

        /// <summary>
        ///     Narrowing conversion of a <see cref="Fractional3D"/> 192-bit source vector to a SharpDX 96-bit <see cref="Vector3"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Vector3 ToDxVector(this Fractional3D source)
        {
            return new Vector3((float) source.A, (float) source.B, (float) source.C);
        }
    }
}