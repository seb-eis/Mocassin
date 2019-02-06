using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Framework.Constraints
{
    /// <summary>
    /// Generic value constraint for cases where source and target type of the constraint are identical
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class ValueConstraint<TSource> : IValueConstraint<TSource, TSource> where TSource : IComparable<TSource>
    {
        /// <summary>
        /// Creates new basic value constraint with the given limitation information
        /// </summary>
        /// <param name="maxValue"></param>
        /// <param name="minValue"></param>
        /// <param name="minIsIncluded"></param>
        /// <param name="maxIsIncluded"></param>
        public ValueConstraint(bool minIsIncluded, TSource minValue, TSource maxValue, bool maxIsIncluded)
        {
            if (minValue.CompareTo(maxValue) == 1)
                throw new ArgumentException("The minimal value compares larger than the maximum value", nameof(minValue));

            MaxValue = maxValue;
            MinValue = minValue;
            MinIsIncluded = minIsIncluded;
            MaxIsIncluded = maxIsIncluded;
        }

        /// <inheritdoc />
        public TSource MaxValue { get; set; }

        /// <inheritdoc />
        public TSource MinValue { get; set; }

        /// <inheritdoc />
        public bool MinIsIncluded { get; set; }

        /// <inheritdoc />
        public bool MaxIsIncluded { get; set; }

        /// <inheritdoc />
        public virtual bool IsValid(TSource sourceValue)
        {
            var minCompare = sourceValue.CompareTo(MinValue);
            if (minCompare == -1 || minCompare == 0 && !MinIsIncluded)
                return false;

            var maxCompare = sourceValue.CompareTo(MaxValue);

            return maxCompare != 1 && (maxCompare != 0 || MaxIsIncluded);
        }

        /// <inheritdoc />
        public bool TryParse(TSource sourceValue, out TSource targetValue)
        {
            var isValid = IsValid(sourceValue);
            targetValue = isValid ? sourceValue : default;
            return isValid;
        }

        /// <summary>
        /// Display the value constraint
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{(MinIsIncluded ? "[" : "(")}{MinValue.ToString()},{MaxValue.ToString()}{(MaxIsIncluded ? "]" : ")")})";
        }
    }
}
