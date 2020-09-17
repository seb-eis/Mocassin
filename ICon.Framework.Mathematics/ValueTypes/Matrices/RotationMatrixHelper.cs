using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Provides helper functions for building <see cref="TransformMatrix2D"/> that describe a rotation
    /// </summary>
    public static class RotationMatrixHelper
    {
        /// <summary>
        ///     Creates a rotation matrix for X-Axis rotation by the given angle
        /// </summary>
        /// <param name="radian"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TransformMatrix2D CreateForAxisX(double radian, NumericComparer comparer)
        {
            var matrix = new TransformMatrix2D(
                new[,]
                {
                    {1.0, 0.0, 0.0},
                    {0.0, Math.Cos(radian), -Math.Sin(radian)},
                    {0, Math.Sin(radian), Math.Cos(radian)}
                },
                comparer);

            matrix.CleanAlmostZeros();
            return matrix;
        }

        /// <summary>
        ///     Creates a rotation matrix for Y-Axis rotation by the given angle
        /// </summary>
        /// <param name="radian"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TransformMatrix2D CreateForAxisY(double radian, IComparer<double> comparer)
        {
            var matrix = new TransformMatrix2D(
                new[,]
                {
                    {Math.Cos(radian), 0.0, Math.Sin(radian)},
                    {0.0, 1.0, 0.0},
                    {-Math.Sin(radian), 0.0, Math.Cos(radian)}
                },
                comparer);

            matrix.CleanAlmostZeros();
            return matrix;
        }

        /// <summary>
        ///     Creates a rotation matrix for Z-Axis rotation by the given angle
        /// </summary>
        /// <param name="radian"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TransformMatrix2D CreateForAxisZ(double radian, NumericComparer comparer)
        {
            var matrix = new TransformMatrix2D(
                new[,]
                {
                    {Math.Cos(radian), -Math.Sin(radian), 0.0},
                    {Math.Sin(radian), Math.Cos(radian), 0.0},
                    {0.0, 0.0, 1.0}
                },
                comparer);

            matrix.CleanAlmostZeros();
            return matrix;
        }

        /// <summary>
        ///     Creates a rotation matrix for arbitrary axis rotation in positive direction
        /// </summary>
        /// <param name="radian"></param>
        /// <param name="axis"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TransformMatrix2D CreateForArbitraryAxis(double radian, in Cartesian3D axis, NumericComparer comparer)
        {
            if (comparer.Compare(axis.GetLength(), 0.0) == 0)
                throw new ArgumentException("Cannot created rotation axis around a zero vector", nameof(axis));

            var matrix = new TransformMatrix2D(comparer);
            var normAxis = axis.GetNormalized();
            var angleCosine = 1.0 - Math.Cos(radian);

            matrix[0, 0] = normAxis.X * normAxis.X * angleCosine + Math.Cos(radian);
            matrix[0, 1] = normAxis.X * normAxis.Y * angleCosine - normAxis.Z * Math.Sin(radian);
            matrix[0, 2] = normAxis.X * normAxis.Z * angleCosine + normAxis.Y * Math.Sin(radian);
            matrix[1, 0] = normAxis.Y * normAxis.X * angleCosine + normAxis.Z * Math.Sin(radian);
            matrix[1, 1] = normAxis.Y * normAxis.Y * angleCosine + Math.Cos(radian);
            matrix[1, 2] = normAxis.Y * normAxis.Z * angleCosine - normAxis.X * Math.Sin(radian);
            matrix[2, 0] = normAxis.X * normAxis.Z * angleCosine - normAxis.Y * Math.Sin(radian);
            matrix[2, 1] = normAxis.Y * normAxis.Z * angleCosine + normAxis.X * Math.Sin(radian);
            matrix[2, 2] = normAxis.Z * normAxis.Z * angleCosine + Math.Cos(radian);

            matrix.CleanAlmostZeros();
            return matrix;
        }
    }
}