using System;
using System.Collections;
using System.Collections.Generic;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.Extensions;
using ICon.Mathematics.Solver;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Double precision two dimensional matrix of arbitrary size, wraps double[,] to restrict access and provide matrix functionality
    /// </summary>
    public class Matrix2D : IEquatable<Matrix2D>
    {
        /// <summary>
        /// The wrapped 2D array of double values
        /// </summary>
        public double[,] Values { get; protected set; }

        /// <summary>
        /// The internal double comparison object
        /// </summary>
        public IComparer<double> Comparer { get; protected set; }

        /// <summary>
        /// The number of rows
        /// </summary>
        public int Rows { get; protected set; }

        /// <summary>
        /// The number of cols
        /// </summary>
        public int Cols { get; protected set; }

        /// <summary>
        /// Construct new matrix from 2D array and double comparer
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="values"></param>
        public Matrix2D(double[,] values, IComparer<double> comparer)
        {
            Comparer = comparer ?? DoubleComparer.Default();
            Values = values ?? throw new ArgumentNullException(nameof(values));
            Rows = values.GetUpperBound(0) + 1;
            Cols = values.GetUpperBound(1) + 1;
        }

        /// <summary>
        /// Creates  new matrix of the specified dimensions
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public Matrix2D(int rows, int cols, IComparer<double> comparer)
        {
            Comparer = comparer ?? DoubleComparer.Default();
            Rows = rows;
            Cols = cols;
            Values = new double[rows, cols];
        }

        /// <summary>
        /// Access the matrix by indexer [row,col]
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public double this[int row, int col] 
        { 
            get => Values[row, col];
            set => Values[row, col] = value;
        }

        /// <summary>
        /// Fixes all almost zero entries to zero
        /// </summary>
        public void CleanAlmostZeros()
        {
            Values.CleanAlmostZeroEntries(Comparer);
        }
        /// <summary>
        /// Compares for almost equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Matrix2D other)
        {
            if (Rows != other.Rows || Cols != other.Cols)
            {
                return false;
            }

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    if (Comparer.Compare(Values[row,col], other.Values[row,col]) == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the matrix is quadratic
        /// </summary>
        /// <returns></returns>
        public bool IsQuadratic()
        {
            return Rows == Cols;
        }

        /// <summary>
        /// Checks if the matrix is symmetric
        /// </summary>
        /// <returns></returns>
        public bool IsSymmetric()
        {
            return Values.IsSymmetric(Comparer);
        }

        /// <summary>
        /// Creates and entity matrix of the specified size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Matrix2D GetEntity(int size, IComparer<double> comparer = null)
        {
            var entity = new double[size, size];
            for (var i = 0; i < size; i++)
            {
                entity[i, i] = 1.0;
            }
            return new Matrix2D(entity, comparer);
        }

        /// <summary>
        /// Tries to inverse the matrix, returns null if failed
        /// </summary>
        /// <returns></returns>
        public Matrix2D GetInverse()
        {
            if (IsQuadratic() == false)
            {
                return null;
            }
            var result = GetEntity(Rows, Comparer);
            var solver = new GaussJordanSolver();
            if (solver.TrySolve(Values, result.Values, Comparer) == false)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Returns the a new matrix that is the transposed of the current
        /// </summary>
        /// <returns></returns>
        public Matrix2D GetTransposed()
        {
            var resultValues = new double[Cols, Rows];
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    resultValues[col, row] = Values[row, col];
                }
            }
            return new Matrix2D(resultValues, Comparer);
        }

        /// <summary>
        /// Get a matrix where each row is reversed
        /// </summary>
        /// <returns></returns>
        public Matrix2D GetRowReversed()
        {
            var result = new Matrix2D(Rows, Cols, Comparer);

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    result[row, col] = this[row, Cols - col - 1];
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the trace of the matrix, returns NaN if not possible
        /// </summary>
        /// <returns></returns>
        public double GetTrace()
        {
            if (IsQuadratic() == false)
            {
                return double.NaN;
            }
            var result = 0.0;
            for (var i = 0; i < Rows; i++)
            {
                result += Values[i, i];
            }
            return result;
        }

        /// <summary>
        /// Adds the matrix onto the current if possible
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public void Add(Matrix2D rhs)
        {
            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }
            if (Rows != rhs.Rows || Cols != rhs.Cols)
            {
                throw new ArgumentException(paramName: nameof(rhs), message: "Matrix for addition is of wrong size");
            }
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    Values[row, col] += rhs.Values[row, col];
                }
            }
        }

        /// <summary>
        /// Subtracts the matrix from the current if possible
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public void Subtract(Matrix2D rhs)
        {
            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }
            if (Rows != rhs.Rows || Cols != rhs.Cols)
            {
                throw new ArgumentException(paramName: nameof(rhs), message: "Matrix for subtraction is of wrong size");
            }
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    Values[row, col] -= rhs.Values[row, col];
                }
            }
        }

        /// <summary>
        /// Performs multiplication with provided scalar value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void ScalarMultiply(double value)
        {
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    Values[row, col] *= value;
                }
            }
        }

        /// <summary>
        /// Performs division by provide scalar value
        /// </summary>
        /// <param name="value"></param>
        public void ScalarDivide(double value)
        {
            if (Comparer.Compare(value, 0.0) == 0)
            {
                throw new DivideByZeroException("Matrix value division was passed an almost zero value");
            }
            ScalarMultiply(1.0 / value);
        }

       /// <summary>
       /// Matrix multiplication operator
       /// </summary>
       /// <param name="lhs"></param>
       /// <param name="rhs"></param>
       /// <returns></returns>
        public static Matrix2D operator* (Matrix2D lhs, Matrix2D rhs)
        {
            return new Matrix2D(lhs.Values.RightMatrixMultiplication(rhs.Values), lhs.Comparer);
        }
    }
}
