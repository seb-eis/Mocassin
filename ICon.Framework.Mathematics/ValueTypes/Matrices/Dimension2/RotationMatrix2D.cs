using System;
using System.Collections.Generic;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Rotation matrix for 3D vectors, contains factories for common cartesian operations
    /// </summary>
    public class RotationMatrix2D : TransformMatrix2D
    {
        public RotationMatrix2D(double[,] values, IComparer<double> comparer = null) : base(values, comparer)
        {
        }

        public RotationMatrix2D(IComparer<double> comparer) : base(comparer)
        {
        }

        /// <summary>
        /// Creates a rotation matrix for X-Axis rotation by the given angle
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static TransformMatrix2D CreateForAxisX(double radian, DoubleComparer comparer)
        {
            var matrix = new TransformMatrix2D(new double[3, 3] { { 1.0, 0.0, 0.0 }, { 0.0, Math.Cos(radian), -Math.Sin(radian) }, { 0, Math.Sin(radian), Math.Cos(radian) } }, comparer);
            matrix.CleanAlmostZeros();
            return matrix;
        }

        /// <summary>
        /// Creates a rotation matrix for Y-Axis rotation by the given angle
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static TransformMatrix2D CreateForAxisY(double radian, IComparer<double> comparer)
        {
            var matrix = new TransformMatrix2D(new double[3, 3] { { Math.Cos(radian), 0.0, Math.Sin(radian) }, { 0.0, 1.0, 0.0 }, { -Math.Sin(radian), 0.0, Math.Cos(radian) } }, comparer);
            matrix.CleanAlmostZeros();
            return matrix;
        }

        /// <summary>
        /// Creates a rotation matrix for Z-Axis rotation by the given angle
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static TransformMatrix2D CreateForAxisZ(double radian, DoubleComparer comparer)
        {
            var matrix = new TransformMatrix2D(new double[3, 3] { { Math.Cos(radian), -Math.Sin(radian), 0.0 }, { Math.Sin(radian), Math.Cos(radian), 0.0 }, { 0.0, 0.0, 1.0 } }, comparer);
            matrix.CleanAlmostZeros();
            return matrix;
        }

        /// <summary>
        /// Creates a rotation matrix for arbitrary axis rotation in positive direction
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static TransformMatrix2D CreateForArbitraryAxis<T1>(double radian, T1 axis, DoubleComparer comparer) where T1 : struct, ICartesian3D<T1>
        {
            if (comparer.Compare(axis.GetLength(), 0.0) == 0) throw new ArgumentException("Cannot created rotation axis around a zero vector", nameof(axis));
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

        /// <summary>
        /// Multiplies with another transformation matrix
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static RotationMatrix2D operator *(RotationMatrix2D lhs, RotationMatrix2D rhs)
        {
            return new RotationMatrix2D(lhs.Values.RightMatrixMultiplication(rhs.Values), lhs.Comparer);
        }
    }
}
