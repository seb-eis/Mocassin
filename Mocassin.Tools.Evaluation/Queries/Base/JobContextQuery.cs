using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Tools.Evaluation.Selection
{
    /// <summary>
    ///     Generic base class for implementations of data queries against <see cref="JobContext" /> instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JobContextQuery<T> : IJobContextQuery<T>
    {
        private IList<T> results;

        /// <inheritdoc />
        public IEnumerable<JobContext> DataSource { get; }

        /// <inheritdoc />
        public IList<T> Results => results ?? (results = Execute(DataSource));

        /// <summary>
        ///     Creates a new <see cref="JobContextQuery{T}"/> for the passed data source
        /// </summary>
        /// <param name="jobContexts"></param>
        protected JobContextQuery(IEnumerable<JobContext> jobContexts)
        {
            DataSource = jobContexts ?? throw new ArgumentNullException(nameof(jobContexts));
        }

        /// <inheritdoc />
        public abstract T Execute(JobContext context);

        /// <inheritdoc />
        public virtual IList<T> Execute(IEnumerable<JobContext> jobContexts)
        {
            EnsureDataSourcesExist(jobContexts);
            return jobContexts.Select(Execute).ToList();
        }

        /// <inheritdoc />
        public T this[int index] => Results[index];

        /// <summary>
        ///     Ensures that potentially required additional data sources exist
        /// </summary>
        /// <param name="jobContexts"></param>
        public virtual void EnsureDataSourcesExist(IEnumerable<JobContext> jobContexts)
        {

        }

        /// <summary>
        ///     Provide the <see cref="JobContextQuery{T}" /> as a delegate
        /// </summary>
        /// <returns></returns>
        public Func<JobContext, T> AsSelector()
        {
            return Execute;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}