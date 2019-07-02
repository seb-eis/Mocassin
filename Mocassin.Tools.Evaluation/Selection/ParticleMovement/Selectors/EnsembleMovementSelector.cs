using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;
using Mocassin.Tools.Evaluation.Selection.Counting.Selectors;

namespace Mocassin.Tools.Evaluation.Selection
{
    /// <summary>
    ///     Selects the cartesian displacement information for all particles from a <see cref="JobResultContext"/>
    /// </summary>
    public class EnsembleMovementSelector : JobResultSelector<IList<EnsembleMovement>>
    {
        /// <summary>
        ///     Get or set a <see cref="IDictionary{TKey,TValue}"/> that contains the particle counts
        /// </summary>
        public IDictionary<IParticle, int> ParticleCounts { get; set; }

        /// <summary>
        ///     Get or set a boolean flag that indicates if the selector yields the mean result
        /// </summary>
        public bool YieldMeanResult { get; set; }

        /// <summary>
        ///     Boolean flag if the result is squared
        /// </summary>
        public virtual bool IsSquared => false;

        /// <inheritdoc />
        public override IList<EnsembleMovement> GetValue(JobResultContext resultContext)
        {
            var vectors = new Cartesian3D[resultContext.ModelContext.GetModelObjects<IParticle>().Count()];
            GetMovementVectors(vectors, resultContext);

            return CreateDisplacementData(vectors, resultContext);
        }

        /// <summary>
        ///     Calculates the ensemble displacement for each <see cref="IParticle"/> index
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="resultContext"></param>
        protected virtual void GetMovementVectors(Cartesian3D[] vectors, JobResultContext resultContext)
        {
            var trackers = resultContext.McsReader.GetGlobalMovementTrackers();
            var trackingModel = resultContext.EvaluationContext.GetSimulationModel(resultContext.JobModel).SimulationTrackingModel;
            var vectorEncoder = resultContext.ModelContext.GetUnitCellVectorEncoder();

            foreach (var trackerModel in trackingModel.GlobalTrackerModels)
            {
                var fractional = trackers[trackerModel.ModelId].AsVector();
                var cartesian = vectorEncoder.Transformer.ToCartesian(fractional);
                vectors[trackerModel.TrackedParticle.Index] += cartesian;
            }
        }

        /// <summary>
        ///     Creates a list of <see cref="EnsembleMovement"/> objects from the passed data
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="resultContext"></param>
        protected virtual IList<EnsembleMovement> CreateDisplacementData(Cartesian3D[] vectors, JobResultContext resultContext)
        {
            var result = new List<EnsembleMovement>(vectors.Length);

            ParticleCounts = ParticleCounts ?? new ParticleCountSelector().GetValue(resultContext);

            for (var i = 0; i < vectors.Length; i++)
            {
                var particle = resultContext.ModelContext.ModelProject.DataTracker.FindObjectByIndex<IParticle>(i);
                var data = new EnsembleMovement(IsSquared, ParticleCounts[particle], particle, vectors[i]);
                result.Add(YieldMeanResult ? data.AsMean() : data);
            }

            return result;
        }
    }
}