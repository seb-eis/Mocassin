using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using SharpDX;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Extensions
{
    /// <summary>
    ///     Provides extensions methods for direct usage of SharpDX vector structs with the <see cref="IVectorTransformer" />
    ///     interface
    /// </summary>
    public static class DxVectorTransformerExtensions
    {
        /// <summary>
        ///     Transforms a <see cref="Fractional3D" /> vector into cartesian coordinates and narrows the result to a SharpDX
        ///     <see cref="Vector3" />
        /// </summary>
        /// <param name="transformer"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 ToCartesianDx(this IVectorTransformer transformer, in Fractional3D vector)
        {
            return transformer.ToCartesian(vector).ToDxVector();
        }

        /// <summary>
        ///     Transforms a <see cref="Cartesian3D" /> vector into fractional coordinates and narrows the result to a SharpDX
        ///     <see cref="Vector3" />
        /// </summary>
        /// <param name="transformer"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 ToFractionalDx(this IVectorTransformer transformer, in Cartesian3D vector)
        {
            return transformer.ToFractional(vector).ToDxVector();
        }
    }
}