using System;
using System.Collections.Generic;
using Mocassin.Framework.Collections.Mocassin.Tools.Evaluation.Queries;
using Mocassin.Framework.Extensions;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query that extracts <see cref="TransitionMobility" /> data from a <see cref="IEvaluableJobSet" />
    /// </summary>
    public class TransitionMobilityEvaluation : JobEvaluation<ReadOnlyList<TransitionMobility>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="GlobalTrackerResult" /> set
        /// </summary>
        public GlobalTrackingEvaluation GlobalTrackerEvaluation { get; set; }

        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="EnsembleMetaData" /> set
        /// </summary>
        public EnsembleMetaEvaluation EnsembleMetaEvaluation { get; set; }

        /// <inheritdoc />
        public TransitionMobilityEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override ReadOnlyList<TransitionMobility> GetValue(JobContext jobContext)
        {
            var globalTrackerResults = GlobalTrackerEvaluation[jobContext.DataId];
            var metaData = EnsembleMetaEvaluation[jobContext.DataId];
            var normField = jobContext.SimulationModel.NormalizedElectricFieldVector;
            var fieldModulus = jobContext.JobModel.JobMetaData.ElectricFieldModulus;
            var time = jobContext.McsReader.ReadMetaData().SimulatedTime;
            var temperature = jobContext.JobModel.JobMetaData.Temperature;

            var result = new List<TransitionMobility>(globalTrackerResults.Count);

            foreach (var trackerResult in globalTrackerResults)
            {
                var particle = trackerResult.TrackerModel.TrackedParticle;
                var density = metaData[particle.Index].ParticleDensity;
                var displacement = trackerResult.DisplacementData.AsMean().VectorR;
                var mobility = Equations.Mobility.DisplacementToMobility(displacement, normField, fieldModulus, time);
                var conductivity = Equations.Mobility.MobilityToConductivity(mobility, particle.Charge, density);
                var normConductivity = Equations.Mobility.MobilityToConductivity(mobility, 1, density);
                var diffCoefficient = Equations.Mobility.MobilityToEffectiveDiffusionCoefficient(mobility, temperature, particle.Charge);
                result.Add(new TransitionMobility(trackerResult.TrackerModel, new EnsembleMobility(particle, mobility, conductivity, normConductivity, diffCoefficient)));
            }

            return result.AsReadOnlyList();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            GlobalTrackerEvaluation ??= new GlobalTrackingEvaluation(JobSet);
            EnsembleMetaEvaluation ??= new EnsembleMetaEvaluation(JobSet);

            if (!GlobalTrackerEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Global tracker evaluation is not compatible.");
            if (!EnsembleMetaEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Ensemble meta evaluation evaluation is not compatible.");
            base.PrepareForExecution();
        }
    }
}