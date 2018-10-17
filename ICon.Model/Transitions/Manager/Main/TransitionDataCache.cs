using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    /// Data cache for the extended on-demand transition model data
    /// </summary>
    internal class TransitionDataCache : ModelDataCache<ITransitionCachePort>
    {
        /// <summary>
        /// Create a new data cache for extended transition data from model event port (Supplies expiration events) and project services
        /// </summary>
        /// <param name="eventPort"></param>
        /// <param name="modelProject"></param>
        public TransitionDataCache(IModelEventPort eventPort, IModelProject modelProject) : base(eventPort, modelProject)
        {

        }

        /// <summary>
        /// Get a read only port for the transition data cache
        /// </summary>
        /// <returns></returns>
        public override ITransitionCachePort AsReadOnly()
        {
            if (CachePort == null)
            {
                CachePort = new TransitionCacheManager(this, ModelProject);
            }
            return CachePort;
        }
    }
}
