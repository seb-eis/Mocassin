using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Cache for extended structure data that stores 'on-demand' calculated dependent data for faster access until the data is no longer valid
    /// </summary>
    internal class ParticleDataCache : DynamicModelDataCache<IParticleCachePort>
    {
        /// <summary>
        /// Creates new cached particle data object with empty cache list and registers to the basic event of the provided event port
        /// </summary>
        public ParticleDataCache(IModelEventPort eventPort, IProjectServices projectServices) : base(eventPort, projectServices)
        {

        }

        /// <summary>
        /// Returns a read only interface for the cache (Supplied from cache after first creation)
        /// </summary>
        /// <returns></returns>
        public override IParticleCachePort AsReadOnly()
        {
            if (CachePort == null)
            {
                CachePort = new ParticleCacheManager(this, ProjectServices);
            }
            return CachePort;
        }
    }
}
