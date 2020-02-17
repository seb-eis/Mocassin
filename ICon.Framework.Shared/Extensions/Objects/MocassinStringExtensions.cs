using System;
using System.Collections.Generic;

namespace Mocassin.Framework.Extensions
{
    /// <summary>
    ///     ICon extension class for strings
    /// </summary>
    public static class MocassinStringExtensions
    {
        /// <summary>
        ///     Splits a string by the given delimiter and sets the result if the specified entry count is matched
        /// </summary>
        /// <param name="literal"></param>
        /// <param name="entryCount"></param>
        /// <param name="delimiter"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TrySplitToSpecificSubstringCount(this string literal, int entryCount, char delimiter, out string[] result)
        {
            var split = literal.Split(delimiter);
            if (split.Length != entryCount)
            {
                result = null;
                return false;
            }

            result = split;
            return true;
        }

        /// <summary>
        ///     Parse a character separated string into the actual values using the provided converter delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separated"></param>
        /// <param name="converter"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<T> ParseToValueList<T>(this string separated, Func<string, T> converter, char separator)
        {
            int index = 0, numOfValues = 1;
            var workSpan = separated.AsSpan();

            foreach (var value in workSpan)
                if (value == separator)
                    numOfValues++;

            var results = new List<T>(numOfValues);

            while (index != workSpan.Length + 1)
            {
                workSpan = workSpan.Slice(index, workSpan.Length - index);
                index = workSpan.IndexOf(separator);
                if (index == -1) index = workSpan.Length;
                results.Add(converter(separated.Substring(separated.Length - workSpan.Length, index++)));
            }

            return results;
        }
    }
}