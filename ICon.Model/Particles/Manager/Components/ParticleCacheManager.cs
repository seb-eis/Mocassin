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
    }
}