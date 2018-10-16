using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions
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
        /// <param name="projectServices"></param>
        public TransitionDataCache(IModelEventPort eventPort, IProjectServices projectServices) : base(eventPort, projectServices)
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
                CachePort = new TransitionCacheManager(this, ProjectServices);
            }
            return CachePort;
        }
    }
}
