using System.Collections.Generic;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the <see cref="StaticTrackerResult" /> set from a <see cref="IEvaluableJobSet" />
    /// </summary>
    public class StaticTrackingEvaluation : JobEvaluation<IReadOnlyList<StaticTrackerResult>>
    {
        /// <inheritdoc />
        public StaticTrackingEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override IReadOnlyList<StaticTrackerResult> GetValue(JobContext jobContext)
        {
            var trackerModels = jobContext.SimulationModel.SimulationTrackingModel.StaticTrackerModels;
            var trackerCountPerCell = jobContext.SimulationModel.SimulationTrackingModel.StaticTrackerCount;
            var trackingData = jobContext.McsReader.GetStaticTrackers();
            var result = new List<StaticTrackerResult>(trackingData.Length);
            var vectorTransformer = jobContext.ModelContext.GetUnitCellVectorEncoder().Transformer;

            for (var offset = 0; offset < trackingData.Length; offset += trackingData.Length)
            {
                foreach (var trackerModel in trackerModels)
                {
                    var vector = vectorTransformer.ToCartesian(trackingData[trackerModel.ModelId + offset]) *
                                 UnitConversions.Length.AngToMeter;
                    result.Add(new StaticTrackerResult(trackerModel.TrackedPositionIndex, trackerModel.TrackedParticle, vector));
                }
            }

            return result;
        }
    }
}