using System;
using System.Collections.Generic;
using ICon.Mathematics.ValueTypes;
using ACoordinates = ICon.Mathematics.ValueTypes.Coordinates<double, double, double>;

namespace ICon.Mathematics.Extensions
{
    /// <summary>
    ///     ICon vector math extensions that are shared between all kinds of 3D vectors
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        ///     Multiplies an array with the provided vector to create a new vector of same type (Will throw if array is not at
        ///     least of size 3x3)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static T1 MultiplyWith<T1>(this double[,] matrix, in T1 vector)
            where T1 : struct, IVector3D<T1>
        {
            var x = matrix[0, 0] * vector.Coordinates.A + matrix[0, 1] * vector.Coordinates.B + matrix[0, 2] * vector.Coordinates.C;
            var y = matrix[1, 0] * vector.Coordinates.A + matrix[1, 1] * vector.Coordinates.B + matrix[1, 2] * vector.Coordinates.C;
            var z = matrix[2, 0] * vector.Coordinates.A + matrix[2, 1] * vector.Coordinates.B + matrix[2, 2] * vector.Coordinates.C;
            return vector.CreateNew(x, y, z);
        }

        /// <summary>
        ///     Multiplies an array with the provided basic cartesian vector (Non generic performance version)
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Cartesian3D MultiplyWith(this double[,] matrix, in Cartesian3D vector)
        {
            var x = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z;
            var y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z;
            var z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z;
            return new Cartesian3D(x, y, z);
        }

        /// <summary>
        ///     Multiplies an array with the provided basic fractional vector (Non generic performance version)
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Fractional3D MultiplyWith(this double[,] matrix, in Fractional3D vector)
        {
            var x = matrix[0, 0] * vector.A + matrix[0, 1] * vector.B + matrix[0, 2] * vector.C;
            var y = matrix[1, 0] * vector.A + matrix[1, 1] * vector.B + matrix[1, 2] * vector.C;
            var z = matrix[2, 0] * vector.A + matrix[2, 1] * vector.B + matrix[2, 2] * vector.C;
            return new Fractional3D(x, y, z);
        }

        /// <summary>
        ///     Multiplies an array with the provided basic spherical vector (Non generic performance version)
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Spherical3D MultiplyWith(this double[,] matrix, in Spherical3D vector)
        {
            var x = matrix[0, 0] * vector.Radius + matrix[0, 1] * vector.Theta + matrix[0, 2] * vector.Phi;
            var y = matrix[1, 0] * vector.Radius + matrix[1, 1] * vector.Theta + matrix[1, 2] * vector.Phi;
            var z = matrix[2, 0] * vector.Radius + matrix[2, 1] * vector.Theta + matrix[2, 2] * vector.Phi;
            return new Spherical3D(x, y, z);
        }

        /// <summary>
        ///     Multiplies an array with the provided basic 3D coordinate tuple (Non generic performance version)
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Coordinates<double, double, double> MultiplyWith(this double[,] matrix, in Coordinates<double, double, double> vector)
        {
            var x = matrix[0, 0] * vector.A + matrix[0, 1] * vector.B + matrix[0, 2] * vector.C;
            var y = matrix[1, 0] * vector.A + matrix[1, 1] * vector.B + matrix[1, 2] * vector.C;
            var z = matrix[2, 0] * vector.A + matrix[2, 1] * vector.B + matrix[2, 2] * vector.C;
            return new Coordinates<double, double, double>(x, y, z);
        }

        /// <summary>
        ///     Calculates the euclidean length of a 3D coordinate tuple (Only useful if the tuple represents cartesian values)
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static double GetLength(this Coordinates<double, double, double> coordinates)
        {
            return Math.Sqrt(coordinates.A * coordinates.A + coordinates.B * coordinates.B + coordinates.C * coordinates.C);
        }

        /// <summary>
        ///     Calculates the cross product with another 3D coordinate tuple
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Coordinates<double, double, double> GetCrossProduct(this Coordinates<double, double, double> first,
            in Coordinates<double, double, double> second)
        {
            var a = first.B * second.C - first.C * second.B;
            var b = first.C * second.A - first.A * second.C;
            var c = first.A * second.B - first.B * second.A;
            return new Coordinates<double, double, double>(a, b, c);
        }

        /// <summary>
        ///     Checks if a 3D coordinate tuple is linear independent from two others
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool IsLinearIndependentFrom(this Coordinates<double, double, double> a, in Coordinates<double, double, double> b,
            in Coordinates<double, double, double> c, IEqualityComparer<double> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var determinant = a.A * b.B * c.C + a.C * b.A * c.B + a.B * b.C * c.A - a.C * b.B * c.A -
                              a.B * b.A * c.C - a.A * b.C * c.B;

            return !comparer.Equals(determinant, 0.0);
        }

        /// <summary>
        ///     Checks if a 3D coordinate tuple is independent from another
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool IsLinearIndependentFrom(this Coordinates<double, double, double> a, in Coordinates<double, double, double> b,
            IEqualityComparer<double> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            return !comparer.Equals(a.GetCrossProduct(b).GetLength(), 0.0);
        }

        /// <summary>
        ///     Adds two 3D double precision coordinates together and returns the new coordinates
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static ACoordinates Add(this ACoordinates first, ACoordinates second)
        {
            return new ACoordinates(first.A + second.A, first.B + second.B, first.C + second.C);
        }

        /// <summary>
        ///     Adds the 3D coordinates of any vector to the first 3D coordinates (Not restricted to to logically compatible vector
        ///     types)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T1 AddAny<T1, T2>(this T1 first, in T2 second)
            where T1 : struct, IVector3D<T1>
            where T2 : struct, IVector3D
        {
            return first.CreateNew(first.Coordinates.Add(second.Coordinates));
        }

        /// <summary>
        ///     Adds any 3D coordinates onto the a 3D vector (Not restricted to to logically compatible coordinate types)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="first"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static T1 AddAny<T1>(this T1 first, in ACoordinates coordinates)
            where T1 : struct, IVector3D<T1>
        {
            return first.CreateNew(first.Coordinates.Add(coordinates));
        }
    }
}