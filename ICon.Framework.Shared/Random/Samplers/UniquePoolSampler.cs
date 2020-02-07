using System;
using System.Collections.Generic;
using Mocassin.Framework.Extensions;

namespace ICon.Framework.Random
{
    /// <summary>
    ///     Selection helper for quick uniform subset selection from an item sequence
    /// </summary>
    /// <remarks>
    ///     Algorithm is by C. T. Fan, M. E. Muller, I. Rezucha, J. Amer. Stat. Assoc. 57 (1962), 387-402
    ///     Discussed in D. E. Knuth "The Art of Computer Programming Vol. 2 - Seminumerical Algorithms" (1997) 142-143
    /// </remarks>
    public class UniquePoolSampler<T1>
    {
        /// <summary>
        ///     Select a subset of defined size from a <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="samplingSize"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public List<T1> SelectSubset(IEnumerable<T1> pool, uint samplingSize, System.Random rng)
        {
            var poolCollection = pool.AsCollection();
            var passedCount = 0;
            var poolSize = poolCollection.Count;
            var subset = new List<T1>();
            foreach (var item in poolCollection)
            {
                if (Convert.ToDouble(samplingSize - subset.Count) <= Convert.ToDouble(poolSize - passedCount) * rng.NextDouble())
                {
                    passedCount++;
                    continue;
                }

                subset.Add(item);
                passedCount++;

                if (subset.Count >= samplingSize) return subset;
            }

            return subset;
        }
    }
}