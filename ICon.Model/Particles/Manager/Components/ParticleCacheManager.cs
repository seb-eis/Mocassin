using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ICon.Mathematics.Permutation;
using ICon.Framework.Collections;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles
{
    /// <summary>
    /// BAsic implementation of the particle data cache manager that provides access to on-demand extended model data
    /// </summary>
    internal class ParticleCacheManager : ModelCacheManager<ParticleDataCache, IParticleCachePort>, IParticleCachePort
    {
        /// <summary>
        /// Create new cache manager for data cache object with the provided project services
        /// </summary>
        /// <param name="dataCache"></param>
        /// <param name="projectServices"></param>
        public ParticleCacheManager(ParticleDataCache dataCache, IProjectServices projectServices) : base(dataCache, projectServices)
        {

        }

        /// <summary>
        /// Get an enumerable sequence of all possible pair codes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PairCode> GetPossiblePairCodes()
        {
            return GetResultFromCache(CalculatePossiblePairCodes).AsEnumerable();
        }

        /// <summary>
        /// Creates a unqie sorted list of possible pair codes
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected SetList<PairCode> CalculatePossiblePairCodes()
        {
            var particles = ProjectServices.GetManager<IParticleManager>().QueryPort
                .Query(port => port.GetParticles())
                .Where(particle => !particle.IsDeprecated)
                .Select(particle => particle.Index);

            var permuter = new SlotMachinePermuter<int>(particles, particles);

            return new SetList<PairCode>(Comparer<PairCode>.Default, 25)
            {
                permuter.Select((value) => new PairCode(value[0], value[1]))
            };
        }
    }
}
