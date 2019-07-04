using System.Collections.Generic;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Tools.Evaluation.Selection
{
    /// <summary>
    ///     Shared interface for data queries against <see cref="JobContext" /> sets
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IJobContextQuery<T> : IEnumerable<T>
    {
        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="JobContext" /> that is used as the data source
        /// </summary>
        IEnumerable<JobContext> DataSource { get; }

        /// <summary>
        ///     Get the <see cref="IList{T}"/> of results
        /// </summary>
        IList<T> Results { get; }

        /// <summary>
        ///     Executes the query for a single <see cref="JobContext" />
        /// </summary>
        /// <param name="jobContext"></param>
        /// <returns></returns>
        T Execute(JobContext jobContext);

        /// <summary>
        ///     Invokes the query on the set data source and returns a <see cref="IList{T}"/> of the results
        /// </summary>
        /// <param name="jobContexts"></param>
        /// <returns></returns>
        IList<T> Execute(IEnumerable<JobContext> jobContexts);

        /// <summary>
        ///     Get the query result at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; }
    }
}