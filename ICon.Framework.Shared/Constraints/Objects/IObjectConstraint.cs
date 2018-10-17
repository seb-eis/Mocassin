using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Framework.Constraints
{
    /// <summary>
    /// General generic interface for all object contraints of an unrestricted source type to a restricted target type (target can be identical to source if no value object is used)
    /// </summary>
    public interface IObjectConstraint<Source, Target> where Source : class where Target : class
    {
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
