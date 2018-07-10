using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// The simulation manager data cache object that stores and supplies on-demand simulation data objects
    /// </summary>
    public class SimulationDataCache : DynamicModelDataCache<ISimulationCachePort>
    {
        /// <summary>
        /// Create new simulation data cache that uses the provided event port and project services
        /// </summary>
        /// <param name="eventPort"></param>
        /// <param name="projectServices"></param>
        public SimulationDataCache(IModelEventPort eventPort, IProjectServices projectServices) : base(eventPort, projectServices)
        {
        }

        /// <summary>
        /// Get an access port for this simulation data cache object
        /// </summary>
        /// <returns></returns>
        public override ISimulationCachePort AsReadOnly()
        {
            if (CachePort == null)
            {
                CachePort = new SimulationCacheManager(this, ProjectServices);
            }
            return CachePort;
        }
    }
}
