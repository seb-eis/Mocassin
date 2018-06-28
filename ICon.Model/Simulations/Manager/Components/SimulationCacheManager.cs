using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations
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
        /// <param name="projectServices"></param>
        public SimulationCacheManager(SimulationDataCache dataCache, IProjectServices projectServices) : base(dataCache, projectServices)
        {

        }
    }
}
