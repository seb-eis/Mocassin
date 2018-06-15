using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICon.Framework.Constraints
{
    /// <summary>
    /// A regular expression based string constraint that limits characters, length and formatting of a string
    /// </summary>
    public class RegexStringConstraint : IObjectConstraint<String, String>
    {
        /// <summary>
        /// Creates new regex based string constraint with the given regex
        /// </summary>
        /// <param name="constraint"></param>
        public RegexStringConstraint(Regex constraint)
        {
            Constraint = constraint;
        }

        public Regex Constraint { get; protected set; }

        /// <summary>
        /// Check if the internal regex is a match for the source string
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public Boolean IsValid(String sourceValue)
        {
            return Constraint.IsMatch(sourceValue);
        }

        /// <summary>
        /// Parses string to out string if internal regex is not violated, returns false on violation
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public Boolean TryParse(String sourceValue, out String targetValue)
        {
            Boolean isValid = IsValid(sourceValue);
            if (isValid == false)
            {
                targetValue = null;
            }
            targetValue = sourceValue;
            return isValid;
        }
    }
}
