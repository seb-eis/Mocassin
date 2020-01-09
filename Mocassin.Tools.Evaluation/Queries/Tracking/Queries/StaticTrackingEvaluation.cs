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
            var trackingData = jobContext.McsReader.ReadStaticTrackers();
            var result = new List<StaticTrackerResult>(trackingData.Length);
            var vectorTransformer = jobContext.ModelContext.GetUnitCellVectorEncoder().Transformer;
            var metaData = jobContext.McsReader.ReadMetaData();

            for (var idOffset = 0; idOffset < trackingData.Length; idOffset += trackingData.Length)
            {
                foreach (var trackerModel in trackerModels)
                {
                    var trackerId = trackerModel.ModelId + idOffset;
                    var velocityVector = (vectorTransformer.ToCartesian(trackingData[trackerId].AsVector()) * UnitConversions.Length.AngstromToMeter) / metaData.SimulatedTime;
                    result.Add(new StaticTrackerResult(trackerModel.TrackedPositionIndex, trackerModel.TrackedParticle, velocityVector));
                }
            }

            return result;
        }
    }
}