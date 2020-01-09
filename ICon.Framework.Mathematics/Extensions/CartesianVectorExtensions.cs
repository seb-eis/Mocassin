using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Extensions
{
    /// <summary>
    ///     Extension class to provide generic mathematical operations for objects that implement the cartesian vector
    ///     interface without the virtual call overhead
    /// </summary>
    public static class CartesianVectorExtensions
    {
        /// <summary>
        ///     Get the two norm of a cartesian vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double GetLength(this ICartesian3D vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        /// <summary>
        ///     Checks if cartesian vector is linear independent from another
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool IsLinearIndependent(this ICartesian3D first, ICartesian3D second, IEqualityComparer<double> comparer)
        {
            return first.Coordinates.IsLinearIndependentFrom(second.Coordinates, comparer);
        }

        /// <summary>
        ///     Check if two vectors are linear dependent within the tolerance of the provided double comparer
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool IsLinearIndependent(this Cartesian3D first, in Cartesian3D second, IEqualityComparer<Cartesian3D> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            return !comparer.Equals(first.GetCrossProduct(second), Cartesian3D.Zero);
        }

        /// <summary>
        ///     Fixes all almost zeros to zero utilizing the provided double comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Cartesian3D GetZeroCleaned(this Cartesian3D vector, IComparer<double> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            var x = comparer.Compare(vector.X, 0.0) == 0 ? 0.0 : vector.X;
            var y = comparer.Compare(vector.Y, 0.0) == 0 ? 0.0 : vector.Y;
            var z = comparer.Compare(vector.Z, 0.0) == 0 ? 0.0 : vector.Z;
            return new Cartesian3D(x, y, z);
        }

        /// <summary>
        ///     Returns the normalized version of the vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Cartesian3D GetNormalized(this Cartesian3D vector)
        {
            return vector * (1.0 / vector.GetLength());
        }

        /// <summary>
        ///     Get the angle between this and the second vector in radian
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static double GetAngleTo(this Cartesian3D first, in Cartesian3D second)
        {
            var dotProduct = first.GetNormalized().GetScalarProduct(second.GetNormalized());
            dotProduct = dotProduct > 1.0 ? 1.0 : dotProduct < -1.0 ? -1.0 : dotProduct;
            return Math.Acos(dotProduct);
        }

        /// <summary>
        ///     Calculates the scalar product (dot product) of the two vectors
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static double GetScalarProduct(this Cartesian3D first, in Cartesian3D second)
        {
            return first.X * second.X + first.Y * second.Y + first.Z * second.Z;
        }

        /// <summary>
        ///     Gets the cross product of the first vector with the second
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Cartesian3D GetCrossProduct(this Cartesian3D first, in Cartesian3D second)
        {
            return new Cartesian3D(first.Coordinates.GetCrossProduct(second.Coordinates));
        }

        /// <summary>
        ///     Get the spat product between three cartesian vectors
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <returns></returns>
        public static double GetSpatProduct(this Cartesian3D first, in Cartesian3D second, in Cartesian3D third)
        {
            return first.GetScalarProduct(second.GetCrossProduct(third));
        }

        /// <summary>
        ///     Calculates the projection length of this vector onto the other
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static double GetProjectionLength(this Cartesian3D first, in Cartesian3D second)
        {
            return first.GetScalarProduct(second.GetNormalized());
        }

        /// <summary>
        ///     Projects the vector onto another
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Cartesian3D GetProjection(this Cartesian3D first, in Cartesian3D second)
        {
            return second * (first.GetScalarProduct(second) / second.GetLength());
        }

        /// <summary>
        ///     Rejection of the vector with another
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Cartesian3D GetRejection(this Cartesian3D first, in Cartesian3D second)
        {
            return first - first.GetProjection(second);
        }

        /// <summary>
        ///     Returns the shortest possible vector of source type from an axis defined by the second vector (New vector is
        ///     perpendicular to second)
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Cartesian3D GetNormalVector(this Cartesian3D first, in Cartesian3D second)
        {
            return first - second.GetNormalized() * first.GetProjectionLength(second);
        }
    }
}