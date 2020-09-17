using System;
using System.Collections.Generic;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries.Base;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query that extracts <see cref="TransitionMobility" /> data from a <see cref="IEvaluableJobSet" />
    /// </summary>
    public class TransitionMobilityEvaluation : JobEvaluation<IReadOnlyList<TransitionMobility>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="GlobalTrackerResult" /> set
        /// </summary>
        public IJobEvaluation<IReadOnlyList<GlobalTrackerResult>> GlobalTrackerEvaluation { get; set; }

        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="EnsembleMetaData" /> set
        /// </summary>
        public IJobEvaluation<IReadOnlyList<EnsembleMetaData>> EnsembleMetaEvaluation { get; set; }

        /// <inheritdoc />
        public TransitionMobilityEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override IReadOnlyList<TransitionMobility> GetValue(JobContext jobContext)
        {
            var globalTrackerResults = GlobalTrackerEvaluation[jobContext.DataId];
            var metaData = EnsembleMetaEvaluation[jobContext.DataId];
            var normField = jobContext.SimulationModel.NormalizedElectricFieldVector;
            var fieldModulus = jobContext.JobModel.JobMetaData.ElectricFieldModulus;
            var time = jobContext.McsReader.ReadMetaData().SimulatedTime;

            var result = new List<TransitionMobility>(globalTrackerResults.Count);

            foreach (var trackerResult in globalTrackerResults)
            {
                var particle = trackerResult.TrackerModel.TrackedParticle;
                var density = metaData[particle.Index].ParticleDensity;
                var displacement = trackerResult.DisplacementData.AsMean().Vector;
                var mobility = Equations.Mobility.DisplacementToMobility(displacement, normField, fieldModulus, time);
                var conductivity = Equations.Mobility.MobilityToConductivity(mobility, particle.Charge, density);
                var normConductivity = Equations.Mobility.MobilityToConductivity(mobility, 1, density);
                result.Add(new TransitionMobility(trackerResult.TrackerModel, new EnsembleMobility(particle, mobility, conductivity, normConductivity)));
            }

            return result.AsReadOnly();
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