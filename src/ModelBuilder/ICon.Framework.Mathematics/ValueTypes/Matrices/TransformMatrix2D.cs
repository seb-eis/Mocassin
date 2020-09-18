using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Transformation matrix for three dimensional vector systems, matrix is always 3x3
    /// </summary>
    public class TransformMatrix2D : Matrix2D
    {
        /// <inheritdoc />
        public TransformMatrix2D(double[,] values, IComparer<double> comparer)
            : base(values, comparer)
        {
            if (Values.GetUpperBound(0) != 2 || Values.GetUpperBound(1) != 2)
                throw new ArgumentException("Input array is not of size 3x3", nameof(values));
        }

        /// <inheritdoc />
        public TransformMatrix2D(IComparer<double> comparer)
            : base(3, 3, comparer)
        {
        }

        /// <summary>
        ///     Generic application of the rotation onto a generic cartesian vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Cartesian3D Transform(in Cartesian3D vector) => Values.MultiplyWith(vector);

        /// <summary>
        ///     Transform operation for unspecified 3D coordinate tuple
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Coordinates3D Transform(in Coordinates3D vector) => Values.MultiplyWith(vector);

        /// <summary>
        ///     Transform operation for non generic basic fractional vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Fractional3D Transform(in Fractional3D vector) => Values.MultiplyWith(vector);


        /// <summary>
        ///     Transform operation for non generic basic spherical vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Spherical3D Transform(in Spherical3D vector) => Values.MultiplyWith(vector);

        /// <summary>
        ///     Matrix vector multiplication with unspecified 3D coordinate tuple
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Coordinates3D operator *(TransformMatrix2D matrix, in Coordinates3D vector) => matrix.Transform(vector);

        /// <summary>
        ///     Matrix vector multiplication with basic cartesian vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Cartesian3D operator *(TransformMatrix2D matrix, in Cartesian3D vector) => matrix.Transform(vector);

        /// <summary>
        ///     Matrix vector multiplication with basic fractional vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Fractional3D operator *(TransformMatrix2D matrix, in Fractional3D vector) => matrix.Transform(vector);

        /// <summary>
        ///     Matrix vector multiplication with basic spherical vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Spherical3D operator *(TransformMatrix2D matrix, in Spherical3D vector) => matrix.Transform(vector);

        /// <summary>
        ///     Multiplies with another transformation matrix
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static TransformMatrix2D operator *(TransformMatrix2D lhs, TransformMatrix2D rhs) =>
            new TransformMatrix2D(lhs.Values.RightMatrixMultiplication(rhs.Values), lhs.Comparer);
    }
}