using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries.Base;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract <see cref="EnsembleDiffusion" /> form a <see cref="IEvaluableJobCollection" />
    /// </summary>
    public class EnsembleDiffusionEvaluation : JobEvaluation<IReadOnlyList<EnsembleDiffusion>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the <see cref="EnsembleDisplacement" /> set
        /// </summary>
        public IJobEvaluation<IReadOnlyCollection<EnsembleDisplacement>> EnsembleDisplacementEvaluation { get; set; }

        /// <inheritdoc />
        public EnsembleDiffusionEvaluation(IEvaluableJobCollection jobCollection)
            : base(jobCollection)
        {
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
            EnsembleDisplacementEvaluation = EnsembleDisplacementEvaluation ?? new SquaredDisplacementEvaluation(JobCollection);

            if (!EnsembleDisplacementEvaluation.JobCollection.CompatibleTo(JobCollection))
                throw new InvalidOperationException("Ensemble displacement evaluation is not compatible.");
            base.PrepareForExecution();
        }
    }
}