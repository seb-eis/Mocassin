using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mocassin.Framework.Extensions
{
    /// <summary>
    ///     ICon string builder extension class that provides specific enumerator converters for compatibility with the C++
    ///     XMLNode
    ///     implementation
    /// </summary>
    public static class MocassinStringBuilderExtensions
    {
        /// <summary>
        ///     Appends a comma separated value list to the builder based upon an enumerator and the ToString() method
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="builder"></param>
        /// <param name="enumerator"></param>
        public static void AppendCommaSeparatedValueList<TValue>(this StringBuilder builder, IEnumerable<TValue> enumerator)
        {
            foreach (var item in enumerator) builder.Append(item + ",");
            builder.PopBack(1);
        }

        /// <summary>
        ///     Appends multiple values by ToString call and a separator
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="builder"></param>
        /// <param name="values"></param>
        /// <param name="separator"></param>
        public static void AppendSeparatedToString<T1>(this StringBuilder builder, IEnumerable<T1> values, char separator)
        {
            foreach (var item in values)
            {
                builder.Append(item);
                builder.Append(separator);
            }

            builder.PopBack(1);
        }

        /// <summary>
        ///     Creates comma separated value string from parameter list
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="builder"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string BuildCommaSeparatedValueString<TValue>(this StringBuilder builder, params TValue[] values)
        {
            builder.Clear();
            builder.AppendCommaSeparatedValueList(values);
            return builder.ToString();
        }

        /// <summary>
        ///     Appends a set of convertibles as comma separated invariant strings
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="separator"></param>
        /// <param name="convertibles"></param>
        public static void AppendSeparated(this StringBuilder builder, char separator, IEnumerable<IConvertible> convertibles)
        {
            foreach (var item in convertibles)
                builder.Append(item.PrimitiveToString() + separator);

            builder.PopBack(1);
        }

        /// <summary>
        ///     Removes all consecutive line break duplicates
        /// </summary>
        /// <param name="builder"></param>
        public static void RemoveConsecutiveNewLines(this StringBuilder builder)
        {
            var result = Regex.Replace(builder.ToString(), @"(?:\r\n|\r(?!\n)|(?<!\r)\n){2,}", Environment.NewLine);
            builder.Clear();
            builder.Append(result);
        }

        /// <summary>
        ///     Removes the specified number of characters from the end of the current builder content
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="count"></param>
        public static void PopBack(this StringBuilder builder, int count)
        {
            if (builder.Length == 0)
                return;

            builder.Remove(builder.Length - count, count);
        }

        /// <summary>
        ///     Removes the specified number of characters from the end of the builder content and replaces them with a new end
        ///     string
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