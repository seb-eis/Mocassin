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
    public class SimulationModelCache : ModelDataCache<ISimulationCachePort>
    {
        /// <inheritdoc />
        public SimulationModelCache(IModelEventPort eventPort, IModelProject modelProject) : base(eventPort, modelProject)
        {
        }

        /// <inheritdoc />
        public override ISimulationCachePort AsReadOnly()
        {
            return CachePort ?? (CachePort = new SimulationCacheManager(this, ModelProject));
        }
    }
}
