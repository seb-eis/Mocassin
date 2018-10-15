using System;
using System.Collections.Generic;
using System.Linq;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// ICon shared struct extensions that provide generic extension methods specifically fro struct types
    /// </summary>
    public static class MocassinStructExtensions
    {
        /// <summary>
        /// Counts how many of the passed parameter values compare equal to the reference value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <param name="testValues"></param>
        /// <returns></returns>
        public static int CountMatches<T1>(this T1 value, IEqualityComparer<T1> comparer, params T1[] testValues) where T1 : struct
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            return testValues.Sum(item => (comparer.Equals(value, item) ? 1 : 0));
        }

        /// <summary>
        /// Counts how many values compare equal to the reference value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int CountMatches<T1>(this T1 value, IEqualityComparer<T1> comparer, IEnumerable<T1> values) where T1 : struct
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return values.Sum(item => (comparer.Equals(value, item) ? 1 : 0));
        }
    }
}
