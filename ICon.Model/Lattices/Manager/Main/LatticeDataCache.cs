using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Data cache for the extended on-demand lattice model data
    /// </summary>
    internal class LatticeDataCache : ModelDataCache<ILatticeCachePort>
    {
        /// <summary>
        /// Creates new cached lattice data object with empty cache list and registers to the basic event of the provided event port
        /// </summary>
        public LatticeDataCache(IModelEventPort eventPort, IProjectServices projectServices) : base(eventPort, projectServices)
        {
        }

        /// <summary>
        /// Returns a read only interface for the cache (Supplied from cache after first creation)
        /// </summary>
        /// <returns></returns>
        public override ILatticeCachePort AsReadOnly()
        {
            if (CachePort == null)
            {
                CachePort = new LatticeCacheManager(this, ProjectServices);
            }
            return CachePort;
        }
    }
}
