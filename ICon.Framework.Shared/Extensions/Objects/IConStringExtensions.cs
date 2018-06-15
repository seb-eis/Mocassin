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
    }
}
