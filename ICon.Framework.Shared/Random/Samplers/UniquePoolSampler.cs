using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICon.Framework.Random
{
    /// <summary>
    /// Get samples from a list
    /// </summary>
    /// <remarks>
    /// Algorithm is by C. T. Fan, M. E. Muller, I. Rezucha, J. Amer. Stat. Assoc. 57 (1962), 387-402
    /// Discussed in D. E. Knuth "The Art of Computer Programming Vol. 2 - Seminumerical Algorithms" (1997) 142-143
    /// </remarks>
    public class UniquePoolSampler<T1>
    {
        /// <summary>
        /// Get samples from a pool
        /// </summary>
        /// <param name="list"></param>
        /// <param name="samplingSize"></param>
        /// <returns></returns>
        public List<T1> GetSamples(IEnumerable<T1> pool, uint samplingSize)
        {

            var random = new System.Random();

            int passedCount = 0;
            int poolCount = pool.Count();

            List<T1> sampling = new List<T1>();

            foreach (var item in pool)  
            {
                if (Convert.ToDouble(samplingSize - sampling.Count) <= Convert.ToDouble(poolCount - passedCount) * random.NextDouble())
                {
                    passedCount++;
                    continue;
                }

                sampling.Add(item);
                passedCount++;
                
                if (sampling.Count >= samplingSize)
                {
                    return sampling;
                }
            }

            return sampling;
        }

        /// <summary>
        /// Apply an action to samples from a pool
        /// </summary>
        /// <param name="list"></param>
        /// <param name="samplingSize"></param>
        /// <param name="action"></param>
        public void ApplyToSamples(IEnumerable<T1> list, uint samplingSize, Action<T1> action)
        {
            List<T1> samples = GetSamples(list, samplingSize);

            foreach (T1 item in samples)
            {
                action(item);
            }
        }

        /// <summary>
        /// Apply an action to samples from a pool
        /// </summary>
        /// <param name="list"></param>
        /// <param name="samplingSize"></param>
        /// <param name="action"></param>
        public List<T1> ApplyToSamplesAndReturn(IEnumerable<T1> list, uint samplingSize, Func<T1,T1> function)
        {
            List<T1> samples = GetSamples(list, samplingSize);

            List<T1> updatedSamples = new List<T1>();

            foreach (T1 item in samples)
            {
                updatedSamples.Add(function(item));
            }

            return updatedSamples;
        }
    }
}
