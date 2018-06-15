using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Constraints
{
    /// <summary>
    /// General generic interface for all value contraints of an unrestricted source type struct to a restricted target type (target can be identical to source if no value object is used)
    /// </summary>
    public interface IValueConstraint<Source, Target> where Source : IComparable<Source>
    {

        /// <summary>
        /// The upper boundary value of the source type
        /// </summary>
        Source MaxValue { get; set; }

        /// <summary>
        /// The lower boundary value of the source type
        /// </summary>
        Source MinValue { get; set; }

        /// <summary>
        /// Indicates if the maximum boudnary value is included or not
        /// </summary>
        Boolean MinIncluded { get; set; }

        /// <summary>
        /// Indicates if the minimal boundary value is included or not
        /// </summary>
        Boolean MaxIncluded { get; set; }

        /// <summary>
        /// Parses source to target if the internal constraint is not violated (Returns false on violation)
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        Boolean TryParse(Source sourceValue, out Target targetValue);

        /// <summary>
        /// Validates if the given source object violates the internal contraint
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        Boolean IsValid(Source sourceValue);
    }
}
