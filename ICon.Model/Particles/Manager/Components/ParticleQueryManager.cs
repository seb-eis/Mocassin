using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Basic particle query manager that provides concurrency safe queries and readers for the particle data
    /// </summary>
    internal class ParticleQueryManager : ModelQueryManager<ParticleModelData, IParticleDataPort, ParticleModelCache, IParticleCachePort>,
        IParticleQueryPort
    {
        /// <inheritdoc />
        public ParticleQueryManager(ParticleModelData baseData, ParticleModelCache cacheModel, AccessLockSource lockSource)
            : base(baseData, cacheModel, lockSource)
        {
        }
    }
}