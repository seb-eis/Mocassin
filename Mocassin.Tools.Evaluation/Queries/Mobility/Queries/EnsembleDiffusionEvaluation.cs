using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries.Base;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract <see cref="EnsembleDiffusion" /> form a <see cref="IEvaluableJobSet" />
    /// </summary>
    public class EnsembleDiffusionEvaluation : JobEvaluation<IReadOnlyList<EnsembleDiffusion>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="EnsembleDisplacement" /> set
        /// </summary>
        public IJobEvaluation<IReadOnlyCollection<EnsembleDisplacement>> EnsembleDisplacementEvaluation { get; set; }

        /// <inheritdoc />
        public EnsembleDiffusionEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <summary>
        ///     Gets the set average and standard deviation of the entire internal result collection
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<(EnsembleDiffusion Average, EnsembleDiffusion Deviation)> GetSetAverage()
        {
            var result = new List<(EnsembleDiffusion, EnsembleDiffusion)>();
            for (var i = 0; i < Result.First().Count; i++)
            {
                var (cX, dX) = Equations.Statistics.Average(Result.Select(x => x[i]), y => y.CoefficientX);
                var (cY, dY) = Equations.Statistics.Average(Result.Select(x => x[i]), y => y.CoefficientY);
                var (cZ, dZ) = Equations.Statistics.Average(Result.Select(x => x[i]), y => y.CoefficientZ);
                var avg = new EnsembleDiffusion(Result[0][i].Particle, cX, cY, cZ);
                var dev = new EnsembleDiffusion(Result[0][i].Particle, dX, dY, dZ);
                result.Add((avg, dev));
            }

            return result;
        }

        /// <inheritdoc />
        protected override IReadOnlyList<EnsembleDiffusion> GetValue(JobContext jobContext)
        {
            var displacements = EnsembleDisplacementEvaluation[jobContext.DataId];
            var time = jobContext.McsReader.GetMetaData().SimulatedTime;
            var result = new List<EnsembleDiffusion>(displacements.Count);

            foreach (var displacement in displacements.Select(x => x.IsMean ? x : x.AsMean()))
            {
                if (!displacement.IsSquared) throw new InvalidOperationException("Displacement data is not squared.");
                var (compX, compY, compZ) = Equations.Diffusion.MeanSquareToCoefficient(displacement.Vector, time);
                result.Add(new EnsembleDiffusion(displacement.Particle, compX, compY, compZ));
            }

            return result.AsReadOnly();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            EnsembleDisplacementEvaluation = EnsembleDisplacementEvaluation ?? new SquaredDisplacementEvaluation(JobSet);

            if (!EnsembleDisplacementEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Ensemble displacement evaluation is not compatible.");
            base.PrepareForExecution();
        }
    }
}