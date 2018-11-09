using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Implementation of the simulation cache manager that handles creation and supply of on-demand simulation data
    ///     objects
    /// </summary>
    internal class SimulationCacheManager : ModelCacheManager<SimulationModelCache, ISimulationCachePort>, ISimulationCachePort
    {
        /// <inheritdoc />
        public SimulationCacheManager(SimulationModelCache modelCache, IModelProject modelProject)
            : base(modelCache, modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IList<IParticleSet>> GetPositionBoundKineticMobileParticles()
        {
            return GetResultFromCache(CreateKineticMobileParticleSets);
        }

        /// <inheritdoc />
        public IList<IList<IParticleSet>> GetPositionBoundMetropolisMobileParticles()
        {
            return GetResultFromCache(CreateMetropolisMobileParticleSets);
        }

        /// <summary>
        /// Creates the 2D list interface with the kinetic mobile particle set for each transition and unit cell position combination
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<IList<IParticleSet>> CreateKineticMobileParticleSets()
        {
            return null;
        }

        /// <summary>
        /// Creates the 2D list interface with the metropolis mobile particle set for each transition and unit cell position combination
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<IList<IParticleSet>> CreateMetropolisMobileParticleSets()
        {
            return null;
        }
    }
}