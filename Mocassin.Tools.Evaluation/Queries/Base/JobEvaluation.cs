using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Generic base class for implementations of data queries against <see cref="JobContext" /> instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JobEvaluation<T> : IJobEvaluation<T>
    {
        private IReadOnlyList<T> results;

        /// <inheritdoc />
        public IEvaluableJobCollection JobCollection { get; }

        /// <inheritdoc />
        public IReadOnlyList<T> Results => results ?? (results = Execute());

        /// <inheritdoc />
        public int Count => Results.Count;

        /// <inheritdoc />
        public T this[int index] => Results[index];

        /// <summary>
        ///     Creates a new <see cref="JobEvaluation{T}" /> for the passed <see cref="IEvaluableJobCollection" /> data source
        /// </summary>
        /// <param name="jobCollection"></param>
        protected JobEvaluation(IEvaluableJobCollection jobCollection)
        {
            JobCollection = jobCollection ?? throw new ArgumentNullException(nameof(jobCollection));
        }

        /// <summary>
        ///     Executes the query for a single <see cref="JobContext" /> without checking if the passed context is part of the
        ///     data source
        /// </summary>
        /// <param name="jobContext"></param>
        /// <returns></returns>
        protected abstract T GetValue(JobContext jobContext);

        /// <summary>
        ///     Executes the query and generates the results
        /// </summary>
        /// <returns></returns>
        protected virtual IReadOnlyList<T> Execute()
        {
            if (results != null) return results;
            PrepareForExecution();
            return JobCollection.Select(GetValue).ToList().AsReadOnly();
        }

        /// <summary>
        ///     Performs required pre execution actions e.g. getting unspecified dependent data sources
        /// </summary>
        protected virtual void PrepareForExecution()
        {
        }

        /// <summary>
        ///     Provide the <see cref="JobEvaluation{T}" /> as a delegate
        /// </summary>
        /// <returns></returns>
        public Func<JobContext, T> AsSelector()
        {
            return GetValue;
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