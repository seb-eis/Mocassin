using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections.Mocassin.Tools.Evaluation.Queries;
using Mocassin.Framework.Extensions;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the <see cref="EnsembleMobility" /> data from an <see cref="IEvaluableJobSet" />
    /// </summary>
    public class EnsembleMobilityEvaluation : JobEvaluation<ReadOnlyList<EnsembleMobility>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="EnsembleDisplacement" /> set
        /// </summary>
        public EnsembleDisplacementEvaluation EnsembleDisplacementEvaluation { get; set; }

        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="EnsembleMetaData" /> set
        /// </summary>
        public EnsembleMetaEvaluation EnsembleMetaEvaluation { get; set; }

        /// <inheritdoc />
        public EnsembleMobilityEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override ReadOnlyList<EnsembleMobility> GetValue(JobContext jobContext)
        {
            var displacements = EnsembleDisplacementEvaluation[jobContext.DataId];
            var metaData = EnsembleMetaEvaluation[jobContext.DataId];
            var normField = jobContext.SimulationModel.NormalizedElectricFieldVector;
            var fieldModulus = jobContext.JobModel.JobMetaData.ElectricFieldModulus;
            var temperature = jobContext.JobModel.JobMetaData.Temperature;
            var time = jobContext.McsReader.ReadMetaData().SimulatedTime;

            var result = new List<EnsembleMobility>(displacements.Count);
            foreach (var displacement in displacements.Select(x => x.IsMean ? x : x.AsMean()))
            {
                var particle = displacement.Particle;
                var density = metaData[particle.Index].ParticleDensity;
                var mobility = Equations.Mobility.DisplacementToMobility(displacement.VectorR, normField, fieldModulus, time);
                var conductivity = Equations.Mobility.MobilityToConductivity(mobility, particle.Charge, density);
                var normConductivity = Equations.Mobility.MobilityToConductivity(mobility, 1, density);
                var diffCoefficient = Equations.Mobility.MobilityToEffectiveDiffusionCoefficient(mobility, temperature, particle.Charge);
                result.Add(new EnsembleMobility(particle, mobility, conductivity, normConductivity, diffCoefficient));
            }

            return result.AsReadOnlyList();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            EnsembleDisplacementEvaluation ??= new EnsembleDisplacementEvaluation(JobSet);
            EnsembleMetaEvaluation ??= new EnsembleMetaEvaluation(JobSet);

            if (!EnsembleDisplacementEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Ensemble displacement evaluation is not compatible.");
            if (!EnsembleMetaEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Ensemble meta evaluation is not compatible.");
            base.PrepareForExecution();
        }
    }
}