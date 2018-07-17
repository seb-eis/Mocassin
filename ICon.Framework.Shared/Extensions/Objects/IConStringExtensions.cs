using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// ICon extension class for strings
    /// </summary>
    public static class IConStringExtensions
    {
        /// <summary>
        /// Splits a string by the given delimiter and sets the result if the specified entry count is matched
        /// </summary>
        /// <param name="literal"></param>
        /// <param name="entryCount"></param>
        /// <param name="delimiter"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Boolean TrySplitToSpecificSubstringCount(this String literal, Int32 entryCount, Char delimiter, out String[] result)
        {
            String[] splitted = literal.Split(delimiter);
            if (splitted.Length != entryCount)
            {
                result = null;
                return false;
            }
            result = splitted;
            return true;
        }

        /// <summary>
        /// Parse a character seprarated string of values into the actual values using the provided converter delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separated"></param>
        /// <param name="separator"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static List<T> ParseToValueList<T>(this string separated, char separator, Func<string,T> converter)
        {
            int index = 0, numOfValues = 1;
            var workSpan = separated.AsSpan();

            for (int i = 0; i < workSpan.Length; i++)
            {
                if (workSpan[i] == separator)
                {
                    numOfValues++;
                }
            }
            var results = new List<T>(numOfValues);

            while (index != workSpan.Length + 1)
            {
                workSpan = workSpan.Slice(index, workSpan.Length - index);
                index = workSpan.IndexOf(separator);
                if (index == -1)
                {
                    index = workSpan.Length;
                }
                results.Add(converter(separated.Substring(separated.Length - workSpan.Length, index++)));

            }
            return results;
        }
    }
}
