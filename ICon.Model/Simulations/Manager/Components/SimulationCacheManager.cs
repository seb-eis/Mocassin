using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Implementation of the simulation cache manager that handles creation and supply of on-demand simulation data objects
    /// </summary>
    internal class SimulationCacheManager : ModelCacheManager<SimulationDataCache, ISimulationCachePort>, ISimulationCachePort
    {
        /// <summary>
        /// Crate new simulation cache manager using the provided data cache and project services
        /// </summary>
        /// <param name="dataCache"></param>
        /// <param name="modelProject"></param>
        public SimulationCacheManager(SimulationDataCache dataCache, IModelProject modelProject) : base(dataCache, modelProject)
        {

        }

        /// <summary>
        /// Get a list of the mobile particle infos for all kinetic simulations on all unit cell positions
        /// </summary>
        /// <returns></returns>
        public IList<IList<IParticleSet>> GetKineticMobileParticleSets()
        {
            return GetResultFromCache(CreateKineticMobileParticleSets);
        }

        /// <summary>
        /// Get a list of the mobile particle infos for all kinetic simulations on all unit cell positions
        /// </summary>
        /// <returns></returns>
        public IList<IList<IParticleSet>> GetMetropolisMobileParticleSets()
        {
            return GetResultFromCache(CreateMetropolisMobileParticleSets);
        }

        [CacheMethodResult]
        protected IList<IList<IParticleSet>> CreateKineticMobileParticleSets()
        {
            return null;
        }

        [CacheMethodResult]
        protected IList<IList<IParticleSet>> CreateMetropolisMobileParticleSets()
        {
            return null;
        }
    }
}
