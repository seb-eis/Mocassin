using System;

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
        public Double[,] Values { get; protected set; }

        /// <summary>
        /// The internal double comparison object
        /// </summary>
        public DoubleComparer Comparer { get; protected set; }


        /// <summary>
        /// The number of rows
        /// </summary>
        public Int32 Rows { get; protected set; }

        /// <summary>
        /// The number of cols
        /// </summary>
        public Int32 Cols { get; protected set; }

        /// <summary>
        /// Construct new matrix from 2D array and double comparer
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="values"></param>
        public Matrix2D(Double[,] values, DoubleComparer comparer)
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
        public Matrix2D(Int32 rows, Int32 cols, DoubleComparer comparer)
        {
            Comparer = comparer ?? DoubleComparer.Default();
            Rows = rows;
            Cols = cols;
            Values = new Double[rows, cols];
        }

        /// <summary>
        /// Acces the matrix by indexer [row,col]
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Double this[Int32 row, Int32 col] { get { return Values[row, col]; } set { Values[row, col] = value; } }

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
        public Boolean Equals(Matrix2D other)
        {
            if (Rows != other.Rows || Cols != other.Cols)
            {
                return false;
            }

            for (Int32 row = 0; row < Rows; row++)
            {
                for (Int32 col = 0; col < Cols; col++)
                {
                    if (Comparer.Equals(Values[row,col],other.Values[row,col]) == false)
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
        public Boolean IsQuadratic()
        {
            return Rows == Cols;
        }

        /// <summary>
        /// Checks if the matrix is symmetric
        /// </summary>
        /// <returns></returns>
        public Boolean IsSymmetric()
        {
            return Values.IsSymmetric(Comparer);
        }

        /// <summary>
        /// Creates and entity matrix of the specified size
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static Matrix2D GetEntity(Int32 size, DoubleComparer comparer = null)
        {
            Double[,] entity = new Double[size, size];
            for (Int32 i = 0; i < size; i++)
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
            var result = Matrix2D.GetEntity(Rows, Comparer);
            var solver = new GaussJordanSolver();
            if (solver.TrySolve(Values, result.Values, Comparer) == false)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Returns the a new matrix that is the tranposed of the current
        /// </summary>
        /// <returns></returns>
        public Matrix2D GetTransposed()
        {
            Double[,] resultValues = new Double[Cols, Rows];
            for (Int32 row = 0; row < Rows; row++)
            {
                for (Int32 col = 0; col < Cols; col++)
                {
                    resultValues[col, row] = Values[row, col];
                }
            }
            return new Matrix2D(resultValues, Comparer);
        }

        /// <summary>
        /// Calculates the trace of the matrix, returns NaN if not possible
        /// </summary>
        /// <returns></returns>
        public Double GetTrace()
        {
            if (IsQuadratic() == false)
            {
                return Double.NaN;
            }
            Double result = 0.0;
            for (Int32 i = 0; i < Rows; i++)
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
            for (Int32 row = 0; row < Rows; row++)
            {
                for (Int32 col = 0; col < Cols; col++)
                {
                    Values[row, col] += rhs.Values[row, col];
                }
            }
        }

        /// <summary>
        /// Substracts the matrix from the current if possible
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public void Substract(Matrix2D rhs)
        {
            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }
            if (Rows != rhs.Rows || Cols != rhs.Cols)
            {
                throw new ArgumentException(paramName: nameof(rhs), message: "Matrix for substraction is of wrong size");
            }
            for (Int32 row = 0; row < Rows; row++)
            {
                for (Int32 col = 0; col < Cols; col++)
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
        public void ScalarMultiply(Double value)
        {
            for (Int32 row = 0; row < Rows; row++)
            {
                for (Int32 col = 0; col < Cols; col++)
                {
                    Values[row, col] *= value;
                }
            }
        }

        /// <summary>
        /// Performs division by provide scalar value
        /// </summary>
        /// <param name="value"></param>
        public void ScalarDivide(Double value)
        {
            if (Comparer.Equals(value, 0.0))
            {
                throw new DivideByZeroException(message: "Matrix value division was passed an almost zero value");
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
