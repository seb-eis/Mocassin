using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Framework.Constraints;
using Mocassin.Framework.Messaging;
using Mocassin.Model.Basic;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Defines a serializable value restriction setting with minimal, maximal and warning limits for validation purposes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class ValueSetting<T> where T : IComparable<T>
    {
        /// <summary>
        ///     The value display name in messages
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        ///     The min allowed value. Value is always included
        /// </summary>
        [DataMember]
        public T MinValue { get; set; }

        /// <summary>
        ///     The max allowed value. Value is always included
        /// </summary>
        [DataMember]
        public T MaxValue { get; set; }

        /// <summary>
        ///     The upper warning limit value. Values larger than this limit can be problematic
        /// </summary>
        [DataMember]
        public T UpperWarningLimit { get; set; }

        /// <summary>
        ///     The lower warning limit value. Values lesser than this limit can be problematic
        /// </summary>
        [DataMember]
        public T LowerWarningLimit { get; set; }

        /// <summary>
        ///     Creates new value restriction with display name and all four restricted values
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="upperWarningLimit"></param>
        /// <param name="lowerWarningLimit"></param>
        public ValueSetting(string displayName, T minValue, T lowerWarningLimit, T upperWarningLimit, T maxValue)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            SetValues(new[] {minValue, maxValue, upperWarningLimit, lowerWarningLimit});
        }

        /// <summary>
        ///     Creates new value restriction with only min and max value set. Warning values are set to the same values
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public ValueSetting(string displayName, T minValue, T maxValue)
            : this(displayName, minValue, minValue, maxValue, maxValue)
        {
        }

        /// <summary>
        ///     Default construct new value setting
        /// </summary>
        public ValueSetting()
        {
        }

        /// <summary>
        ///     Get the minimal and maximal value as a two value tuple
        /// </summary>
        /// <returns></returns>
        public (T Min, T Max) GetMinMaxTuple() => (MinValue, MaxValue);

        /// <summary>
        ///     Parses the value and checks for limitation violations and supplies affiliated warning messages. Returned integer
        ///     indicates level of violation
        /// </summary>
        /// <param name="value"></param>
        /// <param name="warnings"></param>
        /// <returns></returns>
        public int ParseValue(T value, out IEnumerable<WarningMessage> warnings)
        {
            var parseResult = ParseValue(value);
            var warningList = new List<WarningMessage>(2);

            if (Math.Abs(parseResult) == 1)
            {
                var detail0 =
                    $"The [{DisplayName}] value ({value}) is outside of the warning boundaries [{LowerWarningLimit},{UpperWarningLimit}]";
                const string detail1 = "Option 1: Value is just unphysical or unusual. Simulation may yield unexpected or questionable results.";
                const string detail2 = "Option 2: Value is performance related. Performance hits and long computations times should be expected.";
                const string detail3 = "Option 3: Value is memory related. Memory related problems (Overflow, out-of-memory, ...) should be expected.";
                warningList.Add(ModelMessageSource.CreateWarningLimitReachedWarning(this, detail0, detail1, detail2, detail3));
            }

            if (Math.Abs(parseResult) == 2)
            {
                var detail =
                    $"The [{DisplayName}] value ({value}) is outside of the restriction boundaries [{LowerWarningLimit},{UpperWarningLimit}]";
                warningList.Add(ModelMessageSource.CreateRestrictionViolationWarning(this, detail));
            }

            warnings = warningList;
            return parseResult;
        }

        /// <summary>
        ///     Parses the value and returns 0 if the value is valid. Returns +/- 1 if the warning limit is reached, returns +/- 2
        ///     if the limitation is violated
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int ParseValue(T value)
        {
            var lowerParse = ParseLower(value);
            return lowerParse == 0
                ? ParseUpper(value)
                : lowerParse;
        }

        /// <summary>
        ///     Creates a <see cref="IValueConstraint{TSource,TTarget}" /> of matching type from the setting
        /// </summary>
        /// <returns></returns>
        public IValueConstraint<T, T> ToConstraint() => new ValueConstraint<T>(true, MinValue, MaxValue, true);

        /// <summary>
        ///     Parses the value for violation of upper warning limit or restriction value. Returns 0, 1 or 2 depending on the
        ///     violation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected int ParseUpper(T value)
        {
            if (value.CompareTo(UpperWarningLimit) <= 0)
                return 0;

            return value.CompareTo(MaxValue) > 0 ? 2 : 1;
        }

        /// <summary>
        ///     Parses the value for violation of lower warning limit or restriction value. Returns 0, -1 or -2 depending on the
        ///     violation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected int ParseLower(T value)
        {
            if (value.CompareTo(LowerWarningLimit) >= 0)
                return 0;

            return value.CompareTo(MinValue) < 0 ? -2 : -1;
        }

        /// <summary>
        ///     Sorts a set of 4 so they form a valid value restriction
        /// </summary>
        /// <param name="values"></param>
        protected void SetValues(T[] values)
        {
            if (values.Length != 4)
                throw new ArgumentException("Wrong number of values for value restriction");

            Array.Sort(values);
            MinValue = values[0];
            LowerWarningLimit = values[1];
            UpperWarningLimit = values[2];
            MaxValue = values[3];
        }
    }
}