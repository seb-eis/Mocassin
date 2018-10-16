using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Basic implementation of the lattice query manager that handles safe data queries and service requests to the Lattice manager from outside sources
    /// </summary>
    internal class LatticeQueryManager : ModelQueryManager<LatticeModelData, ILatticeDataPort, LatticeDataCache, ILatticeCachePort>, ILatticeQueryPort
    {
        /// <summary>
        /// Create new lattice query manager from data, cache object and data access locker
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="extendedData"></param>
        /// <param name="lockSource"></param>
        public LatticeQueryManager(LatticeModelData baseData, LatticeDataCache extendedData, AccessLockSource lockSource) : base(baseData, extendedData, lockSource)
        {
        }
    }
}
