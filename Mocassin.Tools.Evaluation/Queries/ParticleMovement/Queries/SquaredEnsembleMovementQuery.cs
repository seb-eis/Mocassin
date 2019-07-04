using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Selection
{
    /// <summary>
    ///     Selects the cartesian squared displacement information in [m^2] for all particles from a <see cref="JobContext"/>
    /// </summary>
    public class SquaredEnsembleMovementQuery : EnsembleMovementQuery
    {
        /// <inheritdoc />
        public override bool IsSquared => true;

        /// <inheritdoc />
        public SquaredEnsembleMovementQuery(IEnumerable<JobContext> jobContexts)
            : base(jobContexts)
        {
        }

        /// <inheritdoc />
        protected override void GetMovementVectors(Cartesian3D[] vectors, JobContext evaluationContext)
        {
            var lattice = evaluationContext.McsReader.GetLattice();
            var trackers = evaluationContext.McsReader.GetMobileMovementTrackers();
            var trackerMapping = evaluationContext.McsReader.GetMobileTrackerMapping();
            var transformer = evaluationContext.ModelContext.GetUnitCellVectorEncoder().Transformer;

            for (var i = 0; i < trackerMapping.Length; i++)
            {
                var positionId = trackerMapping[i];
                var particleId = lattice[positionId];
                var vector = transformer.ToCartesian(trackers[i].AsVector());
                vectors[particleId] += new Cartesian3D(vector.X * vector.X, vector.Y * vector.Y, vector.Z * vector.Z);
            }
        }
    }
}