using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Basic particle query manager that provides concurrency safe queries and readers for the particle data
    /// </summary>
    internal class ParticleQueryManager : ModelQueryManager<ParticleModelData, IParticleDataPort, ParticleDataCache, IParticleCachePort>, IParticleQueryPort
    {
        /// <summary>
        /// Creates new particle query manager for the provided base data and cache data using the passed data access locker
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="cacheData"></param>
        /// <param name="dataLocker"></param>
        public ParticleQueryManager(ParticleModelData baseData, ParticleDataCache cacheData, DataAccessLocker dataLocker) : base(baseData, cacheData, dataLocker)
        {

        }
    }
}
