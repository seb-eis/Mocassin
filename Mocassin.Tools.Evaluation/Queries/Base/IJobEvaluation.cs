using System.Collections.Generic;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Represents an evaluation of a <see cref="IEvaluableJobSet" /> that can provide resuluts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IJobEvaluation<out T> : IReadOnlyList<T>
    {
        /// <summary>
        ///     Get the <see cref="IEvaluableJobSet" /> that serves as the data source
        /// </summary>
        IEvaluableJobSet JobSet { get; }

        /// <summary>
        ///     Get the <see cref="IReadOnlyList{T}" /> of results
        /// </summary>
        IReadOnlyList<T> Result { get; }
    }
}