using Mocassin.Mathematics.ValueTypes;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Selection
{
    /// <summary>
    ///     An <see cref="EnsembleMovementSelector" /> that yields the squared values instead of the linear results
    /// </summary>
    public class SquaredEnsembleMovementSelector : EnsembleMovementSelector
    {
        /// <inheritdoc />
        public override bool IsSquared => true;

        /// <inheritdoc />
        protected override void GetMovementVectors(Cartesian3D[] vectors, JobResultContext resultContext)
        {
            var lattice = resultContext.McsReader.GetLattice();
            var trackers = resultContext.McsReader.GetMobileMovementTrackers();
            var trackerMapping = resultContext.McsReader.GetMobileTrackerMapping();
            var transformer = resultContext.ModelContext.GetUnitCellVectorEncoder().Transformer;

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