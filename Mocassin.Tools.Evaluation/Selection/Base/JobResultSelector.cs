using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Tools.Evaluation.Selection
{
    /// <summary>
    ///     Generic base class for implementations of job data selectors that extract data from <see cref="JobResultContext" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JobResultSelector<T>
    {
        /// <summary>
        ///     Get the target value for the passed <see cref="JobResultContext" />
        /// </summary>
        /// <param name="resultContext"></param>
        /// <returns></returns>
        public abstract T GetValue(JobResultContext resultContext);

        /// <summary>
        ///     Get a <see cref="IDictionary{TKey,TValue}"/> that maps the result to the affiliated <see cref="JobResultContext"/>
        /// </summary>
        /// <param name="jobResultContexts"></param>
        /// <returns></returns>
        public virtual IDictionary<JobResultContext, T> MapResults(IEnumerable<JobResultContext> jobResultContexts)
        {
            return jobResultContexts.ToDictionary(x => x, GetValue);
        }

        /// <summary>
        ///     Provide the <see cref="JobResultSelector{T}" /> as a delegate
        /// </summary>
        /// <returns></returns>
        public Func<JobResultContext, T> AsDelegate()
        {
            return GetValue;
        }
    }
}