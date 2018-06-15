using System;
using System.Collections.Generic;

using ICon.Mathematics.Extensions;
using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Solver
{
    /// <summary>
    /// Classical Gauss-Jordan solver for multiple linear equation systems
    /// </summary>
    public class GaussJordanSolver
    {
        /// <summary>
        /// Solves the left matrix to an entity if possible, the right matrix afterwards contains the solutions of the linear equation system
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        public Boolean TrySolve(Double[,] leftMatrix, Double[,] rightMatrix, IEqualityComparer<Double> comparer)
        {
            if (CheckStartConditionsAndFixZeros(leftMatrix, rightMatrix, comparer) == false)
            {
                return false;
            }
            if (TryTransformSystemToStartConditions(leftMatrix, rightMatrix, comparer) == false)
            {
                return false;
            }
            if (TryApplyGaussJordan(leftMatrix, rightMatrix, comparer) == false)
            {
                return false;
            }

            NormalizeByLeftMatrix(leftMatrix, rightMatrix, comparer);

            return true;
        }

        /// <summary>
        /// Solves left matrix to an entity if possible, the right matrix contains the solution of the linear equation system on success
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public Boolean TrySolve(Matrix2D leftMatrix, Matrix2D rightMatrix, IEqualityComparer<Double> comparer)
        {
            return TrySolve(leftMatrix.Values, rightMatrix.Values, comparer);
        }

        /// <summary>
        /// Checks if the start conditions are met and fixes potential almost zero entries in both arrays, returns false if not met
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private Boolean CheckStartConditionsAndFixZeros(Double[,] leftMatrix, Double[,] rightMatrix, IEqualityComparer<Double> comparer)
        {
            if (leftMatrix.IsQuadratic() == false)
            {
                return false;
            }
            if (leftMatrix.GetUpperBound(0) != rightMatrix.GetUpperBound(0))
            {
                return false;
            }
            leftMatrix.CleanAlmostZeroEntries(comparer);
            rightMatrix.CleanAlmostZeroEntries(comparer);
            return true;
        }

        /// <summary>
        /// Brings the system into a condition where all diagonal entries of the left matrix are non zero, returns false if impossible
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        private Boolean TryTransformSystemToStartConditions(Double[,] leftMatrix, Double[,] rightMatrix, IEqualityComparer<Double> comparer)
        {
            Int32 rowCount = leftMatrix.GetUpperBound(0) + 1;
            for (Int32 leftRow = 0; leftRow < rowCount; leftRow++)
            {
                if (comparer.Equals(leftMatrix[leftRow,leftRow], 0.0))
                {
                    while (comparer.Equals(leftMatrix[leftRow, leftRow], 0.0))
                    {
                        for (Int32 rightRow = 0; rightRow < rowCount; rightRow++)
                        {
                            if (comparer.Equals(leftMatrix[rightRow,leftRow], 0.0) == false)
                            {
                                AddFirstRowToSecond(leftMatrix, rightRow, leftRow);
                                AddFirstRowToSecond(rightMatrix, rightRow, leftRow);
                                break;
                            }

                            // If the end of the rows is reached and no non-zero value is found, the system cannot be solved
                            if (rightRow == rightMatrix.GetUpperBound(0))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Applies the actual gauß jordan algorithm to the prepared system, returns false if non-solvable
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private Boolean TryApplyGaussJordan(Double[,] leftMatrix, Double[,] rightMatrix, IEqualityComparer<Double> comparer)
        {
            Double divisor = 0.0;
            Int32 counter = 0;
            Int32 leftRowCount = leftMatrix.GetUpperBound(0) + 1;
            Int32 leftColCount = leftMatrix.GetUpperBound(1) + 1;

            for (Int32 row = 0; row < leftRowCount; row++)
            {
                for (Int32 col = 0; col < leftColCount; col++)
                {
                    counter = 0;
                    while (comparer.Equals(leftMatrix[row, row], 0.0) && counter < (leftRowCount - row))
                    {
                        // Break if the end of rows is reached as the system is non-solvable if no non-zero entry is found
                        if (counter == leftRowCount - row - 1)
                        {
                            return false;
                        }

                        AddFirstRowToSecond(leftMatrix, row + counter, row);
                        AddFirstRowToSecond(rightMatrix, row + counter, row);
                        counter++;
                    }

                    divisor = leftMatrix[row, row];
                    if (row != col && comparer.Equals(leftMatrix[col, row], 0.0) == false)
                    {
                        divisor = leftMatrix[col, row] / divisor;
                        AddFirstRowToSecond(leftMatrix, row, col, -divisor);
                        AddFirstRowToSecond(rightMatrix, row, col, -divisor);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Uses the left matrix to normalize both matrices
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <param name="comparer"></param>
        private void NormalizeByLeftMatrix(Double[,] leftMatrix, Double[,] rightMatrix, IEqualityComparer<Double> comparer)
        {
            for (Int32 row = 0; row < leftMatrix.GetUpperBound(0) + 1; row++)
            {
                Double divisor = 1.0 / leftMatrix[row, row];
                MultiplyRowWithFactor(leftMatrix, row, divisor);
                MultiplyRowWithFactor(rightMatrix, row, divisor);
            }
            leftMatrix.CleanAlmostZeroEntries(comparer);
            rightMatrix.CleanAlmostZeroEntries(comparer);
        }

        /// <summary>
        /// Adds the source row to the target row (entry by entry) with the optional multiplication factor
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="factor"></param>
        private void AddFirstRowToSecond(Double[,] matrix, Int32 sourceRow, Int32 targetRow, Double factor = 1.0)
        {
            for (Int32 col = 0; col < matrix.GetUpperBound(1) + 1; col++)
            {
                matrix[targetRow, col] += matrix[sourceRow, col] * factor;
            }
        }

        /// <summary>
        /// Multiplies the target row with the provided factor
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="factor"></param>
        private void MultiplyRowWithFactor(Double[,] matrix, Int32 row, Double factor)
        {
            for (Int32 col = 0; col < matrix.GetUpperBound(1) + 1; col++)
            {
                matrix[row, col] *= factor;
            }
        }
    }
}
