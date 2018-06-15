using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.Comparers;

namespace ICon.Mathematics.Constraints
{
    /// <summary>
    /// Periodic double constraint that allows periodic parsing into the constraint range
    /// </summary>
    public class DoublePeriodicConstraint : DoubleConstraint
    {
        /// <summary>
        /// Creates new periodic double constraint with the specified components
        /// </summary>
        /// <param name="minIncluded"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="maxIncluded"></param>
        /// <param name="comparator"></param>
        public DoublePeriodicConstraint(Boolean minIncluded, Double minValue, Double maxValue, Boolean maxIncluded, DoubleComparer comparator) 
            : base(minIncluded, minValue, maxValue, maxIncluded, comparator)
        {
            if (minIncluded == false && maxIncluded == false)
            {
                throw new ArgumentException(message: "Periodic constraints require at least one included boundary value");
            }
        }

        /// <summary>
        /// Parses a double value into the constraint with the almost equal tolerance range
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Double ParseToPeriodicRange(Double sourceValue)
        {
            Double deltaValue = MaxValue - MinValue;
            while (sourceValue < MinValue)
            {
                sourceValue += deltaValue;
            }
            while (sourceValue > MaxValue)
            {
                sourceValue -= deltaValue;
            }

            if (TryParseToMinValue(sourceValue, out sourceValue) == true)
            {
                return sourceValue;
            }
            if (TryParseToMaxValue(sourceValue, out sourceValue) == true)
            {
                return sourceValue;
            }
            return sourceValue;
        }

        /// <summary>
        /// Tries to parse source value to lower limit value, sets result to source on fail and returns false
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private Boolean TryParseToMinValue(Double sourceValue, out Double result)
        {
            Int32 maxCompare = Comparer.Compare(sourceValue, MaxValue);
            if (maxCompare == 0 && MaxIncluded == true)
            {
                result = MaxValue;
                return true;
            }
            if (maxCompare == 0 && MaxIncluded == false)
            {
                result = MinValue;
                return true;
            }
            result = sourceValue;
            return false;
        }

        /// <summary>
        /// Tries to parse source value to upper limit value, sets result to source on fail and returns false
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private Boolean TryParseToMaxValue(Double sourceValue, out Double result)
        {
            Int32 minCompare = Comparer.Compare(sourceValue, MinValue);
            if (minCompare == 0 && MinIncluded == true)
            {
                result = MinValue;
                return true;
            }
            if (minCompare == 0 && MinIncluded == false)
            {
                result = MaxValue;
                return true;
            }
            result = sourceValue;
            return false;
        }
    }
}
