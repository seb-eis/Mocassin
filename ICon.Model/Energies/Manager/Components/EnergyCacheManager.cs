using System.Collections.Generic;
using System.Linq;
using ICon.Mathematics.Constraints;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <inheritdoc cref="ICon.Model.Energies.IEnergyCachePort" />
    internal class EnergyCacheManager : ModelCacheManager<EnergyModelCache, IEnergyCachePort>, IEnergyCachePort
    {
        /// <inheritdoc />
        public EnergyCacheManager(EnergyModelCache modelCache, IProjectServices projectServices)
            : base(modelCache, projectServices)
        {
        }

        /// <inheritdoc />
        public IPairInteractionFinder GetPairInteractionFinder()
        {
            return GetResultFromCache(CreatePairInteractionFinder);
        }

        /// <inheritdoc />
        public IPositionGroupInfo GetPositionGroupInfo(int index)
        {
            return GetPositionGroupInfos()[index];
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<IUnitCellPosition, IReadOnlyList<IPairInteraction>> GetPositionPairInteractions()
        {
            return GetResultFromCache(CreatePositionPairInteractions);
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<IUnitCellPosition, IReadOnlyList<IGroupInteraction>> GetPositionGroupInteractions()
        {
            return GetResultFromCache(CreatePositionGroupInteractions);
        }

        /// <inheritdoc />
        public IReadOnlyList<IPositionGroupInfo> GetPositionGroupInfos()
        {
            return GetResultFromCache(CreateAllPositionGroupInfos);
        }

        /// <inheritdoc />
        public IEnergySetterProvider GetFullEnergySetterProvider()
        {
            return GetResultFromCache(CreateEnergySetterProvider);
        }

        /// <inheritdoc />
        public IReadOnlyList<IPairEnergySetter> GetStablePairEnergySetters()
        {
            return GetResultFromCache(CreateStablePairEnergySetters);
        }

        /// <inheritdoc />
        public IPairEnergySetter GetStablePairEnergySetter(int index)
        {
            return GetStablePairEnergySetters()[index];
        }

        /// <inheritdoc />
        public IReadOnlyList<IPairEnergySetter> GetUnstablePairEnergySetters()
        {
            return GetResultFromCache(CreateUnstablePairEnergySetters);
        }

        /// <inheritdoc />
        public IPairEnergySetter GetUnstablePairEnergySetter(int index)
        {
            return GetUnstablePairEnergySetters()[index];
        }

        /// <inheritdoc />
        public IReadOnlyList<IGroupEnergySetter> GetGroupEnergySetters()
        {
            return GetResultFromCache(CreateGroupEnergySetters);
        }

        /// <inheritdoc />
        public IGroupEnergySetter GetGroupEnergySetter(int index)
        {
            return GetGroupEnergySetters()[index];
        }

        /// <summary>
        ///     Pulls energy setter provider from data manager and creates all energy setters for stable pair interactions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IReadOnlyList<IPairEnergySetter> CreateStablePairEnergySetters()
        {
            return GetFullEnergySetterProvider().GetStablePairEnergySetters();
        }

        /// <summary>
        ///     Pulls energy setter provider from data manager and creates all energy setters for unstable pair interactions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IReadOnlyList<IPairEnergySetter> CreateUnstablePairEnergySetters()
        {
            return GetFullEnergySetterProvider().GetUnstablePairEnergySetters();
        }

        /// <summary>
        ///     Pulls energy setter provider from data manager and creates all energy setters for group interactions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IReadOnlyList<IGroupEnergySetter> CreateGroupEnergySetters()
        {
            return GetFullEnergySetterProvider().GetGroupEnergySetters();
        }

        /// <summary>
        ///     Pulls energy setter provider from data and sets the value constraints according to the project service settings
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IEnergySetterProvider CreateEnergySetterProvider()
        {
            var provider = ProjectServices.GetManager<IEnergyManager>().QueryPort.Query(port => port.GetEnergySetterProvider());

            var (pairMin, pairMax) = ProjectServices.SettingsData.EnergySettings.PairEnergies.GetMinMaxTuple();
            var (groupMin, groupMax) = ProjectServices.SettingsData.EnergySettings.GroupEnergies.GetMinMaxTuple();

            provider.PairEnergyConstraint =
                new NumericConstraint(true, pairMin, pairMax, true, ProjectServices.CommonNumeric.RangeComparer);

            provider.GroupEnergyConstraint =
                new NumericConstraint(true, groupMin, groupMax, true, ProjectServices.CommonNumeric.RangeComparer);

            return provider;
        }

        /// <summary>
        ///     Creates the pair interaction finder for the currently linked structure definition and space group service
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IPairInteractionFinder CreatePairInteractionFinder()
        {
            var unitCellProvider = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            return new PairInteractionFinder(unitCellProvider, ProjectServices.SpaceGroupService);
        }

        /// <summary>
        ///     Creates the position group information for all group interactions that are not marked as deprecated. Deprecated
        ///     ones are null
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected List<IPositionGroupInfo> CreateAllPositionGroupInfos()
        {
            var ucProvider = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var analyzer = new GeometryGroupAnalyzer(ucProvider, ProjectServices.SpaceGroupService);
            var interactions = ProjectServices.GetManager<IEnergyManager>().QueryPort.Query(port => port.GetGroupInteractions());

            return analyzer.CreateExtendedPositionGroups(interactions)
                .Select(value => (IPositionGroupInfo) new PositionGroupInfo(value)).ToList();
        }

        /// <summary>
        ///     Creates a dictionary that assigns each unit cell position its set of defined pair interactions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IReadOnlyDictionary<IUnitCellPosition, IReadOnlyList<IPairInteraction>> CreatePositionPairInteractions()
        {
            var manager = ProjectServices.GetManager<IEnergyManager>();
            var symmetricPairs = manager.QueryPort.Query(port => port.GetStablePairInteractions());
            var asymmetricPairs = manager.QueryPort.Query(port => port.GetUnstablePairInteractions());
            var symmetricResult = AssignPairInteractionsToPosition(symmetricPairs);
            var asymmetricResult = AssignPairInteractionsToPosition(asymmetricPairs);

            return symmetricResult
                .Concat(asymmetricResult)
                .ToDictionary(item => item.Key, item => (IReadOnlyList<IPairInteraction>) item.Value);
        }

        /// <summary>
        ///     Assigns a set of pair interactions to their unit cell positions. Asymmetric pairs are assigned to their first
        ///     position only
        /// </summary>
        /// <returns></returns>
        protected IDictionary<IUnitCellPosition, List<IPairInteraction>> AssignPairInteractionsToPosition<T>(IEnumerable<T> pairInteractions) 
            where T : IPairInteraction
        {
            var localResult = new Dictionary<IUnitCellPosition, List<IPairInteraction>>();

            foreach (var pairInteraction in pairInteractions)
            {
                if (!localResult.TryGetValue(pairInteraction.Position0, out var pairList0))
                {
                    pairList0 = new List<IPairInteraction>();
                    localResult.Add(pairInteraction.Position0, pairList0);
                }

                pairList0.Add(pairInteraction);

                if (pairInteraction is IAsymmetricPairInteraction || pairInteraction.Position0 == pairInteraction.Position1)
                    continue;

                if (!localResult.TryGetValue(pairInteraction.Position1, out var pairList1))
                {
                    pairList1 = new List<IPairInteraction>();
                    localResult.Add(pairInteraction.Position1, pairList1);
                }

                pairList1.Add(pairInteraction);
            }

            return localResult;
        }

        /// <summary>
        ///     Creates a dictionary that assigns each unit cell position its set of defined group interactions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IReadOnlyDictionary<IUnitCellPosition, IReadOnlyList<IGroupInteraction>> CreatePositionGroupInteractions()
        {
            var manager = ProjectServices.GetManager<IEnergyManager>();
            var groupInteractions = manager.QueryPort.Query(port => port.GetGroupInteractions());
            var localResult = new Dictionary<IUnitCellPosition, List<IGroupInteraction>>();
            foreach (var groupInteraction in groupInteractions)
            {
                if (!localResult.TryGetValue(groupInteraction.CenterUnitCellPosition, out var list))
                {
                    list = new List<IGroupInteraction>();
                    localResult.Add(groupInteraction.CenterUnitCellPosition, list);
                }

                list.Add(groupInteraction);
            }

            return localResult.ToDictionary(item => item.Key, item => (IReadOnlyList<IGroupInteraction>) item.Value);
        }
    }
}