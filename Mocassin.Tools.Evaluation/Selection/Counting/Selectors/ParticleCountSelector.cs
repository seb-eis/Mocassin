using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Selection.Counting.Selectors
{
    /// <summary>
    ///     Selector to extract the number of specific <see cref="IParticle" /> instances from a
    ///     <see cref="JobResultContext" />
    /// </summary>
    public class ParticleCountSelector : JobResultSelector<Dictionary<IParticle, int>>
    {
        /// <inheritdoc />
        public override Dictionary<IParticle, int> GetValue(JobResultContext resultContext)
        {
            var counts = CountParticles(resultContext);
            var index = 0;
            return counts.ToDictionary(x => resultContext.ModelContext.GetModelObject<IParticle>(index++), x => x);
        }

        /// <summary>
        ///     Counts the occurrences of all particles in the lattice
        /// </summary>
        /// <param name="resultContext"></param>
        /// <returns></returns>
        private int[] CountParticles(JobResultContext resultContext)
        {
            var counts = new int[resultContext.ModelContext.GetModelObjects<IParticle>().Count()];
            foreach (var id in resultContext.McsReader.GetLattice()) counts[id]++;

            return counts;
        }
    }
}