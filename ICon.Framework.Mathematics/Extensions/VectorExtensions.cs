using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Extensions
{
    /// <summary>
    ///     ICon vector math extensions that are shared between all kinds of 3D vectors
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        ///     Multiplies an array with the provided basic 3D coordinate tuple (matrix * vector)
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Coordinates3D MultiplyWith(this double[,] matrix, in Coordinates3D vector)
        {
            var x = matrix[0, 0] * vector.A + matrix[0, 1] * vector.B + matrix[0, 2] * vector.C;
            var y = matrix[1, 0] * vector.A + matrix[1, 1] * vector.B + matrix[1, 2] * vector.C;
            var z = matrix[2, 0] * vector.A + matrix[2, 1] * vector.B + matrix[2, 2] * vector.C;
            return new Coordinates3D(x, y, z);
        }

        /// <summary>
        ///     Multiplies an array with the provided basic cartesian vector (matrix * vector)
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Cartesian3D MultiplyWith(this double[,] matrix, in Cartesian3D vector)
        {
            return new Cartesian3D(matrix.MultiplyWith(vector.Coordinates));
        }

        /// <summary>
        ///     Multiplies an array with the provided basic fractional vector (matrix * vector)
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Fractional3D MultiplyWith(this double[,] matrix, in Fractional3D vector)
        {
            return new Fractional3D(matrix.MultiplyWith(vector.Coordinates));
        }

        /// <summary>
        ///     Multiplies an array with the provided basic spherical vector (matrix * vector)
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Spherical3D MultiplyWith(this double[,] matrix, in Spherical3D vector)
        {
            return new Spherical3D(matrix.MultiplyWith(vector.Coordinates));
        }

        /// <summary>
        ///     Calculates the euclidean length of a 3D coordinate tuple (Only useful if the tuple represents cartesian values)
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static double GetLength(this Coordinates3D coordinates)
        {
            return Math.Sqrt(coordinates.A * coordinates.A + coordinates.B * coordinates.B + coordinates.C * coordinates.C);
        }

        /// <summary>
        ///     Calculates the cross product with another 3D coordinate tuple
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Coordinates3D GetCrossProduct(this Coordinates3D first, in Coordinates3D second)
        {
            var a = first.B * second.C - first.C * second.B;
            var b = first.C * second.A - first.A * second.C;
            var c = first.A * second.B - first.B * second.A;
            return new Coordinates3D(a, b, c);
        }

        /// <summary>
        ///     Checks if a 3D coordinate tuple is linear independent from two others
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool IsLinearIndependentFrom(this Coordinates3D a, in Coordinates3D b, in Coordinates3D c, IEqualityComparer<double> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            var determinant = a.A * b.B * c.C +
                              a.C * b.A * c.B +
                              a.B * b.C * c.A -
                              a.C * b.B * c.A -
                              a.B * b.A * c.C -
                              a.A * b.C * c.B;

            return !comparer.Equals(determinant, 0.0);
        }

        /// <summary>
        ///     Checks if a 3D coordinate tuple is independent from another
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool IsLinearIndependentFrom(this Coordinates3D a, in Coordinates3D b, IEqualityComparer<double> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            return !comparer.Equals(a.GetCrossProduct(b).GetLength(), 0.0);
        }

        /// <summary>
        ///     Generic test wherever a <see cref="IFractional3D"/> lies within abounding cuboid defined by two <see cref="IFractional3D"/> points
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="comparer"></param>
        /// <returns>-1 if any component is below the lower point, 0 if all are within bounds an +1 if any component is above the upper point</returns>
        public static int CuboidHitTest<T>(this T value, T start, T end, IComparer<double> comparer) where T : IFractional3D
        {
            if (comparer.Compare(value.A, start.A) < 0 || comparer.Compare(value.B, start.B) < 0 || comparer.Compare(value.C, start.C) < 0) return -1;
            if (comparer.Compare(value.A, end.A) > 0 || comparer.Compare(value.B, end.B) > 0 || comparer.Compare(value.C, end.C) > 0) return 1;
            return 0;
        }
    }
}