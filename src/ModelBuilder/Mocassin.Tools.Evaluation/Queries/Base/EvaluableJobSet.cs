using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Tools.Evaluation.Queries;

namespace Mocassin.Tools.Evaluation.Context
{
    /// <summary>
    ///     Represents an read only collection of <see cref="JobContexts" /> that can be used with job contexts queries
    /// </summary>
    public class EvaluableJobSet : IEvaluableJobSet
    {
        /// <summary>
        ///     The wrapped list of <see cref="JobContext" /> instances
        /// </summary>
        private IList<JobContext> JobContexts { get; }

        /// <inheritdoc />
        public MslEvaluationContext EvaluationContext { get; }

        /// <inheritdoc />
        public int Count => JobContexts.Count;

        /// <inheritdoc />
        public JobContext this[int index] => JobContexts[index];

        /// <summary>
        ///     Creates an <see cref="EvaluableJobSet" /> from a <see cref="IList{T}" /> of <see cref="JobContext" /> instances
        /// </summary>
        /// <param name="jobContexts"></param>
        /// <exception cref="ArgumentException"></exception>
        public EvaluableJobSet(IList<JobContext> jobContexts)
        {
            JobContexts = jobContexts ?? throw new ArgumentNullException(nameof(jobContexts));
            EvaluationContext = JobContexts[0].EvaluationContext;
            if (!JobContexts.All(x => ReferenceEquals(x.EvaluationContext, EvaluationContext)))
                throw new ArgumentException("Job context set cannot have more than one evaluation context.");
        }

        /// <inheritdoc />
        public EvaluableJobSet(IEnumerable<JobContext> jobContexts)
            : this(jobContexts?.ToList())
        {
        }

        /// <inheritdoc />
        public bool CompatibleTo(IEvaluableJobSet other) => ReferenceEquals(EvaluationContext, other?.EvaluationContext);

        /// <inheritdoc />
        public IEnumerator<JobContext> GetEnumerator() => JobContexts.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) JobContexts).GetEnumerator();
    }
}