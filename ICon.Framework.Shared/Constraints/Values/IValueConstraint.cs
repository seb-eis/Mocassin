using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Framework.Constraints
{
    /// <summary>
    /// Generic value constraint of an unrestricted source type struct to a restricted target type (target can be identical to source if no value object is used)
    /// </summary>
    public interface IValueConstraint<TSource, TTarget> where TSource : IComparable<TSource>
    {

        /// <summary>
        /// The upper boundary value of the source type
        /// </summary>
        TSource MaxValue { get; set; }

        /// <summary>
        /// The lower boundary value of the source type
        /// </summary>
        TSource MinValue { get; set; }

        /// <summary>
        /// Indicates if the maximum boundary value is included or not
        /// </summary>
        bool MinIsIncluded { get; set; }

        /// <summary>
        /// Indicates if the minimal boundary value is included or not
        /// </summary>
        bool MaxIsIncluded { get; set; }

        /// <summary>
        /// Parses source to target if the internal constraint is not violated (Returns false on violation)
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        bool TryParse(TSource sourceValue, out TTarget targetValue);

        /// <summary>
        /// Validates if the given source object violates the internal constraint
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        bool IsValid(TSource sourceValue);
    }
}
