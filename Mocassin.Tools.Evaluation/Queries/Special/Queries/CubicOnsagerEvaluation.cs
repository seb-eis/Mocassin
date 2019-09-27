using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries.Base;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     A <see cref="JobEvaluation{T}" /> that yields the onsager coefficient matrix for the special case of a cubic system
    ///     using the kubo green solution {R_i*R_j}/(6*V*k*T*t)
    /// </summary>
    public class CubicOnsagerEvaluation : JobEvaluation<double[,]>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="EnsembleDisplacement" /> sets
        /// </summary>
        public IJobEvaluation<IReadOnlyList<EnsembleDisplacement>> DisplacementEvaluation { get; set; }

        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="LatticeMetaData" /> sets
        /// </summary>
        public IJobEvaluation<LatticeMetaData> LatticeMetaEvaluation { get; set; }

        /// <inheritdoc />
        public CubicOnsagerEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <summary>
        ///     Takes the internal results an averages the results to an average coefficient matrix and a deviation matrix
        /// </summary>
        /// <returns></returns>
        public (double[,] Average, double[,] Deviation) AverageResultWithDeviation()
        {
            var (average, deviation) = (CreateEmptyMatrix(JobSet.First()), CreateEmptyMatrix(JobSet.First()));
            for (var i = 0; i < average.GetLength(0); i++)
            {
                for (var j = i; j < average.GetLength(0); j++)
                {
                    var (avg, dev) = Equations.Statistics.Average(Result, x => x[i, j]);
                    average[i, j] = average[j, i] = avg;
                    deviation[i, j] = deviation[j, i] = dev;
                }
            }

            return (average, deviation);
        }

        /// <inheritdoc />
        protected override double[,] GetValue(JobContext jobContext)
        {
            var result = CreateEmptyMatrix(jobContext);
            var displacements = DisplacementEvaluation[jobContext.DataId];
            var volume = LatticeMetaEvaluation[jobContext.DataId].Volume;
            var temperature = jobContext.JobModel.JobMetaData.Temperature;
            var time = jobContext.McsReader.ReadMetaData().SimulatedTime;
            
            if (displacements.First().IsSquared) throw new InvalidOperationException("Cannot use squared displacements.");

            for (var i = 0; i < displacements.Count; i++)
            {
                for (var j = i; j < displacements.Count; j++)
                {
                    var dis1 = displacements[i];
                    var dis2 = displacements[j];
                    var value = Equations.Statistics.CubicOnsagerKuboGreen(dis1.Vector, dis2.Vector, volume, time, temperature);
                    result[dis1.Particle.Index, dis2.Particle.Index] = result[dis2.Particle.Index, dis1.Particle.Index] = value;
                }
            }

            return result;
        }

        /// <summary>
        ///     Creates the empty coefficient matrix of the required size
        /// </summary>
        /// <param name="jobContext"></param>
        /// <returns></returns>
        protected double[,] CreateEmptyMatrix(JobContext jobContext)
        {
            var particleCount = jobContext.ModelContext.ModelProject.DataTracker.ObjectCount<IParticle>();
            return new double[particleCount, particleCount];
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            DisplacementEvaluation = DisplacementEvaluation ?? new EnsembleDisplacementEvaluation(JobSet);
            LatticeMetaEvaluation = LatticeMetaEvaluation ?? new LatticeMetaEvaluation(JobSet);

            if (!DisplacementEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Displacement evaluation is not compatible.");
            if (!LatticeMetaEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Lattice meta evaluation is not compatible.");

            base.PrepareForExecution();
        }
    }
}