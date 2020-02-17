using System;
using System.Collections.Generic;

namespace Mocassin.Framework.Collections
{
    /// <summary>
    ///     Contains factory methods to create special containers without too much generic argument cluttering
    /// </summary>
    public static class ContainerFactory
    {
        /// <summary>
        ///     Creates a new set equivalent list that uses the specified comparer (All entries are unique and sorted)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static SetList<T1> CreateSetList<T1>(IComparer<T1> comparer)
        {
            return new SetList<T1>(comparer);
        }

        /// <summary>
        ///     Creates new set list with the provided comparer that is filled with a sequence of initial values
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="comparer"></param>
        /// <param name="initialValues"></param>
        /// <returns></returns>
        public static SetList<T1> CreateSetList<T1>(IComparer<T1> comparer, IEnumerable<T1> initialValues)
        {
            if (initialValues == null) throw new ArgumentNullException(nameof(initialValues));
            return new SetList<T1>(comparer) {initialValues};
        }

        /// <summary>
        ///     Creates a new multiset equivalent list that uses the specified comparer (All entries are sorted but not unique)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static MultisetList<T1> CreateMultisetList<T1>(IComparer<T1> comparer)
        {
            return new MultisetList<T1>(comparer);
        }

        /// <summary>
        ///     Creates new multiset list with the provided comparer that is filled with a sequence of initial values
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="comparer"></param>
        /// <param name="initialValues"></param>
        /// <returns></returns>
        public static MultisetList<T1> CreateMultisetList<T1>(IComparer<T1> comparer, IEnumerable<T1> initialValues)
        {
            if (initialValues == null) throw new ArgumentNullException(nameof(initialValues));
            return new MultisetList<T1>(comparer) {initialValues};
        }
    }
}