using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Collections;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// Contains extensions methods for comparer objects and IComparer interfaces
    /// </summary>
    public static class MocassinComparerExtensions
    {
        /// <summary>
        /// Wraps an IComparer interface into a delegate based equality comparer using the (first.CompareTo(second) == 0) formalism
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEqualityComparer<T1> ToEqualityComparer<T1>(this IComparer<T1> comparer)
        {
            return new CompareAdapter<T1>(comparer.Compare);
        }
    }
}
