﻿using System;

using ICon.Mathematics.Extensions;
using ICon.Mathematics.Comparers;

using ACoorTuple = ICon.Mathematics.ValueTypes.Coordinates<System.Double, System.Double, System.Double>;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Transformation matrix for three dimensional vector systems, matrix is always 3x3
    /// </summary>
    public class TransformMatrix2D : Matrix2D
    {
        /// <summary>
        /// Creates a new transformation matrix from the provided 3x3 array and value comparer (Uses default if null)
        /// </summary>
        /// <param name="values"></param>
        /// <param name="comparer"></param>
        public TransformMatrix2D(Double[,] values, DoubleComparer comparer) : base(values, comparer)
        {
            if (Values.GetUpperBound(0) != 2 || Values.GetUpperBound(1) != 2)
            {
                throw new ArgumentException("Input array is not of size 3x3", nameof(values));
            }
        }

        /// <summary>
        /// Creates new empty transformation matrix with the specified comparer
        /// </summary>
        /// <param name="comparer"></param>
        public TransformMatrix2D(DoubleComparer comparer) : base(3, 3, comparer)
        {

        }

        /// <summary>
        /// Sets a specific row of a rotation matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        private void SetRowEntries(Int32 row, Double a, Double b, Double c)
        {
            this[row, 0] = a;
            this[row, 1] = b;
            this[row, 2] = c;
        }

        /// <summary>
        /// Generic application of the rotation onto a generic cartesian vector
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T1 Transform<T1>(T1 vector) where T1 : struct, IVector3D<T1>
        {
            return Values.MultiplyWith(vector);
        }

        /// <summary>
        /// Transform operation for unspecified 3D coordinate tuple
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public ACoorTuple Transform(ACoorTuple vector)
        {
           return Values.MultiplyWith(vector);
        }

        /// <summary>
        /// Transform operation for non generic basic cartesian vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Cartesian3D Transform(Cartesian3D vector)
        {
            return Values.MultiplyWith(vector);
        }


        /// <summary>
        /// Transform operation for non generic basic fractional vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Fractional3D Transform(Fractional3D vector)
        {
            return Values.MultiplyWith(vector);
        }


        /// <summary>
        /// Transform operation for non generic basic spherical vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Spherical3D Transform(Spherical3D vector)
        {
            return Values.MultiplyWith(vector);
        }

        /// <summary>
        /// Matrix vector multiplication with unspecified 3D coordinate tuple
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static ACoorTuple operator *(TransformMatrix2D matrix, ACoorTuple vector)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// Matrix vector multiplication with basic cartesian vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Cartesian3D operator *(TransformMatrix2D matrix, Cartesian3D vector)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// Matrix vector multiplication with basic fractional vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Fractional3D operator *(TransformMatrix2D matrix, Fractional3D vector)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// Matrix vector multiplication with basic spherical vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Spherical3D operator *(TransformMatrix2D matrix, Spherical3D vector)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// Multiplies with another transformation matrix
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static TransformMatrix2D operator* (TransformMatrix2D lhs, TransformMatrix2D rhs)
        {
            return new TransformMatrix2D(lhs.Values.RightMatrixMultiplication(rhs.Values), lhs.Comparer);
        }
    }
}
