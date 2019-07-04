using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Selection.Counting.Selectors
{
    /// <summary>
    ///     Counts the occurrences of al <see cref="IParticle"/> instances in a <see cref="JobContext"/>
    /// </summary>
    public class ParticleCountQuery : JobContextQuery<int[]>
    {
        /// <inheritdoc />
        public ParticleCountQuery(IEnumerable<JobContext> jobContexts)
            : base(jobContexts)
        {
        }

        /// <inheritdoc />
        public override int[] Execute(JobContext context)
        {
            var counts = new int[context.ModelContext.GetModelObjects<IParticle>().Count()];
            foreach (var id in context.McsReader.GetLattice()) counts[id]++;

            return counts;
        }
    }
}