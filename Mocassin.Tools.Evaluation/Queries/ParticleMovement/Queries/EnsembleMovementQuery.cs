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
    ///     Selects the cartesian displacement information in [m] for all particles from a <see cref="JobContext"/>
    /// </summary>
    public class EnsembleMovementQuery : JobContextQuery<IList<EnsembleMovement>>
    {
        /// <summary>
        ///     Get or set a <see cref="IJobContextQuery{T}"/> that serves as the particle count source
        /// </summary>
        public IJobContextQuery<int[]> ParticleCountSource { get; set; }

        /// <summary>
        ///     Get or set a boolean flag that indicates if the selector yields the mean result
        /// </summary>
        public bool YieldMeanResult { get; set; }

        /// <summary>
        ///     Boolean flag if the result is squared
        /// </summary>
        public virtual bool IsSquared => false;

        /// <inheritdoc />
        public EnsembleMovementQuery(IEnumerable<JobContext> jobContexts)
            : base(jobContexts)
        {
        }

        /// <inheritdoc />
        public override IList<EnsembleMovement> Execute(JobContext context)
        {
            var vectors = new Cartesian3D[context.ModelContext.GetModelObjects<IParticle>().Count()];
            GetMovementVectors(vectors, context);

            return CreateDisplacementData(vectors, context);
        }

        /// <inheritdoc />
        public override void EnsureDataSourcesExist(IEnumerable<JobContext> jobContexts)
        {
            if (ParticleCountSource == null) ParticleCountSource = new ParticleCountQuery(jobContexts);
        }

        /// <summary>
        ///     Calculates the ensemble displacement for each <see cref="IParticle"/> index
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="context"></param>
        protected virtual void GetMovementVectors(Cartesian3D[] vectors, JobContext context)
        {
            var trackers = context.McsReader.GetGlobalMovementTrackers();
            var trackingModel = context.EvaluationContext.GetSimulationModel(context.JobModel).SimulationTrackingModel;
            var vectorEncoder = context.ModelContext.GetUnitCellVectorEncoder();

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
        /// <param name="context"></param>
        protected virtual IList<EnsembleMovement> CreateDisplacementData(Cartesian3D[] vectors, JobContext context)
        {
            var result = new List<EnsembleMovement>(vectors.Length);

            var particleCounts = ParticleCountSource[context.DataId];
            for (var i = 0; i < vectors.Length; i++)
            {
                var particle = context.ModelContext.ModelProject.DataTracker.FindObjectByIndex<IParticle>(i);
                var data = new EnsembleMovement(IsSquared, particleCounts[i], particle, vectors[i]);
                result.Add(YieldMeanResult ? data.AsMean() : data);
            }

            return result;
        }
    }
}