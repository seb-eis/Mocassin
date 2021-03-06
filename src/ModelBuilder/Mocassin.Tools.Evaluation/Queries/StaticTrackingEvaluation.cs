﻿using System.Collections.Generic;
using Mocassin.Framework.Collections.Mocassin.Tools.Evaluation.Queries;
using Mocassin.Framework.Extensions;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.Evaluation.Queries.Data;
using Mocassin.UI.Data.Helper;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the <see cref="StaticTrackerResult" /> set from a <see cref="IEvaluableJobSet" />
    /// </summary>
    public class StaticTrackingEvaluation : JobEvaluation<ReadOnlyList<StaticTrackerResult>>
    {
        /// <inheritdoc />
        public StaticTrackingEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override ReadOnlyList<StaticTrackerResult> GetValue(JobContext jobContext)
        {
            var trackerModels = jobContext.SimulationModel.SimulationTrackingModel.StaticTrackerModels;
            var trackingData = jobContext.McsReader.ReadStaticTrackers();
            var result = new List<StaticTrackerResult>(trackingData.Length);
            var vectorEncoder = jobContext.ModelContext.GetUnitCellVectorEncoder();
            var metaData = jobContext.McsReader.ReadMetaData();

            var positionIndexOffset = 0;
            for (var idOffset = 0; idOffset < trackingData.Length; idOffset += trackerModels.Count)
            {
                foreach (var trackerModel in trackerModels)
                {
                    var trackerId = trackerModel.ModelId + idOffset;
                    var velocityVector = vectorEncoder.Transformer.ToCartesian(trackingData[trackerId].AsVector()) * UnitConversions.Length.AngstromToMeter /
                                         metaData.SimulatedTime;
                    result.Add(new StaticTrackerResult(trackerModel.TrackedPositionIndex + positionIndexOffset, trackerModel.TrackedParticle, velocityVector));
                }

                positionIndexOffset += vectorEncoder.PositionCount;
            }

            return result.AsReadOnlyList();
        }
    }
}