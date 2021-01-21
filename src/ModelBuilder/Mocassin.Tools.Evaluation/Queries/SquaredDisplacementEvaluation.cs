using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to select the squared ensemble displacement for each particle from a <see cref="JobContext" /> sequence
    /// </summary>
    public class SquaredDisplacementEvaluation : EnsembleDisplacementEvaluation
    {
        /// <inheritdoc />
        public override bool IsSquared => true;

        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> of <see cref="MobileTrackerResult" />
        /// </summary>
        public IJobEvaluation<IReadOnlyList<MobileTrackerResult>> MobileTrackingEvaluation { get; set; }

        /// <inheritdoc />
        public SquaredDisplacementEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override void PopulateRawDisplacementData(Cartesian3D[] vectors, double[] displacements, JobContext jobContext)
        {
            foreach (var trackingData in MobileTrackingEvaluation[jobContext.DataId])
            {
                var length = trackingData.Displacement.GetLength();
                var vector = trackingData.Displacement.GetSquared();
                vectors[trackingData.Particle.Index] += vector;
                displacements[trackingData.Particle.Index] += length * length;
            }
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            MobileTrackingEvaluation ??= new MobileTrackingEvaluation(JobSet);
            if (!MobileTrackingEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("The mobile tracking evaluation is not compatible");

            base.PrepareForExecution();
        }
    }
}