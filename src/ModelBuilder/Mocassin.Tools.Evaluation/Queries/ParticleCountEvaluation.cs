using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections.Mocassin.Tools.Evaluation.Queries;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the counts of <see cref="IParticle" /> instances in a <see cref="JobContext" /> lattice
    /// </summary>
    public class ParticleCountEvaluation : JobEvaluation<ReadOnlyList<int>>
    {
        /// <inheritdoc />
        public ParticleCountEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override ReadOnlyList<int> GetValue(JobContext context)
        {
            var counts = new int[context.ModelContext.GetModelObjects<IParticle>().Count()];
            foreach (var id in context.McsReader.ReadLattice()) counts[id]++;

            return counts.ToList().AsReadOnlyList();
        }
    }
}