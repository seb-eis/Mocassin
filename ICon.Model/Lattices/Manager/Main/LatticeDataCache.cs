using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Data cache for the extended on-demand lattice model data
    /// </summary>
    internal class LatticeDataCache : ModelDataCache<ILatticeCachePort>
    {
        /// <summary>
        /// Creates new cached lattice data object with empty cache list and registers to the basic event of the provided event port
        /// </summary>
        public LatticeDataCache(IModelEventPort eventPort, IModelProject modelProject) : base(eventPort, modelProject)
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
                CachePort = new LatticeCacheManager(this, ModelProject);
            }
            return CachePort;
        }
    }
}
