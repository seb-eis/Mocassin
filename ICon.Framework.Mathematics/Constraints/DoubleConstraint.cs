using System;
using System.Collections.Generic;

using ICon.Framework.Constraints;
using ICon.Mathematics.Comparers;

namespace ICon.Mathematics.Constraints
{
    /// <summary>
    /// Double constraint class that restricts the definition range of a double value
    /// </summary>
    public class DoubleConstraint : ValueConstraint<Double>
    {
        /// <summary>
        /// The double comparator object
        /// </summary>
        public DoubleComparer Comparer { get; set; }

        /// <summary>
        /// Constructor, takes constraint information and double comparator
        /// </summary>
        /// <param name="maxValue"></param>
        /// <param name="minValue"></param>
        /// <param name="minIsIncluded"></param>
        /// <param name="maxIsIncluded"></param>
        /// <param name="comparer"></param>
        public DoubleConstraint(Boolean minIsIncluded, Double minValue, Double maxValue, Boolean maxIsIncluded, DoubleComparer comparer) : base(minIsIncluded, minValue, maxValue, maxIsIncluded)
        {
            Comparer = comparer;
        }

        /// <summary>
        /// Check if a source value is valid within the contraint information
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public override Boolean IsValid(Double sourceValue)
        {
            Int32 minCompare = Comparer.Compare(sourceValue, MinValue);
            if (minCompare == -1 || (minCompare == 0 && MinIsIncluded == false))
            {
                return false;
            }
            Int32 maxCompare = Comparer.Compare(sourceValue, MaxValue);
            if (maxCompare == 1 || (maxCompare == 0 && MaxIsIncluded == false))
            {
                return false;
            }
            return true;
        }
    }
}
