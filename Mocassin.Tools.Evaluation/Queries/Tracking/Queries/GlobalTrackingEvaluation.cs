using System;
using System.Collections.Generic;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the <see cref="GlobalTrackerResult" /> list from a sequence of <see cref="JobContext" />
    ///     instances
    /// </summary>
    public class GlobalTrackingEvaluation : JobEvaluation<IReadOnlyList<GlobalTrackerResult>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that provides the particle counts
        /// </summary>
        public IJobEvaluation<IReadOnlyList<int>> ParticleCountEvaluation { get; set; }

        /// <inheritdoc />
        public GlobalTrackingEvaluation(IEvaluableJobCollection jobCollection)
            : base(jobCollection)
        {
        }

        /// <inheritdoc />
        protected override IReadOnlyList<GlobalTrackerResult> GetValue(JobContext context)
        {
            var globalTrackerModels = context.SimulationModel.SimulationTrackingModel.GlobalTrackerModels;
            var trackingData = context.McsReader.GetGlobalTrackers();
            var result = new List<GlobalTrackerResult>(globalTrackerModels.Count);
            var vectorTransformer = context.ModelContext.GetUnitCellVectorEncoder().Transformer;


            foreach (var trackerModel in globalTrackerModels)
            {
                var shift = vectorTransformer.ToCartesian(trackingData[trackerModel.ModelId]);
                var particleCount = ParticleCountEvaluation[context.DataId][trackerModel.TrackedParticle.Index];
                var displacement = new EnsembleDisplacement(false, particleCount, trackerModel.TrackedParticle,
                    shift * UnitConversions.Length.AngToMeter);
                result.Add(new GlobalTrackerResult(trackerModel, displacement));
            }

            return result.AsReadOnly();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            if (ParticleCountEvaluation == null) ParticleCountEvaluation = new ParticleCountEvaluation(JobCollection);
            if (!ParticleCountEvaluation.JobCollection.CompatibleTo(JobCollection))
                throw new InvalidOperationException("Particle count evaluation does not have the same data source.");
        }
    }
}