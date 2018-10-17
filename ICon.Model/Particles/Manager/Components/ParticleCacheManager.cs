using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.Permutation;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Basic implementation of the particle data cache manager that provides access to on-demand extended model data
    /// </summary>
    internal class ParticleCacheManager : ModelCacheManager<ParticleModelCache, IParticleCachePort>, IParticleCachePort
    {
        /// <inheritdoc />
        public ParticleCacheManager(ParticleModelCache modelCache, IModelProject modelProject)
            : base(modelCache, modelProject)
        {
        }

        /// <inheritdoc />
        public IEnumerable<PairCode> GetPossiblePairCodes()
        {
            return GetResultFromCache(CalculatePossiblePairCodes).AsEnumerable();
        }

        /// <summary>
        ///     Creates a unique sorted list of possible pair codes
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected SetList<PairCode> CalculatePossiblePairCodes()
        {
            var particles = ModelProject.GetManager<IParticleManager>().QueryPort
                .Query(port => port.GetParticles())
                .Where(particle => !particle.IsDeprecated)
                .Select(particle => particle.Index)
                .ToList();

            var permutationSource = new PermutationSlotMachine<int>(particles, particles);

            return new SetList<PairCode>(Comparer<PairCode>.Default, 25)
            {
                permutationSource.Select(value => new PairCode(value[0], value[1]))
            };
        }
    }
}