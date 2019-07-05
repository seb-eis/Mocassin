﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Tools.Evaluation.Queries;

namespace Mocassin.Tools.Evaluation.Context
{
    /// <summary>
    ///     Represents an read only collection of <see cref="JobContexts" /> that can be used with job contexts queries
    /// </summary>
    public class EvaluableJobCollection : IEvaluableJobCollection
    {
        /// <summary>
        ///     The wrapped list of <see cref="JobContext" /> instances
        /// </summary>
        private IList<JobContext> JobContexts { get; }

        /// <inheritdoc />
        public MslEvaluationContext EvaluationContext { get; }

        /// <inheritdoc />
        public bool CompatibleTo(IEvaluableJobCollection other)
        {
            return ReferenceEquals(EvaluationContext, other?.EvaluationContext);
        }

        /// <inheritdoc />
        public IEnumerator<JobContext> GetEnumerator()
        {
            return JobContexts.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) JobContexts).GetEnumerator();
        }

        /// <inheritdoc />
        public int Count => JobContexts.Count;

        /// <inheritdoc />
        public JobContext this[int index] => JobContexts[index];

        public EvaluableJobCollection(IList<JobContext> jobContexts)
        {
            JobContexts = jobContexts ?? throw new ArgumentNullException(nameof(jobContexts));
            EvaluationContext = JobContexts[0].EvaluationContext;
            if (!JobContexts.All(x => ReferenceEquals(x.EvaluationContext, EvaluationContext)))
                throw new ArgumentException("Job context set cannot have more than one evaluation context.");
        }

        public EvaluableJobCollection(IEnumerable<JobContext> jobContexts)
            : this(jobContexts?.ToList())
        {
        }
    }
}