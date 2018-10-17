﻿using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures
{
    /// <summary>
    /// Cache for extended structure data that stores 'on-demand' calculated dependent data for faster access until the data is no longer valid
    /// </summary>
    internal class StructureDataCache : ModelDataCache<IStructureCachePort>
    {
        /// <summary>
        /// Creates new cached structure data object with empty cache list and registers to the basic event of the provided event port
        /// </summary>
        public StructureDataCache(IModelEventPort eventPort, IModelProject modelProject) : base(eventPort, modelProject)
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
                CachePort = new StructureCacheManager(this, ModelProject);
            }
            return CachePort;
        }
    }
}
