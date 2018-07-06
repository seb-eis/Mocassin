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
        /// Get the position group info that belongs to the interaction group at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPositionGroupInfo GetPositionGroupInfo(int index)
        {
            return GetPositionGroupInfos()[index];
        }

        /// <summary>
        /// Get a read only list of all position group infos for all defined group interactions
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IPositionGroupInfo> GetPositionGroupInfos()
        {
            return AccessCacheableDataEntry(CreateAllPositionGroupInfos);
        }

        /// <summary>
        /// Get an energy setter provider that has the value constraints set to their porject settings defined values
        /// </summary>
        /// <returns></returns>
        public IEnergySetterProvider GetFullEnergySetterProvider()
        {
            return AccessCacheableDataEntry(CreateEnergySetterProvider);
        }

        /// <summary>
        /// Get a read only list of all stable pair interaction energy setters
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IPairEnergySetter> GetStablePairEnergySetters()
        {
            return AccessCacheableDataEntry(CreateStablePairEnergySetters);
        }

        /// <summary>
        /// Get the stable pair interaction energy setter for the interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPairEnergySetter GetStablePairEnergySetter(int index)
        {
            return GetStablePairEnergySetters()[index];
        }

        /// <summary>
        /// Get a read only list of all unstable pair interaction energy setters
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IPairEnergySetter> GetUnstablePairEnergySetters()
        {
            return AccessCacheableDataEntry(CreateUnstablePairEnergySetters);
        }

        /// <summary>
        /// Get the unstable pair interaction energy setter for the interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPairEnergySetter GetUnstablePairEnergySetter(int index)
        {
            return GetUnstablePairEnergySetters()[index];
        }

        /// <summary>
        /// Get a read only list of all energy setters for group interactions
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IGroupEnergySetter> GetGroupEnergySetters()
        {
            return AccessCacheableDataEntry(CreateGroupEnergySetters);
        }

        /// <summary>
        /// Get the group interaction energy setter for the group interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IGroupEnergySetter GetGroupEnergySetter(int index)
        {
            return GetGroupEnergySetters()[index];
        }

        /// <summary>
        /// Pulls energy setter provider from data manager and creates all energy setters for stable pair interactions
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IReadOnlyList<IPairEnergySetter> CreateStablePairEnergySetters()
        {
            return GetFullEnergySetterProvider().GetStablePairEnergySetters(); 
        }

        /// <summary>
        /// Pulls energy setter provider from data manager and creates all energy setters for unstable pair interactions
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IReadOnlyList<IPairEnergySetter> CreateUnstablePairEnergySetters()
        {
            return GetFullEnergySetterProvider().GetUnstablePairEnergySetters();
        }

        /// <summary>
        /// Pulls energy setter provider from data manager and creates all energy setters for group interactions
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IReadOnlyList<IGroupEnergySetter> CreateGroupEnergySetters()
        {
            return GetFullEnergySetterProvider().GetGroupEnergySetters();
        }

        /// <summary>
        /// Pulls energy setter provider from data and sets the valu constraints according to the project service settings
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IEnergySetterProvider CreateEnergySetterProvider()
        {
            var provider = ProjectServices.GetManager<IEnergyManager>().QueryPort.Query(port => port.GetEnergySetterProvider());

            var (pairMin, pairMax) = ProjectServices.SettingsData.EnergySettings.PairEnergies.GetMinMaxTuple();
            var (groupMin, groupMax) = ProjectServices.SettingsData.EnergySettings.GroupEnergies.GetMinMaxTuple();

            provider.PairEnergyConstraint = new DoubleConstraint(true, pairMin, pairMax, true, ProjectServices.CommonNumerics.RangeComparer);
            provider.GroupEnergyConstraint = new DoubleConstraint(true, groupMin, groupMax, true, ProjectServices.CommonNumerics.RangeComparer);
            return provider;
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

        /// <summary>
        /// Creates the position group infomations for all group interactions that are not marked as deprecated. Deprecated ones are null
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected List<IPositionGroupInfo> CreateAllPositionGroupInfos()
        {
            var ucProvider = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var analyzer = new GeometryGroupAnalyzer(ucProvider, ProjectServices.SpaceGroupService);
            var interactions = ProjectServices.GetManager<IEnergyManager>().QueryPort.Query(port => port.GetGroupInteractions());
            return analyzer.CreateExtendedPositionGroups(interactions).Select(value => (IPositionGroupInfo) new PositionGroupInfo(value)).ToList();
        }
    }
}
