using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.Constraints;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Basic implementation of the energy cache manager that provides read only access to the extended 'on demand' energy data
    /// </summary>
    internal class EnergyCacheManager : ModelCacheManager<EnergyDataCache, IEnergyCachePort>, IEnergyCachePort
    {
        /// <summary>
        /// Create new energy cache manager for the provided data cache and project services
        /// </summary>
        /// <param name="dataCache"></param>
        /// <param name="projectServices"></param>
        public EnergyCacheManager(EnergyDataCache dataCache, IProjectServices projectServices) : base(dataCache, projectServices)
        {

        }

        /// <summary>
        /// Get a pair interaction finder that can be used to search the currently linked structure system for symmetric and asymmetric interactions
        /// </summary>
        /// <returns></returns>
        public IPairInteractionFinder GetPairInteractionFinder()
        {
            return AccessCacheableDataEntry(CreatePairInteractionFinder);
        }

        /// <summary>
        /// Creates the pair interaction finder for the currently linked structure definition and space group service
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IPairInteractionFinder CreatePairInteractionFinder()
        {
            var unitCellProvider = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            return new PairInteractionFinder(unitCellProvider, ProjectServices.SpaceGroupService);
        }
    }
}
