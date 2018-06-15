using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Cache for extended structure data that stores 'on-demand' calculated dependent data for faster access until the data is no longer valid
    /// </summary>
    internal class StructureDataCache : DynamicModelDataCache<IStructureCachePort>
    {
        /// <summary>
        /// Creates new cached structure data object with empty cache list and registers to the basic event of the provided event port
        /// </summary>
        public StructureDataCache(IModelEventPort eventPort, IProjectServices projectServices) : base(eventPort, projectServices)
        {

        }

        /// <summary>
        /// Returns a read only interface for the cache (Supplied from cache after first creation)
        /// </summary>
        /// <returns></returns>
        public override IStructureCachePort AsReadOnly()
        {
            if (CachePort == null)
            {
                CachePort = new StructureCacheManager(this, ProjectServices);
            }
            return CachePort;
        }
    }
}
