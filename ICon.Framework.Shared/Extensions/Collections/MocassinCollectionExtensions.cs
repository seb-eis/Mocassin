using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mocassin.Framework.Extensions
{
    /// <summary>
    /// Extension class for general extension collections
    /// </summary>
    public static class MocassinCollectionExtensions
    {
        /// <summary>
        /// Uniformly select a subset of entries from a collection using the provided random number generator
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="collection"></param>
        /// <param name="count"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static IEnumerable<T1> SelectRandom<T1>(this ICollection<T1> collection, int count, System.Random random)
        {
            if (count >= collection.Count)
            {
                foreach (var item in collection)
                {
                    yield return item;
                }

                yield break;
            }

            int passed = -1, used = 0;
            foreach (var item in collection)
            {
                if (used == count)
                {
                    yield break;
                }

                if (!((count - used) > (collection.Count - ++passed) * random.NextDouble()))
                    continue;

                ++used;
                yield return item;
            }
        }

        /// <summary>
        /// Project each element of a uniformly selected subset of specified size from a generic collection into a new form
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="collection"></param>
        /// <param name="count"></param>
        /// <param name="random"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T2> SelectRandom<T1, T2>(this ICollection<T1> collection, int count, System.Random random, Func<T1, T2> selector)
        {
            return collection.SelectRandom(count, random).Select(selector);
        }

        /// <summary>
        /// Adds multiple values to a generic collection
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="collection"></param>
        /// <param name="values"></param>
        public static void AddRange<T1>(this ICollection<T1> collection, IEnumerable<T1> values)
        {
            foreach (var item in values)
            {
                collection.Add(item);
            }
        }

        public static IEnumerable<int> SelectRandomIndex<T1>(this ICollection<T1> collection, int count, System.Random random)
        {
            if (count >= collection.Count)
            {
                for(int i = 0; i < collection.Count; i++)
                {
                    yield return i;
                }

                yield break;
            }

            int passed = -1, used = 0;
            for (int i = 0; i < collection.Count; i++)
            {
                if (used == count)
                {
                    yield break;
                }
                if ((count - used) > (collection.Count - ++passed) * random.NextDouble())
                {
                    ++used;
                    yield return i;
                }
            }
        }
    }
}
