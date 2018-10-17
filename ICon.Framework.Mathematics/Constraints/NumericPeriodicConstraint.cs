using System;
using Mocassin.Mathematics.Comparers;

namespace Mocassin.Mathematics.Constraints
{
    /// <summary>
    ///     Periodic numeric constraint that allows periodic parsing into the constraint range
    /// </summary>
    public class NumericPeriodicConstraint : NumericConstraint
    {
        /// <inheritdoc />
        public NumericPeriodicConstraint(bool minIsIncluded, double minValue, double maxValue, bool maxIsIncluded,
            NumericComparer comparator)
            : base(minIsIncluded, minValue, maxValue, maxIsIncluded, comparator)
        {
            if (minIsIncluded == false && maxIsIncluded == false)
                throw new ArgumentException("Periodic constraints require at least one included boundary value");
        }

        /// <summary>
        ///     Parses a double value into the constraint with the almost equal tolerance range
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public double ParseToPeriodicRange(double sourceValue)
        {
            var deltaValue = MaxValue - MinValue;
            while (sourceValue < MinValue) 
                sourceValue += deltaValue;

            while (sourceValue > MaxValue) 
                sourceValue -= deltaValue;

            if (TryParseToMinValue(sourceValue, out sourceValue))
                return sourceValue;

            TryParseToMaxValue(sourceValue, out sourceValue);
                return sourceValue;
        }

        /// <summary>
        ///     Tries to parse source value to lower limit value, sets result to source on fail and returns false
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryParseToMinValue(double sourceValue, out double result)
        {
            var maxCompare = Comparer.Compare(sourceValue, MaxValue);
            switch (maxCompare)
            {
                case 0 when MaxIsIncluded:
                    result = MaxValue;
                    return true;

                case 0 when !MaxIsIncluded:
                    result = MinValue;
                    return true;

                default:
                    result = sourceValue;
                    return false;
            }
        }

        /// <summary>
        ///     Tries to parse source value to upper limit value, sets result to source on fail and returns false
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryParseToMaxValue(double sourceValue, out double result)
        {
            var minCompare = Comparer.Compare(sourceValue, MinValue);
            switch (minCompare)
            {
                case 0 when MinIsIncluded:
                    result = MinValue;
                    return true;

                case 0 when !MinIsIncluded:
                    result = MaxValue;
                    return true;

                default:
                    result = sourceValue;
                    return false;
            }
        }
    }
}