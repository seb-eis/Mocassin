using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Cache for extended structure data that stores 'on-demand' calculated dependent data for faster access until the
    ///     data is no longer valid
    /// </summary>
    internal class ParticleModelCache : ModelDataCache<IParticleCachePort>
    {
        /// <inheritdoc />
        public ParticleModelCache(IModelEventPort eventPort, IModelProject modelProject)
            : base(eventPort, modelProject)
        {
        }

        /// <inheritdoc />
        public override IParticleCachePort AsReadOnly()
        {
            return CachePort ?? (CachePort = new ParticleCacheManager(this, ModelProject));
        }
    }
}