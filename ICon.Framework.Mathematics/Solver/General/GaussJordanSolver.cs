using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Solver
{
    /// <summary>
    ///     Classical Gauss-Jordan solver implementation for multiple linear equation systems
    /// </summary>
    public class GaussJordanSolver
    {
        /// <summary>
        ///     Solves the left matrix to an entity if possible, the right matrix afterwards contains the solutions of the linear
        ///     equation system
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        public bool TrySolve(double[,] leftMatrix, double[,] rightMatrix, IComparer<double> comparer)
        {
            if (!CheckStartConditionsAndFixZeros(leftMatrix, rightMatrix, comparer))
                return false;

            if (!TryTransformSystemToStartConditions(leftMatrix, rightMatrix, comparer))
                return false;

            if (!TryGaussJordanSolve(leftMatrix, rightMatrix, comparer))
                return false;

            NormalizeByLeftMatrix(leftMatrix, rightMatrix, comparer);

            return true;
        }

        /// <summary>
        ///     Solves left matrix to an entity if possible, the right matrix contains the solution of the linear equation system
        ///     on success
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool TrySolve(Matrix2D leftMatrix, Matrix2D rightMatrix, IComparer<double> comparer)
        {
            return TrySolve(leftMatrix.Values, rightMatrix.Values, comparer);
        }

        /// <summary>
        ///     Checks if the start conditions are met and fixes potential almost zero entries in both arrays, returns false if not
        ///     met
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private static bool CheckStartConditionsAndFixZeros(double[,] leftMatrix, double[,] rightMatrix, IComparer<double> comparer)
        {
            if (!leftMatrix.IsQuadratic()) 
                return false;

            if (leftMatrix.GetUpperBound(0) != rightMatrix.GetUpperBound(0)) 
                return false;

            leftMatrix.CleanAlmostZeroEntries(comparer);
            rightMatrix.CleanAlmostZeroEntries(comparer);
            return true;
        }

        /// <summary>
        ///     Brings the system into a condition where all diagonal entries of the left matrix are non zero, returns false if
        ///     impossible
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        private bool TryTransformSystemToStartConditions(double[,] leftMatrix, double[,] rightMatrix, IComparer<double> comparer)
        {
            var rowCount = leftMatrix.GetUpperBound(0) + 1;
            for (var leftRow = 0; leftRow < rowCount; leftRow++)
            {
                if (comparer.Compare(leftMatrix[leftRow, leftRow], 0.0) != 0)
                    continue;

                while (comparer.Compare(leftMatrix[leftRow, leftRow], 0.0) == 0)
                {
                    for (var rightRow = 0; rightRow < rowCount; rightRow++)
                    {
                        if (comparer.Compare(leftMatrix[rightRow, leftRow], 0.0) != 0)
                        {
                            AddFirstRowToSecond(leftMatrix, rightRow, leftRow);
                            AddFirstRowToSecond(rightMatrix, rightRow, leftRow);
                            break;
                        }

                        // If the end of the rows is reached and no non-zero value is found, the system cannot be solved
                        if (rightRow == rightMatrix.GetUpperBound(0))
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Applies the actual gauss jordan algorithm to the prepared system, returns false if non-solvable
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private bool TryGaussJordanSolve(double[,] leftMatrix, double[,] rightMatrix, IComparer<double> comparer)
        {
            var leftRowCount = leftMatrix.GetUpperBound(0) + 1;
            var leftColCount = leftMatrix.GetUpperBound(1) + 1;

            for (var row = 0; row < leftRowCount; row++)
            {
                for (var col = 0; col < leftColCount; col++)
                {
                    var counter = 0;
                    while (comparer.Compare(leftMatrix[row, row], 0.0) == 0 && counter < leftRowCount - row)
                    {
                        // Break if the end of rows is reached as the system is non-solvable if no non-zero entry is found
                        if (counter == leftRowCount - row - 1) return false;

                        AddFirstRowToSecond(leftMatrix, row + counter, row);
                        AddFirstRowToSecond(rightMatrix, row + counter, row);
                        counter++;
                    }

                    var divisor = leftMatrix[row, row];
                    if (row == col || comparer.Compare(leftMatrix[col, row], 0.0) == 0)
                        continue;

                    divisor = leftMatrix[col, row] / divisor;
                    AddFirstRowToSecond(leftMatrix, row, col, -divisor);
                    AddFirstRowToSecond(rightMatrix, row, col, -divisor);
                }
            }

            return true;
        }

        /// <summary>
        ///     Uses the left matrix to normalize both matrices
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        private void NormalizeByLeftMatrix(double[,] leftMatrix, double[,] rightMatrix, IComparer<double> comparer)
        {
            for (var row = 0; row < leftMatrix.GetUpperBound(0) + 1; row++)
            {
                var divisor = 1.0 / leftMatrix[row, row];
                MultiplyRowWithFactor(leftMatrix, row, divisor);
                MultiplyRowWithFactor(rightMatrix, row, divisor);
            }

            leftMatrix.CleanAlmostZeroEntries(comparer);
            rightMatrix.CleanAlmostZeroEntries(comparer);
        }

        /// <summary>
        ///     Adds the source row to the target row (entry by entry) with the optional multiplication factor
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="targetRow"></param>
        /// <param name="factor"></param>
        /// <param name="sourceRow"></param>
        private static void AddFirstRowToSecond(double[,] matrix, int sourceRow, int targetRow, double factor = 1.0)
        {
            for (var col = 0; col < matrix.GetUpperBound(1) + 1; col++)
                matrix[targetRow, col] += matrix[sourceRow, col] * factor;
        }

        /// <summary>
        ///     Multiplies the target row with the provided factor
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="factor"></param>
        private static void MultiplyRowWithFactor(double[,] matrix, int row, double factor)
        {
            for (var col = 0; col < matrix.GetUpperBound(1) + 1; col++)
                matrix[row, col] *= factor;
        }
    }
}