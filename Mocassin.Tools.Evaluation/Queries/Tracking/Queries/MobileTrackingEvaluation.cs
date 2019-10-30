using System.Collections.Generic;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the the <see cref="MobileTrackerResult" /> list from a <see cref="JobContext" /> sequence
    /// </summary>
    public class MobileTrackingEvaluation : JobEvaluation<IReadOnlyList<MobileTrackerResult>>
    {
        /// <inheritdoc />
        protected override IReadOnlyList<MobileTrackerResult> GetValue(JobContext context)
        {
            var lattice = context.McsReader.ReadLattice();
            var trackerMapping = context.McsReader.ReadMobileTrackerMapping();
            var trackerData = context.McsReader.ReadMobileTrackers();
            var result = new List<MobileTrackerResult>(trackerMapping.Length);
            var vectorTransformer = context.ModelContext.GetUnitCellVectorEncoder().Transformer;

            for (var i = 0; i < trackerMapping.Length; i++)
            {
                var positionId = trackerMapping[i];
                var particle = context.ModelContext.GetModelObject<IParticle>(lattice[positionId]);
                var vector = vectorTransformer.ToCartesian(trackerData[i]);
                result.Add(new MobileTrackerResult(particle, positionId, vector * UnitConversions.Length.AngstromToMeter));
            }

            return result;
        }

        /// <inheritdoc />
        public MobileTrackingEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
            ExecuteParallel = true;
        }
    }
}