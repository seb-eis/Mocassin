using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract <see cref="EnsembleDiffusion" /> form a <see cref="IEvaluableJobSet" />
    /// </summary>
    public class EnsembleDiffusionEvaluation : JobEvaluation<IReadOnlyList<EnsembleDiffusion>>
    {
        /// <summary>
        ///     Get or set the <see cref="IJobEvaluation{T}" /> that supplies the squared <see cref="EnsembleDisplacement" /> set
        /// </summary>
        public IJobEvaluation<IReadOnlyList<EnsembleDisplacement>> SquaredDisplacementEvaluation { get; set; }

        /// <inheritdoc />
        public EnsembleDiffusionEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override IReadOnlyList<EnsembleDiffusion> GetValue(JobContext jobContext)
        {
            var r2displacements = SquaredDisplacementEvaluation[jobContext.DataId];

            var time = jobContext.McsReader.ReadMetaData().SimulatedTime;
            var result = new List<EnsembleDiffusion>(r2displacements.Count);

            foreach (var displacement in r2displacements.Select(x => x.AsMean()))
            {
                if (!displacement.IsSquared) throw new InvalidOperationException("Displacement data is not squared.");
                var msdComponents = (displacement.VectorR.X, displacement.VectorR.Y, displacement.VectorR.Z, displacement.DisplacementR);
                result.Add(new EnsembleDiffusion(displacement.Particle, displacement.EnsembleSize, time, msdComponents));
            }

            return result.AsReadOnly();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            SquaredDisplacementEvaluation ??= new SquaredDisplacementEvaluation(JobSet);

            if (!SquaredDisplacementEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("Squared ensemble displacement evaluation is not compatible.");
            base.PrepareForExecution();
        }
    }
}