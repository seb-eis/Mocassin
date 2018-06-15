using System;
using System.Collections.Generic;
using ICon.Mathematics.Comparers;

namespace ICon.Mathematics.Extensions
{
    /// <summary>
    /// ICon floating point extension class that provides almost equal comparisons for FLP32 and FLP64 types
    /// </summary>
    public static class FloatingPointExtensions
    {
        /// <summary>
        /// Bit conversion of double value into 64 bit signed integer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int64 ConvertToInt64Bits(this Double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        /// <summary>
        /// Bit conversion of single value into 32 bit signed integer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32 ConvertToInt32Bits(this Single value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// Almost equal double comparion based upon ULP steps and IEEE standard (Cannot be used for 0.0 comparisons)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        public static Boolean AlmostEqualByULP(this Double value, Double other, Int32 steps)
        {
            if (value == 0.0)
            {
                throw new ArgumentException(paramName: nameof(value), message: "ULP comparisons cannot be used to compare to zero!");
            }
            if (other == 0.0)
            {
                throw new ArgumentException(paramName: nameof(value), message: "ULP comparisons cannot be used to compare to zero!");
            }
            return Math.Abs(value.ConvertToInt64Bits() - other.ConvertToInt64Bits()) <= steps;
        }

        /// <summary>
        /// Almost equal single comparion based upon ULP steps and IEEE standard (Cannot be used for 0.0 comparisons)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        public static Boolean AlmostEqualByULP(this Single value, Single other, Int32 steps)
        {
            if (value == 0.0F)
            {
                throw new ArgumentException(paramName: nameof(value), message: "ULP comparisons cannot be used to compare to zero!");
            }
            if (other == 0.0F)
            {
                throw new ArgumentException(paramName: nameof(value), message: "ULP comparisons cannot be used to compare to zero!");
            }
            return Math.Abs(value.ConvertToInt32Bits() - other.ConvertToInt32Bits()) <= steps;
        }

        /// <summary>
        /// Almost equal double comparison based upon range value (most flexible, no limitations)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Boolean AlmostEqualByRange(this Double value, Double other, Double range)
        {
            return Math.Abs(value - other) <= Math.Abs(range);
        }

        /// <summary>
        /// Almost equal single comparison based upon range value (most flexible, no limitations)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Boolean AlmostEqualByRange(this Single value, Single other, Single range)
        {
            return Math.Abs(value - other) <= Math.Abs(range);
        }

        /// <summary>
        /// Almost equal double comparison based upon relative range (cannot be used if factor or value are 0.0)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Boolean AlmostEqualByRelativeRange(this Double value, Double other, Double factor)
        {
            if (value == 0.0)
            {
                if (other == 0)
                {
                    return true;
                }
                throw new ArgumentException(paramName: nameof(value), message: "Factor comparisons cannot be used if the multiplication value is 0.0 while the other is not 0.0");
            }
            if (factor == 0.0)
            {
                throw new ArgumentException(paramName: nameof(value), message: "Factor comparisons cannot be used if the multiplication factor is 0.0");
            }
            return value.AlmostEqualByRange(other, value * factor);
        }

        /// <summary>
        /// Almost equal double comparison based upon relative range (cannot be used if factor or value are 0.0)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Boolean AlmostEqualByRelativeRange(this Single value, Single other, Single factor)
        {
            if (value == 0.0F)
            {
                throw new ArgumentException(paramName: nameof(value), message: "Factor comparisons cannot be used if the multiplication value is 0.0");
            }
            if (value == 0.0F)
            {
                throw new ArgumentException(paramName: nameof(value), message: "Factor comparisons cannot be used if the multiplication factor is 0.0");
            }
            return value.AlmostEqualByRange(other, value * factor);
        }

        /// <summary>
        /// Almost zero test for double value based upon range value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Boolean AlmostZero(this Double value, Double range)
        {
            return value.AlmostEqualByRange(0.0, range);
        }

        /// <summary>
        /// Almost zero test for single value based upon range value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Boolean AlmostZero(this Single value, Single range)
        {
            return value.AlmostEqualByRange(0.0F, range);
        }

        /// <summary>
        /// Alternative CompareTo() implementation utilizing almost equal double comparison by range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Int32 CompareToByRange(this Double value, Double other, Double range)
        {
            if (value.AlmostEqualByRange(other, range))
            {
                return 0;
            }
            return (value < other) ? -1 : 1;
        }

        /// <summary>
        /// Alternative CompareTo() implementation utilizing almost equal double comparison by range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Int32 CompareToByRange(this Single value, Single other, Single range)
        {
            if (value.AlmostEqualByRange(other, range))
            {
                return 0;
            }
            return (value < other) ? -1 : 1;
        }

        /// <summary>
        /// Trims a double value into a specified range [lower, upper) constraint
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <param name="almostEqualRange"></param>
        public static Double PeriodicTrim(this Double value, Double lowerBound, Double upperBound, Double almostEqualRange)
        {
            if (lowerBound > upperBound)
            {
                throw new ArgumentException(paramName: nameof(lowerBound), message: "Value is larger than upper bound");
            }
            if (lowerBound.AlmostEqualByRange(upperBound, almostEqualRange))
            {
                return upperBound;
            }
            Double trimValue = upperBound - lowerBound;
            value %= trimValue;
            value = (value >= lowerBound) ? value : value + trimValue;
            if (value.AlmostEqualByRange(upperBound, almostEqualRange) || value.AlmostEqualByRange(lowerBound, almostEqualRange))
            {
                return lowerBound;
            }
            return value;
        }

        /// <summary>
        /// Trims a double value into a specified range [lower, upper) constraint in steps of (upper - lower) using the provided tolerance comparer
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <param name="almostEqualRange"></param>
        public static Double PeriodicTrim(this Double value, Double lowerBound, Double upperBound, DoubleComparer comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            if (lowerBound > upperBound)
            {
                throw new ArgumentException(paramName: nameof(lowerBound), message: "Value is larger than upper bound");
            }
            if (comparer.Equals(lowerBound, upperBound))
            {
                return upperBound;
            }
            Double trimValue = upperBound - lowerBound;
            value %= trimValue;
            value = (value >= lowerBound) ? value : value + trimValue;
            if (comparer.Equals(value, upperBound) || comparer.Equals(value, lowerBound))
            {
                return lowerBound;
            }
            return value;
        }

        /// <summary>
        /// Checks if an 2D array of double values is symmetric with the provided equality comparer
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Boolean IsSymmetric(this Double[,] matrix, IEqualityComparer<Double> comparer)
        {
            if (matrix.GetUpperBound(0) != matrix.GetUpperBound(1))
            {
                return false;
            }

            for (Int32 row = 0; row < matrix.GetUpperBound(0) + 1; row++)
            {
                for (Int32 col = row + 1; col < matrix.GetUpperBound(1) + 1; col++)
                {
                    if (comparer.Equals(matrix[row,col], matrix[col,row]) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Generic check if a 2D array of any type is quadratic
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Boolean IsQuadratic<T1>(this T1[,] matrix)
        {
            return matrix.GetUpperBound(0) == matrix.GetUpperBound(1);
        }

        /// <summary>
        /// Use the provided double equality comparer and set all values that compare equal to 0.0 to actual 0.0
        /// </summary>
        /// <param name="array"></param>
        /// <param name="comparer"></param>
        public static void CleanAlmostZeroEntries(this Double[,] array, IEqualityComparer<Double> comparer)
        {
            for (Int32 row = 0; row < array.GetUpperBound(0) + 1; row++)
            {
                for (Int32 col = 0; col < array.GetUpperBound(1) + 1; col++)
                {
                    if (comparer.Equals(array[row, col], 0.0))
                    {
                        array[row, col] = 0.0;
                    }
                }
            }
        }

        /// <summary>
        /// Performs a matrix multiplication from the right (equal to lhs * rhs)
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Double[,] RightMatrixMultiplication(this Double[,] lhs, Double[,] rhs)
        {
            var lhsDim = lhs.GetDimensions();
            var rhsDim = rhs.GetDimensions();
            if (lhsDim.Cols != rhsDim.Rows)
            {
                throw new ArgumentException($"The array dimensions (lhsCols = {lhsDim.Cols}, rhsRows = {rhsDim.Rows}) are incompatible for matrix multiplication");
            }

            Double[,] resultValues = new Double[lhsDim.Rows, rhsDim.Cols];
            for (Int32 outerRow = 0; outerRow < lhsDim.Rows; outerRow++)
            {
                for (Int32 outerCol = 0; outerCol < rhsDim.Cols; outerCol++)
                {
                    for (Int32 i = 0; i < lhsDim.Cols; i++)
                    {
                        resultValues[outerRow, outerCol] += lhs[outerRow, i] * rhs[i, outerCol];
                    }
                }
            }
            return resultValues;
        }

        /// <summary>
        /// Performs a left matrix multiplication (equal to rhs * lhs)
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Double[,] LeftMatrixMultiplication(this Double[,] lhs, Double[,] rhs)
        {
            return rhs.RightMatrixMultiplication(lhs);
        }

        /// <summary>
        /// Get the dimensions of a Double[,] array
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static (Int32 Rows, Int32 Cols) GetDimensions(this Double[,] matrix)
        {
            return (matrix != null) ? (matrix.GetUpperBound(0) + 1, matrix.GetUpperBound(1) + 1) : throw new ArgumentNullException(nameof(matrix));
        }

        /// <summary>
        /// Rounds a double to the other value if close enough (Uses zero safe default ranged comparer)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Double ZeroSafeRound(this Double value, Double other)
        {
            return value.ZeroSafeRound(other, DoubleComparer.Default());
        }

        /// <summary>
        /// Rounds a double to the other value if close enough (Zero safe, throws if one of the values is exactly zero and the comparer does not support zero safe comparisons)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Double ZeroSafeRound(this Double value, Double other, DoubleComparer comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            if (value == 0.0 || other == 0.0)
            {
                if (comparer.ZeroSafe == false)
                {
                    throw new ArgumentException("One of the values is equal to 0.0 and comparer does not support zero safe comparisons", nameof(comparer));
                }
            }
            return (comparer.Equals(value, other)) ? other : value;
        }
    }
}
