using System;
using System.Collections.Generic;

using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Extensions
{
    /// <summary>
    /// Extension class to provide generic mathematical operations for objects that implement the cartesian vector interface without the virtual call overhead
    /// </summary>
    public static class CartesianVectorExtensions
    {
        /// <summary>
        /// Calculate the euklidean norm length (also called 2-norm)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Double GetLength<T1>(this T1 vector) where T1 : struct, ICartesian3D<T1>
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        /// <summary>
        /// Checks if cartesian vector is linear independet from another
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean IsLinearIndepentFrom(this Cartesian3D first, Cartesian3D second, IEqualityComparer<Double> comparer)
        {
            return first.Coordinates.IsLinearIndependentFrom(second.Coordinates, comparer);
        }

        /// <summary>
        /// Generic cehck if two cartesian vectors are linear independent
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean IsLinearIndependentFrom<T1, T2>(this T1 first, T2 second, IEqualityComparer<Double> comparer) where T1 : struct, ICartesian3D where T2 : struct, ICartesian3D
        {
            return first.Coordinates.IsLinearIndependentFrom(second.Coordinates, comparer);
        }

        /// <summary>
        /// Fiex all almsot zeros to zero utilizing the provided double comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static T1 GetZeroCleaned<T1>(this T1 vector, IComparer<Double> comparer) where T1 : struct, ICartesian3D<T1>
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            Double x = comparer.Compare(vector.X, 0.0) == 0 ? 0.0 : vector.X;
            Double y = comparer.Compare(vector.Y, 0.0) == 0 ? 0.0 : vector.Y;
            Double z = comparer.Compare(vector.Z, 0.0) == 0 ? 0.0 : vector.Z;
            return vector.CreateNew(x, y, z);
        }

        /// <summary>
        /// Returns the normalized version of the vector
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static T1 GetNormalized<T1>(this T1 vector) where T1 : struct, ICartesian3D<T1>
        {
            return vector.MultiplyWith(1.0 / vector.GetLength());
        }

        /// <summary>
        /// Get the angle between this and the second vector in radian
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Double GetAngleTo<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            Double dotProduct = first.GetNormalized().GetScalarProduct(second.GetNormalized());
            dotProduct = dotProduct > 1.0 ? 1.0 : dotProduct < -1.0 ? -1.0 : dotProduct;
            return Math.Acos(dotProduct);
        }

        /// <summary>
        /// Performs a generic scalar multiplication ofor the vector
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T1 MultiplyWith<T1>(this T1 vector, Double value) where T1 : struct, ICartesian3D<T1>
        {
            return vector.CreateNew(vector.X * value, vector.Y * value, vector.Z * value);
        }

        /// <summary>
        /// Performs a generic scalar division for the vector
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T1 DivideBy<T1>(this T1 vector, Double value) where T1 : struct, ICartesian3D<T1>
        {
            return vector.MultiplyWith(1.0 / value);
        }

        /// <summary>
        /// Adds the second vector onto the current one, result is of source type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T1 Add<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            return first.CreateNew(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
        }

        /// <summary>
        /// Subtract the vector second vector from the current one, result is of source type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T1 Substract<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            return first.CreateNew(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
        }

        /// <summary>
        /// Calculates the scalar product (dot product) of the two vectors
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Double GetScalarProduct<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            return first.X * second.X + first.Y * second.Y + first.Z * second.Z;
        }

        /// <summary>
        /// Gets the cross product of the first vector with the second, result is of same type as first
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T1 GetCrossProduct<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            return first.CreateNew(first.Coordinates.GetCrossProduct(second.Coordinates));
        }

        /// <summary>
        /// Get the spat product between three cartesian vectors
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <returns></returns>
        public static double GetSpatProduct<T1, T2, T3>(this T1 first, T2 second, T3 third) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2> where T3 : struct, ICartesian3D<T3>
        {
            return first.GetScalarProduct(second.GetCrossProduct(third));
        }

        /// <summary>
        /// Calculates the projection length of this vector onto the other
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Double GetProjectionLength<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            return first.GetScalarProduct(second.GetNormalized());
        }

        /// <summary>
        /// Projects the vector onto another, the result vector is of the source type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T1 GetSourceProjection<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            Double factor = first.GetScalarProduct(second) / second.GetLength();
            return first.CreateNew(second.X, second.Y, second.Z).MultiplyWith(factor);
        }

        /// <summary>
        /// Projects the vector onto another, the result vector is of the target type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T2 GetTargetProjection<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            Double factor = first.GetScalarProduct(second) / second.GetLength();
            return second.MultiplyWith(factor);
        }

        /// <summary>
        /// Rejection of the vector with another, the result vector is of the source type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T1 GetSourceRejection<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            return first.Substract(first.GetTargetProjection(second));
        }

        /// <summary>
        /// Recetion of the vector with another, the result vector is of the target type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T2 GetTargetRejection<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            T1 rejection = first.GetSourceRejection(second);
            return second.CreateNew(rejection.X, rejection.Y, rejection.Z);
        }

        /// <summary>
        /// Returns the shortest possible vector of source type from an axis defined by the second vector (New vector is perpedicular to second)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T1 GetNormalVector<T1, T2>(this T1 first, T2 second) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            return first.Substract(second.GetNormalized().MultiplyWith(first.GetProjectionLength(second)));
        }

        /// <summary>
        /// Cehcks if two vectors are linear dependent within the tolerance of the provided double comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean IsLinearIndependentFrom<T1, T2>(this T1 first, T2 second, IEqualityComparer<T1> comparer) where T1 : struct, ICartesian3D<T1> where T2 : struct, ICartesian3D<T2>
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            return !comparer.Equals(first.GetCrossProduct(second), first.CreateNew(0.0, 0.0, 0.0));
        }
    }
}
