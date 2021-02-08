using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections.Mocassin.Tools.Evaluation.Queries;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     A <see cref="JobEvaluation{T}"/> that calculates the displacement correlation of all particles for a single simulation
    /// </summary>
    public class EnsembleCorrelationEvaluation : JobEvaluation<ReadOnlyList<EnsembleCorrelationData>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> of <see cref="MobileTrackerResult" />
        /// </summary>
        public MobileTrackingEvaluation MobileTrackingEvaluation { get; set; }

        /// <summary>
        ///     Get or set the required <see cref="EnsembleMetaEvaluation" />
        /// </summary>
        public EnsembleMetaEvaluation EnsembleMetaEvaluation { get; set; }

        /// <summary>
        ///     Get or set a boolean flag that indicates if the immobile species should eb added to the result
        /// </summary>
        public bool YieldImmobile { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if vacancy species should be correlated as well (Usually longer calculation times)
        /// </summary>
        public bool SkipVacancies { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if quick correlation should be used
        /// </summary>
        public bool UseQuickCorrelationBySums { get; set; }

        /// <summary>
        ///     Get or set the <see cref="IComparer{T}"/> that is used to compare floats and check for zero
        /// </summary>
        public IComparer<double> DoubleComparer { get; set; }

        /// <inheritdoc />
        public EnsembleCorrelationEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
            ExecuteParallel = true;
            UseQuickCorrelationBySums = true;
        }

        /// <inheritdoc />
        protected override ReadOnlyList<EnsembleCorrelationData> GetValue(JobContext jobContext)
        {
            var particleCount = jobContext.ModelContext.GetModelObjects<IParticle>().Count();
            var riRjs = new double[particleCount, particleCount];
            var riRis = new double[particleCount, particleCount];

            if (UseQuickCorrelationBySums)
            {
                QuickPopulateDataByParticle(riRjs, riRis, jobContext);
            }
            else
            {
                PopulateDataByParticle(riRjs, riRis, jobContext);   
            }

            var result = CreateResultData(riRjs, riRis, jobContext);

            return result.AsReadOnlyList();
        }

        /// <summary>
        ///     Populates the correlation tables r_i * r_j and r_i * r_i using the mobile trackers of the ensemble (slow)
        /// </summary>
        /// <param name="riRjs"></param>
        /// <param name="riRis"></param>
        /// <param name="jobContext"></param>
        protected void PopulateDataByParticle(double[,] riRjs, double[,] riRis, JobContext jobContext)
        {
            var trackers = MobileTrackingEvaluation[jobContext.DataId];

            if (SkipVacancies) trackers = trackers.Where(x => !x.Particle.IsVacancy).ToList().AsReadOnlyList();
            for (var i = 0; i < trackers.Count; i++)
            {
                var tracker1 = trackers[i];
                var riRi = tracker1.Displacement * tracker1.Displacement;
                riRis[tracker1.Particle.Index, tracker1.Particle.Index] += riRi;

                for (var j = i + 1; j < trackers.Count; j++)
                {
                    var tracker2 = trackers[j];
                    var dotProduct = tracker1.Displacement * tracker2.Displacement;
                    riRjs[tracker1.Particle.Index, tracker2.Particle.Index] += dotProduct;
                }
            }

            for (var i = 0; i < riRjs.GetLength(0); i++)
            {
                for (var j = 0; j < riRjs.GetLength(1); j++)
                {
                    riRjs[i, j] *= 2.0;
                }
            }
        }

        /// <summary>
        ///     Populates the correlation tables r_i * r_j and r_i * r_i using the mobile trackers of the ensembles by first building the sums (fast)
        /// </summary>
        /// <param name="riRjs"></param>
        /// <param name="riRis"></param>
        /// <param name="jobContext"></param>
        protected void QuickPopulateDataByParticle(double[,] riRjs, double[,] riRis, JobContext jobContext)
        {
            var trackers = MobileTrackingEvaluation[jobContext.DataId];
            if (SkipVacancies) trackers = trackers.Where(x => !x.Particle.IsVacancy).ToList().AsReadOnlyList();

            var ensembleShifts = new Cartesian3D[riRis.GetLength(0)];
            foreach (var tracker in trackers)
            {
                var i = tracker.Particle.Index;
                riRis[i, i] += tracker.Displacement * tracker.Displacement;
                ensembleShifts[i] += tracker.Displacement;
            }

            for (var i = 0; i < ensembleShifts.Length; i++)
            {
                for (var j = 0; j < ensembleShifts.Length; j++)
                {
                    var dRi = ensembleShifts[i];
                    var dRj = ensembleShifts[j];
                    riRjs[i, j] = dRi * dRj - riRis[i, j];
                }
            }
        }

        /// <summary>
        ///     Converts the correlation data tables into a set of <see cref="EnsembleCorrelationData"/> by adding affiliated data
        /// </summary>
        /// <param name="riRjs"></param>
        /// <param name="riRis"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual List<EnsembleCorrelationData> CreateResultData(double[,] riRjs, double[,] riRis, JobContext context)
        {
            var result = new List<EnsembleCorrelationData>(riRjs.Length);
            var particles = context.ModelContext.ModelProject.DataTracker.MapObjects<IParticle>();
            var particleCounts = EnsembleMetaEvaluation.ParticleCountEvaluation[context.DataId];
            var cellVolume = EnsembleMetaEvaluation.LatticeMetaEvaluation[context.DataId].Volume;
            var simulatedTime = context.McsReader.ReadMetaData().SimulatedTime;
            var temperature = context.JobModel.JobMetaData.Temperature;


            for (var i = 0; i < riRjs.GetLength(0); i++)
            {
                for (var j = 0; j < riRjs.GetLength(1); j++)
                {
                    if (!YieldImmobile && DoubleComparer.Compare(riRjs[i, j], 0.0) == 0) continue;
                    var onsagerCoefficient = GetOnsagerCoefficient(riRis, riRjs, simulatedTime, cellVolume, temperature, i, j);
                    var data = new EnsembleCorrelationData(riRis[i, j], riRjs[i, j], onsagerCoefficient, cellVolume, simulatedTime, particleCounts[i], particleCounts[j],
                        particles[i], particles[j]);
                    result.Add(data);
                }
            }

            result.TrimExcess();
            return result;
        }

        /// <summary>
        ///     Gets the onsager coefficient from the correlation data. Yields L_ii for i==j and L_ij otherwise
        /// </summary>
        /// <param name="riRis"></param>
        /// <param name="riRjs"></param>
        /// <param name="simulatedTime"></param>
        /// <param name="cellVolume"></param>
        /// <param name="temperature"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public double GetOnsagerCoefficient(double[,] riRis, double[,] riRjs, double simulatedTime, double cellVolume, double temperature, int i, int j) =>
            i == j
                ? Equations.Statistics.CalcOnsagerR3FromCorrelationLii(riRis[i, j], riRjs[i, j], simulatedTime, temperature, cellVolume)
                : Equations.Statistics.CalcOnsagerR3FromCorrelationLij(riRjs[i, j], simulatedTime, temperature, cellVolume);

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            MobileTrackingEvaluation ??= new MobileTrackingEvaluation(JobSet);
            if (!MobileTrackingEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("The mobile tracking evaluation is not compatible");

            EnsembleMetaEvaluation ??= new EnsembleMetaEvaluation(JobSet);
            if (!EnsembleMetaEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("The ensemble meta evaluation is not compatible");

            DoubleComparer ??= Comparer<double>.Default;

            MobileTrackingEvaluation.Run().Wait();
            EnsembleMetaEvaluation.Run().Wait();

            base.PrepareForExecution();
        }
    }
}