using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to select the squared ensemble displacement for each particle from a <see cref="JobContext"/> sequence
    /// </summary>
    public class SquaredDisplacementEvaluation : EnsembleDisplacementEvaluation
    {
        /// <inheritdoc />
        public override bool IsSquared => true;

        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}"/> of <see cref="MobileTrackerResult"/>
        /// </summary>
        public IJobEvaluation<IReadOnlyList<MobileTrackerResult>> MobileTrackingEvaluation { get; set; }

        /// <inheritdoc />
        public SquaredDisplacementEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override void GetMovementVectors(Cartesian3D[] vectors, JobContext jobContext)
        {
            foreach (var trackingData in MobileTrackingEvaluation[jobContext.DataId])
            {
                vectors[trackingData.Particle.Index] += trackingData.Displacement.GetSquared();
            }
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            MobileTrackingEvaluation = MobileTrackingEvaluation ?? new MobileTrackingEvaluation(JobSet);
            if (!MobileTrackingEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("The mobile tracking evaluation is not compatible");

            base.PrepareForExecution();
        }
    }
}