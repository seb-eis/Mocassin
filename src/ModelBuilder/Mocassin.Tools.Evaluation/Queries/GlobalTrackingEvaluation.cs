﻿using System;
using System.Collections.Generic;
using Mocassin.Framework.Collections.Mocassin.Tools.Evaluation.Queries;
using Mocassin.Framework.Extensions;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the <see cref="GlobalTrackerResult" /> list from a sequence of <see cref="JobContext" />
    ///     instances
    /// </summary>
    public class GlobalTrackingEvaluation : JobEvaluation<ReadOnlyList<GlobalTrackerResult>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that provides the particle counts
        /// </summary>
        public ParticleCountEvaluation ParticleCountEvaluation { get; set; }

        /// <inheritdoc />
        public GlobalTrackingEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override ReadOnlyList<GlobalTrackerResult> GetValue(JobContext context)
        {
            var globalTrackerModels = context.SimulationModel.SimulationTrackingModel.GlobalTrackerModels;
            var trackingData = context.McsReader.ReadGlobalTrackers();
            var result = new List<GlobalTrackerResult>(globalTrackerModels.Count);
            var vectorTransformer = context.ModelContext.GetUnitCellVectorEncoder().Transformer;


            foreach (var trackerModel in globalTrackerModels)
            {
                var shift = vectorTransformer.ToCartesian(trackingData[trackerModel.ModelId].AsVector());
                var particleCount = ParticleCountEvaluation[context.DataId][trackerModel.TrackedParticle.Index];
                if (particleCount == 0) continue;

                var displacement = new EnsembleDisplacement(false, false, particleCount, trackerModel.TrackedParticle, shift.GetLength() * UnitConversions.Length.AngstromToMeter,
                    shift * UnitConversions.Length.AngstromToMeter);
                result.Add(new GlobalTrackerResult(trackerModel, displacement));
            }

            return result.AsReadOnlyList();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            if (ParticleCountEvaluation == null) ParticleCountEvaluation = new ParticleCountEvaluation(JobSet);
            if (!ParticleCountEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Particle count evaluation does not have the same data source.");
        }
    }
}