using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries.Base;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the <see cref="EnsembleMobility" /> data from an <see cref="IEvaluableJobSet" />
    /// </summary>
    public class EnsembleMobilityEvaluation : JobEvaluation<IReadOnlyList<EnsembleMobility>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}"/> that supplies the <see cref="EnsembleDisplacement"/> set
        /// </summary>
        public IJobEvaluation<IReadOnlyList<EnsembleDisplacement>> EnsembleDisplacementEvaluation { get; set; }

        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}"/> that supplies the <see cref="EnsembleMetaData"/> set
        /// </summary>
        public IJobEvaluation<IReadOnlyList<EnsembleMetaData>> EnsembleMetaEvaluation { get; set; }

        /// <inheritdoc />
        public EnsembleMobilityEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override IReadOnlyList<EnsembleMobility> GetValue(JobContext jobContext)
        {
            var displacements = EnsembleDisplacementEvaluation[jobContext.DataId];
            var metaData = EnsembleMetaEvaluation[jobContext.DataId];
            var normField = jobContext.SimulationModel.NormalizedElectricFieldVector;
            var fieldModulus = jobContext.JobModel.JobMetaData.ElectricFieldModulus;
            var time = jobContext.McsReader.ReadMetaData().SimulatedTime;

            var result = new List<EnsembleMobility>(displacements.Count);
            foreach (var displacement in displacements.Select(x => x.IsMean ? x : x.AsMean()))
            {
                var particle = displacement.Particle;
                var density = metaData[particle.Index].ParticleDensity;
                var mobility = Equations.Mobility.DisplacementToMobility(displacement.Vector, normField, fieldModulus, time);
                var conductivity = Equations.Mobility.MobilityToConductivity(mobility, particle.Charge, density);
                result.Add(new EnsembleMobility(particle, mobility, conductivity));
            }

            return result.AsReadOnly();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            EnsembleDisplacementEvaluation = EnsembleDisplacementEvaluation ?? new EnsembleDisplacementEvaluation(JobSet);
            EnsembleMetaEvaluation = EnsembleMetaEvaluation ?? new EnsembleMetaEvaluation(JobSet);

            if (!EnsembleDisplacementEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Ensemble displacement evaluation is not compatible.");
            if (!EnsembleMetaEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Ensemble meta evaluation is not compatible.");
            base.PrepareForExecution();
        }
    }
}