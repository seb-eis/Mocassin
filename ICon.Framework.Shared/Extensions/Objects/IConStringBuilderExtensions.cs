using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// ICon string builder extension class that provides specific enumerator convertes for compatability with the C++ XMLNode implementation
    /// </summary>
    public static class IConStringBuilderExtensions
    {
        /// <summary>
        /// Appends a comma sperated value list to the builder based upon an enumerator and the ToString() method
        /// </summary>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="builder"></param>
        /// <param name="enumerator"></param>
        public static void AppendCommaSeparatedValueList<ValueType>(this StringBuilder builder, IEnumerable<ValueType> enumerator)
        {
            foreach (ValueType item in enumerator)
            {
                builder.Append(item.ToString() + ",");
            }
            builder.PopBack(1);
        }

        /// <summary>
        /// Appends multiple values by ToString call and a separator
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="builder"></param>
        /// <param name="values"></param>
        /// <param name="separator"></param>
        public static void AppendSeparatedToString<T1>(this StringBuilder builder, IEnumerable<T1> values, char separator)
        {
            foreach (var item in values)
            {
                builder.Append(item.ToString());
                builder.Append(separator);
            }
            builder.PopBack(1);
        }

        /// <summary>
        /// Creates comma separated value string from parameter list
        /// </summary>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="builder"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static String BuildCommaSeparatedValueString<ValueType>(this StringBuilder builder, params ValueType[] values)
        {
            builder.Clear();
            builder.AppendCommaSeparatedValueList(values);
            return builder.ToString();
        }

        /// <summary>
        /// Appends a set of convertibles as comma separted invariant strings
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="convertibles"></param>
        public static void AppendSeparated(this StringBuilder builder, char separator, IEnumerable<IConvertible> convertibles)
        {
            foreach (IConvertible item in convertibles)
            {
                builder.Append(item.PrimitiveToString() + separator);
            }
            builder.PopBack(1);
        }

        /// <summary>
        /// Removes all consecutive line break dublicates
        /// </summary>
        /// <param name="builder"></param>
        public static void RemoveConsecutiveNewLines(this StringBuilder builder)
        {
            var result = Regex.Replace(builder.ToString(), @"(?:\r\n|\r(?!\n)|(?<!\r)\n){2,}", Environment.NewLine);
            builder.Clear();
            builder.Append(result);
        }

        /// <summary>
        /// Removes the specified number of characters from the end of the current builder content
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="count"></param>
        public static void PopBack(this StringBuilder builder, int count)
        {
            if (builder.Length == 0)
            {
                return;
            }
            builder.Remove(builder.Length - count, count);
        }

        /// <summary>
        /// Removes the specified number of characters from the end of the builder content and replaces them with a new end string
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="count"></param>
        /// <param name="newEnd"></param>
        public static void PopBack(this StringBuilder builder, int count, string newEnd)
        {
            builder.PopBack(count);
            builder.Append(newEnd);
        }
    }
}
