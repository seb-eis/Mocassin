using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// The simulation manager data cache object that stores and supplies on-demand simulation data objects
    /// </summary>
    public class SimulationDataCache : ModelDataCache<ISimulationCachePort>
    {
        /// <summary>
        /// Create new simulation data cache that uses the provided event port and project services
        /// </summary>
        /// <param name="eventPort"></param>
        /// <param name="modelProject"></param>
        public SimulationDataCache(IModelEventPort eventPort, IModelProject modelProject) : base(eventPort, modelProject)
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
                CachePort = new SimulationCacheManager(this, ModelProject);
            }
            return CachePort;
        }
    }
}
