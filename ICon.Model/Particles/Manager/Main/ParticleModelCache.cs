using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles
{
    /// <summary>
    ///     Cache for extended structure data that stores 'on-demand' calculated dependent data for faster access until the
    ///     data is no longer valid
    /// </summary>
    internal class ParticleModelCache : ModelDataCache<IParticleCachePort>
    {
        /// <inheritdoc />
        public ParticleModelCache(IModelEventPort eventPort, IProjectServices projectServices)
            : base(eventPort, projectServices)
        {
        }

        /// <inheritdoc />
        public override IParticleCachePort AsReadOnly()
        {
            return CachePort ?? (CachePort = new ParticleCacheManager(this, ProjectServices));
        }
    }
}