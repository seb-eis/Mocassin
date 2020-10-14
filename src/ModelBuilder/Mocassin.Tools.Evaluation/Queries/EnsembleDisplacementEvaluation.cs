using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the <see cref="EnsembleDisplacement" /> for each <see cref="IParticle" /> from a
    ///     <see cref="JobContext" /> sequence
    /// </summary>
    public class EnsembleDisplacementEvaluation : JobEvaluation<IReadOnlyList<EnsembleDisplacement>>
    {
        /// <summary>
        ///     Get or set a <see cref="IJobEvaluation{T}" /> that provides the particle counts
        /// </summary>
        public IJobEvaluation<IReadOnlyList<int>> ParticleCountEvaluation { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if the evaluation should return the mean result
        /// </summary>
        public bool YieldMeanResult { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if the evaluation should also yield immobile species
        /// </summary>
        public bool YieldImmobile { get; set; }

        /// <summary>
        ///     Get or set a <see cref="IComparer{T}" /> for <see cref="Cartesian3D" /> for the evaluation
        /// </summary>
        public IComparer<Cartesian3D> VectorComparer { get; set; }

        /// <summary>
        ///     Boolean flag if the result is squared
        /// </summary>
        public virtual bool IsSquared => false;

        /// <inheritdoc />
        public EnsembleDisplacementEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override IReadOnlyList<EnsembleDisplacement> GetValue(JobContext context)
        {
            var vectors = new Cartesian3D[context.ModelContext.GetModelObjects<IParticle>().Count()];
            GetMovementVectors(vectors, context);

            return CreateDisplacementData(vectors, context).AsReadOnly();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            if (ParticleCountEvaluation == null) ParticleCountEvaluation = new ParticleCountEvaluation(JobSet);
            if (!ParticleCountEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Particle count evaluation does not have the same data source.");
            if (VectorComparer == null)
                VectorComparer = new VectorComparer3D<Cartesian3D>(NumericComparer.CreateRanged(0));
        }

        /// <summary>
        ///     Calculates the ensemble displacement for each <see cref="IParticle" /> index
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="context"></param>
        protected virtual void GetMovementVectors(Cartesian3D[] vectors, JobContext context)
        {
            var trackers = context.McsReader.ReadGlobalTrackers();
            var trackingModel = context.EvaluationContext.GetSimulationModel(context.JobModel).SimulationTrackingModel;
            var vectorTransformer = context.ModelContext.GetUnitCellVectorEncoder().Transformer;

            foreach (var trackerModel in trackingModel.GlobalTrackerModels)
            {
                var fractional = trackers[trackerModel.ModelId];
                var cartesian = vectorTransformer.ToCartesian(fractional.AsVector());
                vectors[trackerModel.TrackedParticle.Index] += cartesian;
            }

            for (var i = 1; i < vectors.Length; i++) vectors[i] *= UnitConversions.Length.AngstromToMeter;
        }

        /// <summary>
        ///     Creates a list of <see cref="EnsembleDisplacement" /> objects from the passed data
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="context"></param>
        protected virtual List<EnsembleDisplacement> CreateDisplacementData(Cartesian3D[] vectors, JobContext context)
        {
            var result = new List<EnsembleDisplacement>(vectors.Length);
            var particleCounts = ParticleCountEvaluation[context.DataId];
            for (var i = 0; i < vectors.Length; i++)
            {
                if (!YieldImmobile && VectorComparer.Compare(vectors[i], new Cartesian3D()) == 0) continue;
                var particle = context.ModelContext.ModelProject.DataTracker.FindObject<IParticle>(i);
                var data = new EnsembleDisplacement(IsSquared, particleCounts[i], particle, vectors[i]);
                result.Add(YieldMeanResult ? data.AsMean() : data);
            }

            result.TrimExcess();
            return result;
        }
    }
}