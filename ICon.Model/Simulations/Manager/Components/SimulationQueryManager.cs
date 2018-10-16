using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Implementation of the simulation query manager that provides save query based access to the simulation model data and cache
    /// </summary>
    internal class SimulationQueryManager : ModelQueryManager<SimulationModelData, ISimulationDataPort, SimulationDataCache, ISimulationCachePort>, ISimulationQueryPort
    {
        /// <summary>
        /// Create new simulation query manager that can access the provided data and cache objects and uses the provided data locker for locking attempts
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="cacheData"></param>
        /// <param name="lockSource"></param>
        public SimulationQueryManager(SimulationModelData baseData, SimulationDataCache cacheData, AccessLockSource lockSource)
            : base(baseData, cacheData, lockSource)
        {
        }
    }
}
