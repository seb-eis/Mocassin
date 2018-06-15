using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Data cache for the extended on-demand energy model data
    /// </summary>
    internal class EnergyDataCache : DynamicModelDataCache<IEnergyCachePort>
    {
        /// <summary>
        /// Creates new energy data cache with the provided event port and project services
        /// </summary>
        /// <param name="eventPort"></param>
        /// <param name="projectServices"></param>
        public EnergyDataCache(IModelEventPort eventPort, IProjectServices projectServices) : base(eventPort, projectServices)
        {

        }

        /// <summary>
        /// Creates a read only interface for the energy data cache
        /// </summary>
        /// <returns></returns>
        public override IEnergyCachePort AsReadOnly()
        {
            if (CachePort == null)
            {
                CachePort = new EnergyCacheManager(this, ProjectServices);
            }
            return CachePort;
        }
    }
}
