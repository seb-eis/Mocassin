using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Constraints
{
    /// <summary>
    /// Generic basic constraint class for source types that implement IComparable and where target type is same as source type
    /// </summary>
    /// <typeparam name="Source"></typeparam>
    public class ValueConstraint<Source> : IValueConstraint<Source, Source> where Source : struct, IComparable<Source>
    {
        /// <summary>
        /// Creates new basic value constraint with the given limitation information
        /// </summary>
        /// <param name="maxValue"></param>
        /// <param name="minValue"></param>
        /// <param name="minIncluded"></param>
        /// <param name="maxIncluded"></param>
        public ValueConstraint(Boolean minIncluded, Source minValue, Source maxValue, Boolean maxIncluded)
        {
            if (minValue.CompareTo(maxValue) == 1)
            {
                throw new ArgumentException(paramName: nameof(minValue), message: "The minimal value compares larger than the maximum value");
            }

            MaxValue = maxValue;
            MinValue = minValue;
            MinIncluded = minIncluded;
            MaxIncluded = maxIncluded;
        }

        /// <summary>
        /// The upper value boundary
        /// </summary>
        public Source MaxValue { get; set; }

        /// <summary>
        /// The lower value boundary
        /// </summary>
        public Source MinValue { get; set; }

        /// <summary>
        /// Indicator if the lower boundary is included
        /// </summary>
        public Boolean MinIncluded { get; set; }

        /// <summary>
        /// Indicator if the upper boundary is included
        /// </summary>
        public Boolean MaxIncluded { get; set; }

        /// <summary>
        /// Validates if the source value is valid in the constraint context
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public virtual Boolean IsValid(Source sourceValue)
        {
            Int32 minCompare = sourceValue.CompareTo(MinValue);
            if (minCompare == -1 || (minCompare == 0 && MinIncluded == false))
            {
                return false;
            }
            Int32 maxCompare = sourceValue.CompareTo(MaxValue);
            if (maxCompare == 1 || (maxCompare == 0 && MaxIncluded == false))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to parse source to target type, returns false if the internal constraint is violated
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public Boolean TryParse(Source sourceValue, out Source targetValue)
        {
            Boolean isValid = IsValid(sourceValue);
            if (isValid == true)
            {
                targetValue = sourceValue;
            }
            else
            {
                targetValue = default(Source);
            }
            return isValid;
        }

        /// <summary>
        /// Overwrites the object ToString() and dispaly the value constraint
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return $"{(MinIncluded ? "[" : "(")}{MinValue.ToString()},{MaxValue.ToString()}{(MaxIncluded ? "]" : ")")})";
        }
    }
}
