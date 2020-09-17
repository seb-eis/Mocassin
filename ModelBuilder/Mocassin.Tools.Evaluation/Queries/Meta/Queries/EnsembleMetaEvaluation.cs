using System;
using System.Collections.Generic;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract <see cref="EnsembleMetaData" /> from a <see cref="IEvaluableJobSet" />
    /// </summary>
    public class EnsembleMetaEvaluation : JobEvaluation<IReadOnlyList<EnsembleMetaData>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the particle count data
        /// </summary>
        public IJobEvaluation<IReadOnlyList<int>> ParticleCountEvaluation { get; set; }

        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="LatticeMetaData" />
        /// </summary>
        public IJobEvaluation<LatticeMetaData> LatticeMetaEvaluation { get; set; }

        /// <inheritdoc />
        public EnsembleMetaEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override IReadOnlyList<EnsembleMetaData> GetValue(JobContext jobContext)
        {
            var particleCounts = ParticleCountEvaluation[jobContext.DataId];
            var latticeMetaData = LatticeMetaEvaluation[jobContext.DataId];
            var result = new List<EnsembleMetaData>(particleCounts.Count);

            for (var i = 0; i < particleCounts.Count; i++)
            {
                var particle = jobContext.ModelContext.GetModelObject<IParticle>(i);
                var count = particleCounts[i];
                result.Add(new EnsembleMetaData(particle, count, count / latticeMetaData.Volume));
            }

            return result.AsReadOnly();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            ParticleCountEvaluation ??= new ParticleCountEvaluation(JobSet);
            LatticeMetaEvaluation ??= new LatticeMetaEvaluation(JobSet);

            if (!ParticleCountEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Particle count evaluation is not compatible.");
            if (!ParticleCountEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Lattice meta evaluation is not compatible.");

            base.PrepareForExecution();
        }
    }
}