using System.Collections.Generic;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Represents a collection of <see cref="JobContext" /> instances that can be evaluated using
    ///     <see cref="JobEvaluation{T}" /> implementations
    /// </summary>
    public interface IEvaluableJobCollection : IReadOnlyList<JobContext>
    {
        /// <summary>
        ///     Get the <see cref="MslEvaluationContext" /> of the queryable
        /// </summary>
        /// <returns></returns>
        MslEvaluationContext EvaluationContext { get; }

        /// <summary>
        ///     Checks if the <see cref="IEvaluableJobCollection"/> is compatible
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool CompatibleTo(IEvaluableJobCollection other);
    }
}