using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Comparers;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Mocassin.Mathematics.Extensions
{
    /// <summary>
    ///     ICon floating point extension class that provides almost equal comparisons for FLP32 and FLP64 types
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        ///     Bit conversion of double value into 64 bit signed integer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ConvertToInt64Bits(this double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        /// <summary>
        ///     Bit conversion of single value into 32 bit signed integer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertToInt32Bits(this float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        ///     Almost equal double comparison based upon ULP steps and IEEE standard (Cannot be used for 0.0 comparisons)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualByUlp(this double value, double other, int steps)
        {
            if (value == 0.0)
                throw new ArgumentException(paramName: nameof(value), message: "ULP comparisons cannot be used to compare to zero!");

            if (other == 0.0)
                throw new ArgumentException(paramName: nameof(value), message: "ULP comparisons cannot be used to compare to zero!");

            return Math.Abs(value.ConvertToInt64Bits() - other.ConvertToInt64Bits()) <= steps;
        }

        /// <summary>
        ///     Almost equal double comparison based upon range value (most flexible, no limitations)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualByRange(this double value, double other, double range)
        {
            return Math.Abs(value - other) <= Math.Abs(range);
        }

        /// <summary>
        ///     Almost equal double comparison based upon relative range (cannot be used if factor or value are 0.0)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualByRelative(this double value, double other, double factor)
        {
            if (value == 0.0)
            {
                if (other == 0.0) 
                    return true;

                throw new ArgumentException(paramName: nameof(value),
                    message: "Factor comparisons cannot be used if the multiplication value is 0.0 while the other is not 0.0");
            }

            if (factor == 0.0)
                throw new ArgumentException(paramName: nameof(value),
                    message: "Factor comparisons cannot be used if the multiplication factor is 0.0");
            return value.IsAlmostEqualByRange(other, value * factor);
        }

        /// <summary>
        ///     Almost zero test for double value based upon range value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsAlmostZero(this double value, double range)
        {
            return value.IsAlmostEqualByRange(0.0, range);
        }

        /// <summary>
        ///     Alternative CompareTo() implementation utilizing almost equal double comparison by range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int CompareToByRange(this double value, double other, double range)
        {
            if (value.IsAlmostEqualByRange(other, range))
                return 0;

            return value < other ? -1 : 1;
        }

        /// <summary>
        ///     Trims a double value into a specified range [lower, upper) constraint
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <param name="almostEqualRange"></param>
        public static double PeriodicTrim(this double value, double lowerBound, double upperBound, double almostEqualRange)
        {
            if (lowerBound > upperBound)
                throw new ArgumentException(paramName: nameof(lowerBound), message: "Value is larger than upper bound");

            if (lowerBound.IsAlmostEqualByRange(upperBound, almostEqualRange))
                return upperBound;

            var trimValue = upperBound - lowerBound;
            value %= trimValue;
            value = value >= lowerBound ? value : value + trimValue;
            if (value.IsAlmostEqualByRange(upperBound, almostEqualRange) || value.IsAlmostEqualByRange(lowerBound, almostEqualRange))
                return lowerBound;
            return value;
        }

        /// <summary>
        ///     Trims a double value into a specified range [lower, upper) constraint in steps of (upper - lower) using the
        ///     provided tolerance comparer
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <param name="comparer"></param>
        public static double PeriodicTrim(this double value, double lowerBound, double upperBound, NumericComparer comparer)
        {
            if (comparer == null) 
                throw new ArgumentNullException(nameof(comparer));

            if (lowerBound > upperBound)
                throw new ArgumentException(paramName: nameof(lowerBound), message: "Value is larger than upper bound");

            if (comparer.Equals(lowerBound, upperBound))
                return upperBound;

            var trimValue = upperBound - lowerBound;
            value %= trimValue;
            value = value >= lowerBound ? value : value + trimValue;
            if (comparer.Equals(value, upperBound) || comparer.Equals(value, lowerBound))
                return lowerBound;

            return value;
        }

        /// <summary>
        ///     Checks if an 2D array of double values is symmetric with the provided equality comparer
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool IsSymmetric(this double[,] matrix, IComparer<double> comparer)
        {
            if (matrix.GetUpperBound(0) != matrix.GetUpperBound(1)) return false;

            for (var row = 0; row < matrix.GetUpperBound(0) + 1; row++)
            {
                for (var col = row + 1; col < matrix.GetUpperBound(1) + 1; col++)
                    if (comparer.Compare(matrix[row, col], matrix[col, row]) != 0)
                        return false;
            }

            return true;
        }

        /// <summary>
        ///     Generic check if a 2D array of any type is quadratic
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsQuadratic<T1>(this T1[,] matrix)
        {
            return matrix.GetUpperBound(0) == matrix.GetUpperBound(1);
        }

        /// <summary>
        ///     Use the provided double equality comparer and set all values that compare equal to 0.0 to actual 0.0
        /// </summary>
        /// <param name="array"></param>
        /// <param name="comparer"></param>
        public static void CleanAlmostZeroEntries(this double[,] array, IComparer<double> comparer)
        {
            for (var row = 0; row < array.GetUpperBound(0) + 1; row++)
            {
                for (var col = 0; col < array.GetUpperBound(1) + 1; col++)
                    if (comparer.Compare(array[row, col], 0.0) == 0)
                        array[row, col] = 0.0;
            }
        }

        /// <summary>
        ///     Performs a matrix multiplication from the right (equal to lhs * rhs)
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static double[,] RightMatrixMultiplication(this double[,] lhs, double[,] rhs)
        {
            var (lhsRows, lhsCols) = lhs.GetDimensions();
            var (rhsRows, rhsCols) = rhs.GetDimensions();

            if (lhsCols != rhsRows)
                throw new ArgumentException(
                    $"The array dimensions (lhsCols = {lhsCols}, rhsRows = {rhsRows}) are incompatible for matrix multiplication");

            var resultValues = new double[lhsRows, rhsCols];
            for (var outerRow = 0; outerRow < lhsRows; outerRow++)
            {
                for (var outerCol = 0; outerCol < rhsCols; outerCol++)
                {
                    for (var i = 0; i < lhsCols; i++) resultValues[outerRow, outerCol] += lhs[outerRow, i] * rhs[i, outerCol];
                }
            }

            return resultValues;
        }

        /// <summary>
        ///     Performs a left matrix multiplication (equal to rhs * lhs)
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static double[,] LeftMatrixMultiplication(this double[,] lhs, double[,] rhs)
        {
            return rhs.RightMatrixMultiplication(lhs);
        }

        /// <summary>
        ///     Get the dimensions of a Double[,] array
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static (int Rows, int Cols) GetDimensions(this double[,] matrix)
        {
            return matrix != null
                ? (matrix.GetUpperBound(0) + 1, matrix.GetUpperBound(1) + 1)
                : throw new ArgumentNullException(nameof(matrix));
        }

        /// <summary>
        ///     Rounds a double to the other value if close enough (Uses zero safe default ranged comparer)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double ZeroSafeRound(this double value, double other)
        {
            return value.ZeroSafeRound(other, NumericComparer.Default());
        }

        /// <summary>
        ///     Rounds a double to the other value if close enough (Zero safe, throws if one of the values is exactly zero and the
        ///     comparer does not support zero safe comparisons)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static double ZeroSafeRound(this double value, double other, NumericComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (value != 0.0 && other != 0.0) 
                return comparer.Equals(value, other) ? other : value;

            if (comparer.IsZeroCompatible == false)
                throw new ArgumentException("One of the values is equal to 0.0 and comparer does not support zero safe comparisons",
                    nameof(comparer));
            return comparer.Equals(value, other) ? other : value;
        }
    }
}