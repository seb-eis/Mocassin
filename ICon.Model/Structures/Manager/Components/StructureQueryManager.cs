using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Basic implementation of the structure query manager that handles safe data queries and service requests to the structure manager from outside sources
    /// </summary>
    internal class StructureQueryManager : ModelQueryManager<StructureModelData, IStructureDataPort, StructureDataCache, IStructureCachePort>, IStructureQueryPort
    {
        /// <summary>
        /// Creates new query manager for the provided structure data objects using the defined data access locker
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="cacheData"></param>
        /// <param name="lockSource"></param>
        public StructureQueryManager(StructureModelData baseData, StructureDataCache cacheData, AccessLockSource lockSource) : base(baseData, cacheData, lockSource)
        {

        }
    }
}
