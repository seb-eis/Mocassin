using System.Text.RegularExpressions;

namespace Mocassin.Framework.Constraints
{
    /// <summary>
    ///     A regular expression based string constraint that limits characters, length and formatting of a string
    /// </summary>
    public class RegexStringConstraint : IObjectConstraint<string, string>
    {
        /// <summary>
        ///     Get the constraint <see cref="Regex" />
        /// </summary>
        public Regex Regex { get; }

        /// <summary>
        ///     Creates new regex based string constraint with the given regex
        /// </summary>
        /// <param name="regex"></param>
        public RegexStringConstraint(Regex regex)
        {
            Regex = regex;
        }

        /// <summary>
        ///     Check if the internal regex is a match for the source string
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public bool IsValid(string sourceValue)
        {
            return Regex.IsMatch(sourceValue);
        }

        /// <summary>
        ///     Parses string to out string if internal regex is not violated, returns false on violation
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public bool TryParse(string sourceValue, out string targetValue)
        {
            var isValid = IsValid(sourceValue);
            targetValue = isValid ? sourceValue : null;
            return isValid;
        }
    }
}