using System;
using System.Collections.Generic;
using System.Linq;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// ICon shared struct extensions that provide generic extension methods specifically fro struct types
    /// </summary>
    public static class IConStructExtensions
    {
        /// <summary>
        /// Counts how many of the passed parameter values compare equal to the reference value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <param name="testValues"></param>
        /// <returns></returns>
        public static Int32 CountMatches<T1>(this T1 value, IEqualityComparer<T1> comparer, params T1[] testValues) where T1 : struct
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            Int32 counter = 0;
            foreach (T1 item in testValues)
            {
                counter += comparer.Equals(value, item) ? 1 : 0;
            }
            return counter;
        }

        /// <summary>
        /// Counts how many values compare equal to the refernce value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Int32 CountMatches<T1>(this T1 value, IEqualityComparer<T1> comparer, IEnumerable<T1> values) where T1 : struct
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            Int32 counter = 0;
            foreach (T1 item in values)
            {
                counter += comparer.Equals(value, item) ? 1 : 0;
            }
            return counter;
        }
    }
}
