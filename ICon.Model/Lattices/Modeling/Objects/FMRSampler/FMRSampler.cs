using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Get samples from a list
    /// </summary>
    /// <remarks>
    /// Algorithm is by C. T. Fan, M. E. Muller, I. Rezucha, J. Amer. Stat. Assoc. 57 (1962), 387-402
    /// Discussed in D. E. Knuth "The Art of Computer Programming Vol. 2 - Seminumerical Algorithms" (1997) 142-143
    /// </remarks>
    public class FMRSampler<T1> where T1 : class
    {
        /// <summary>
        /// Get samples from a list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="samplingSize"></param>
        /// <returns></returns>
        public List<T1> GetSamples(IList<T1> list, uint samplingSize)
        {
            int passedCount = 0;
            Random random = new Random();
            List<T1> sampling = new List<T1>();

            foreach (var item in list)  
            {
                if (Convert.ToDouble(samplingSize - sampling.Count) <= Convert.ToDouble(list.Count - passedCount) * random.NextDouble())
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
        /// Apply an action to samples from a list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="samplingSize"></param>
        /// <param name="action"></param>
        public void ApplyToSamples(IList<T1> list, uint samplingSize, Action<T1> action)
        {
            IList<T1> samples = GetSamples(list, samplingSize);

            foreach (T1 item in samples)
            {
                action(item);
            }
        }
    }
}
