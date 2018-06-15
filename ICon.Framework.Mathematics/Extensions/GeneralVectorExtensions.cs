using System;
using System.Collections.Generic;

using ICon.Mathematics.ValueTypes;
using ACoorTuple = ICon.Mathematics.ValueTypes.Coordinates<System.Double, System.Double, System.Double>;

namespace ICon.Mathematics.Extensions
{
    /// <summary>
    /// ICon vector math extensions that are shared between all kinds of 3D vectors
    /// </summary>
    public static class GeneralVectorExtensions
    {
        /// <summary>
        /// Multiplies an array with the provided vector to create a new vector of same type (Will throw if array is not at least of size 3x3)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static T1 MultiplyWith<T1>(this Double[,] matrix, T1 vector) where T1 : struct, IVector3D<T1>
        {
            Double x = matrix[0, 0] * vector.Coordinates.A + matrix[0, 1] * vector.Coordinates.B + matrix[0, 2] * vector.Coordinates.C;
            Double y = matrix[1, 0] * vector.Coordinates.A + matrix[1, 1] * vector.Coordinates.B + matrix[1, 2] * vector.Coordinates.C;
            Double z = matrix[2, 0] * vector.Coordinates.A + matrix[2, 1] * vector.Coordinates.B + matrix[2, 2] * vector.Coordinates.C;
            return vector.CreateNew(x, y, z);
        }

        /// <summary>
        /// Multiplies an array with the provided basic cartesian vector (Non generic performance version)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Cartesian3D MultiplyWith(this Double[,] matrix, Cartesian3D vector)
        {
            Double x = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z;
            Double y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z;
            Double z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z;
            return new Cartesian3D(x, y, z);
        }

        /// <summary>
        /// Multiplies an array with the provided basic fractional vector (Non generic performance version)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Fractional3D MultiplyWith(this Double[,] matrix, Fractional3D vector)
        {
            Double x = matrix[0, 0] * vector.A + matrix[0, 1] * vector.B + matrix[0, 2] * vector.C;
            Double y = matrix[1, 0] * vector.A + matrix[1, 1] * vector.B + matrix[1, 2] * vector.C;
            Double z = matrix[2, 0] * vector.A + matrix[2, 1] * vector.B + matrix[2, 2] * vector.C;
            return new Fractional3D(x, y, z);
        }

        /// <summary>
        /// Multiplies an array with the provided basic spherical vector (Non generic performance version)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Spherical3D MultiplyWith(this Double[,] matrix, Spherical3D vector)
        {
            Double x = matrix[0, 0] * vector.Radius + matrix[0, 1] * vector.Theta + matrix[0, 2] * vector.Phi;
            Double y = matrix[1, 0] * vector.Radius + matrix[1, 1] * vector.Theta + matrix[1, 2] * vector.Phi;
            Double z = matrix[2, 0] * vector.Radius + matrix[2, 1] * vector.Theta + matrix[2, 2] * vector.Phi;
            return new Spherical3D(x, y, z);
        }

        /// <summary>
        /// Multiplies an array with the provided basic 3D coordinate tuple (Non generic performance version)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Coordinates<Double, Double, Double> MultiplyWith(this Double[,] matrix, Coordinates<Double, Double, Double> vector)
        {
            Double x = matrix[0, 0] * vector.A + matrix[0, 1] * vector.B + matrix[0, 2] * vector.C;
            Double y = matrix[1, 0] * vector.A + matrix[1, 1] * vector.B + matrix[1, 2] * vector.C;
            Double z = matrix[2, 0] * vector.A + matrix[2, 1] * vector.B + matrix[2, 2] * vector.C;
            return new Coordinates<Double, Double, Double>(x, y, z);
        }

        /// <summary>
        /// Calculates the euklidic length of a 3D coordinate tuple (Only useful if the tuple represents cartesian values)
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static Double GetLength(this Coordinates<Double, Double, Double> coordinates)
        {
            return Math.Sqrt(coordinates.A * coordinates.A + coordinates.B * coordinates.B + coordinates.C * coordinates.C);
        }

        /// <summary>
        /// Calculates the cross product with another 3D coordinate tuple
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Coordinates<Double, Double, Double> GetCrossProduct(this Coordinates<Double, Double, Double> first, Coordinates<Double, Double, Double> second)
        {
            Double a = first.B * second.C - first.C * second.B;
            Double b = first.C * second.A - first.A * second.C;
            Double c = first.A * second.B - first.B * second.A;
            return new Coordinates<Double, Double, Double>(a, b, c);
        }

        /// <summary>
        /// Checks if a 3D coordinate tuple is linear independent from two others
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <returns></returns>
        public static Boolean IsLinearIndependentFrom(this Coordinates<Double, Double, Double> vecA, Coordinates<Double, Double, Double> vecB, Coordinates<Double, Double, Double> vecC, IEqualityComparer<Double> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            Double determinant = vecA.A * vecB.B * vecC.C + vecA.C * vecB.A * vecC.B + vecA.B * vecB.C * vecC.A - vecA.C * vecB.B * vecC.A - vecA.B * vecB.A * vecC.C - vecA.A * vecB.C * vecC.B;
            return !comparer.Equals(determinant, 0.0);
        }

        /// <summary>
        /// Checks if a 3D coordinate tuple is independet from another
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Boolean IsLinearIndependentFrom(this Coordinates<Double, Double, Double> vecA, Coordinates<Double, Double, Double> vecB, IEqualityComparer<Double> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            return !comparer.Equals(vecA.GetCrossProduct(vecB).GetLength(), 0.0);
        }

        /// <summary>
        /// Adds two 3D double precision coordinates together and retusn the new coordinates
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static ACoorTuple Add(this ACoorTuple first, ACoorTuple second)
        {
            return new ACoorTuple(first.A + second.A, first.B + second.B, first.C + second.C);
        }

        /// <summary>
        /// Adds the 3D coordinates of any vector to the first 3D coordinates (Not restricted to to logically compatible vector types)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T1 AddAny<T1, T2>(this T1 first, T2 second) where T1 : struct, IVector3D<T1> where T2 : struct, IVector3D
        {
            return first.CreateNew(first.Coordinates.Add(second.Coordinates));
        }

        /// <summary>
        /// Adds any 3D coordinates onto the a 3D vector (Not restricted to to logically compatible coordinate types)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T1 AddAny<T1>(this T1 first, ACoorTuple coordinates) where T1 : struct, IVector3D<T1>
        {
            return first.CreateNew(first.Coordinates.Add(coordinates));
        }
    }
}
