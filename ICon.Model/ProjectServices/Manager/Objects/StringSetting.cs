using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ICon.Framework.Messaging;
using ICon.Model.Basic;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    ///     Defines a string regex limitation pattern for settings objects
    /// </summary>
    [DataContract]
    public class StringSetting
    {
        /// <summary>
        ///     Boolean flag that indicates if the value can be null
        /// </summary>
        [DataMember]
        public bool IsNullAllowing { get; set; }

        /// <summary>
        ///     The regex pattern string used to create regex instances
        /// </summary>
        [DataMember]
        public string RegexPattern { get; set; }

        /// <summary>
        ///     The display name of the check value
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        ///     Create default pattern setting
        /// </summary>
        public StringSetting()
        {
        }

        /// <summary>
        ///     Create pattern setting from pattern string. Throws if the pattern is not a valid regex pattern or null
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="regexPattern"></param>
        /// <param name="isNullAllowing"></param>
        public StringSetting(string displayName, string regexPattern, bool isNullAllowing)
        {
            IsNullAllowing = isNullAllowing;
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            RegexPattern = regexPattern ?? throw new ArgumentNullException(nameof(regexPattern));
            Regex.IsMatch("", regexPattern); // Enforce exception on invalid regex pattern that is not null!
        }

        /// <summary>
        ///     Parse a value and check if it matches the settings limitations. Returns false on failed and sets the affiliated
        ///     warnings to the out parameter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="warnings"></param>
        /// <returns></returns>
        public bool ParseValue(string value, out IEnumerable<WarningMessage> warnings)
        {
            warnings = null;
            switch (value)
            {
                case null when IsNullAllowing:
                    return true;

                case null:
                    return false;
            }

            if (Regex.IsMatch(value, RegexPattern))
                return true;

            var messages = new List<WarningMessage>(2);
            var detail0 = $"The defined string value ({value}) for [{DisplayName}] violates its allowed regex pattern ({RegexPattern})";
            const string detail1 = "Option 1: Change your string to match the regex pattern. This is recommended.";
            const string detail2 = "Option 2: Change the regex pattern in the settings. This is not recommended.";
            const string detail3 = "Important: Invalid pattern definitions in the settings may crash the application!";
            messages.Add(ModelMessageSource.CreateNamingViolationWarning(this, detail0, detail1, detail2, detail3));
            warnings = messages;
            return false;
        }
    }
}