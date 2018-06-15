using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Basic implementation of the energy query manager that handles safe data queries and service requests to the energy manager from outside sources
    /// </summary>
    internal class EnergyQueryManager : ModelQueryManager<EnergyModelData, IEnergyDataPort, EnergyDataCache, IEnergyCachePort>, IEnergyQueryPort
    {
        /// <summary>
        /// Create new energy query manager from data, cache object and data access locker
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="extendedData"></param>
        /// <param name="dataLocker"></param>
        public EnergyQueryManager(EnergyModelData baseData, EnergyDataCache extendedData, DataAccessLocker dataLocker)
            : base(baseData, extendedData, dataLocker)
        {

        }
    }
}
