using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    public class EnsembleShiftCorrelationEvaluation : JobEvaluation<IReadOnlyList<EnsembleShiftCorrelationData>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> of <see cref="MobileTrackerResult" />
        /// </summary>
        public IJobEvaluation<IReadOnlyList<MobileTrackerResult>> MobileTrackingEvaluation { get; set; }

        /// <summary>
        ///     Get or set a <see cref="IJobEvaluation{T}" /> that provides the particle counts
        /// </summary>
        public IJobEvaluation<IReadOnlyList<int>> ParticleCountEvaluation { get; set; }

        public bool YieldImmobile { get; set; }

        public bool SkipVacancies { get; set; }

        public IComparer<double> DoubleComparer { get; set; }

        /// <inheritdoc />
        public EnsembleShiftCorrelationEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
            ExecuteParallel = true;
        }

        /// <inheritdoc />
        protected override IReadOnlyList<EnsembleShiftCorrelationData> GetValue(JobContext jobContext)
        {
            var particleCount = jobContext.ModelContext.GetModelObjects<IParticle>().Count();
            var rirjs = new double[particleCount,particleCount];
            var deltaRiRjs = new double[particleCount,particleCount];

            PopulateDataByParticle(rirjs, deltaRiRjs, jobContext);

            var result = CreateResultData(rirjs, deltaRiRjs, jobContext);

            return result.AsReadOnly();
        }

        protected void PopulateDataByParticle(double[,] rirjs, double[,] deltaRiRjs, JobContext jobContext)
        {
            var trackers = MobileTrackingEvaluation[jobContext.DataId];
            var lengths = trackers.Select(x => x.Displacement.GetLength()).ToList(trackers.Count);

            for (var i = 0; i < trackers.Count; i++)
            {
                var tracker1 = trackers[i];
                if (SkipVacancies && tracker1.Particle.IsVacancy) continue;
                for (var j = 0; j < trackers.Count; j++)
                {
                    if (i == j) continue;
                    var tracker2 = trackers[j];
                    if (SkipVacancies && tracker2.Particle.IsVacancy) continue;
                    var dotProduct = tracker1.Displacement * tracker2.Displacement;
                    rirjs[tracker1.Particle.Index, tracker2.Particle.Index] += dotProduct;
                    deltaRiRjs[tracker1.Particle.Index, tracker2.Particle.Index] += lengths[i] * lengths[j];
                }
            }
        }

        protected virtual List<EnsembleShiftCorrelationData> CreateResultData(double[,] rirjs, double[,] deltaRiRjs, JobContext context)
        {
            var result = new List<EnsembleShiftCorrelationData>(rirjs.Length);
            var particles = context.ModelContext.ModelProject.DataTracker.MapObjects<IParticle>();
            var particleCounts = ParticleCountEvaluation[context.DataId];
            for (var i = 0; i < rirjs.GetLength(0); i++)
            {
                for (var j = 0; j < rirjs.GetLength(1); j++)
                {
                    if (!YieldImmobile && DoubleComparer.Compare(rirjs[i,j], 0.0) == 0) continue;
                    var data = new EnsembleShiftCorrelationData(rirjs[i,j], deltaRiRjs[i,j], particleCounts[i], particleCounts[j], particles[i], particles[j]);
                    result.Add(data);   
                }
            }

            result.TrimExcess();
            return result;
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            MobileTrackingEvaluation ??= new MobileTrackingEvaluation(JobSet);
            if (!MobileTrackingEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("The mobile tracking evaluation is not compatible");

            ParticleCountEvaluation ??= new ParticleCountEvaluation(JobSet);
            if (!ParticleCountEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Particle count evaluation does not have the same data source.");
            DoubleComparer ??= Comparer<double>.Default;

            MobileTrackingEvaluation.Run().Wait();
            ParticleCountEvaluation.Run().Wait();

            base.PrepareForExecution();
        }
    }
}