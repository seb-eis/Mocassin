using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IEnergyCachePort" />
    internal class EnergyCacheManager : ModelCacheManager<EnergyModelCache, IEnergyCachePort>, IEnergyCachePort
    {
        /// <inheritdoc />
        public EnergyCacheManager(EnergyModelCache modelCache, IModelProject modelProject)
            : base(modelCache, modelProject)
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
        public IReadOnlyDictionary<ICellSite, IReadOnlyList<IPairInteraction>> GetPositionPairInteractions()
        {
            return GetResultFromCache(CreatePositionPairInteractions);
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<ICellSite, IReadOnlyList<IGroupInteraction>> GetPositionGroupInteractions()
        {
            return GetResultFromCache(CreatePositionGroupInteractions);
        }

        /// <inheritdoc />
        public IReadOnlyList<IPositionGroupInfo> GetPositionGroupInfos()
        {
            return GetResultFromCache(CreateAllPositionGroupInfos);
        }

        /// <inheritdoc />
        public IEnergySetterProvider GetEnergySetterProvider()
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
            return GetEnergySetterProvider().GetStablePairEnergySetters();
        }

        /// <summary>
        ///     Pulls energy setter provider from data manager and creates all energy setters for unstable pair interactions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IReadOnlyList<IPairEnergySetter> CreateUnstablePairEnergySetters()
        {
            return GetEnergySetterProvider().GetUnstablePairEnergySetters();
        }

        /// <summary>
        ///     Pulls energy setter provider from data manager and creates all energy setters for group interactions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IReadOnlyList<IGroupEnergySetter> CreateGroupEnergySetters()
        {
            return GetEnergySetterProvider().GetGroupEnergySetters();
        }

        /// <summary>
        ///     Pulls energy setter provider from data and sets the value constraints according to the project service settings
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IEnergySetterProvider CreateEnergySetterProvider()
        {
            var queryPort = ModelProject.Manager<IEnergyManager>().DataAccess;
            var provider = queryPort.Query(port => port.GetEnergySetterProvider(ModelProject.Settings, queryPort));
            return provider;
        }

        /// <summary>
        ///     Creates the pair interaction finder for the currently linked structure definition and space group service
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IPairInteractionFinder CreatePairInteractionFinder()
        {
            var unitCellProvider = ModelProject.Manager<IStructureManager>().DataAccess.Query(port => port.GetFullUnitCellProvider());
            return new PairInteractionFinder(unitCellProvider, ModelProject.SpaceGroupService);
        }

        /// <summary>
        ///     Creates the position group information for all group interactions that are not marked as deprecated. Deprecated
        ///     ones are null
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected List<IPositionGroupInfo> CreateAllPositionGroupInfos()
        {
            var ucProvider = ModelProject.Manager<IStructureManager>().DataAccess.Query(port => port.GetFullUnitCellProvider());
            var analyzer = new GeometryGroupAnalyzer(ucProvider, ModelProject.SpaceGroupService);
            var interactions = ModelProject.Manager<IEnergyManager>().DataAccess.Query(port => port.GetGroupInteractions());

            var result = analyzer.CreateExtendedPositionGroups(interactions)
                .Select(value => (IPositionGroupInfo) new PositionGroupInfo(value))
                .ToList();

            result.ForEach(x => x.SynchronizeEnergyDictionary());
            return result;
        }

        /// <summary>
        ///     Creates a dictionary that assigns each unit cell position its set of defined pair interactions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IReadOnlyDictionary<ICellSite, IReadOnlyList<IPairInteraction>> CreatePositionPairInteractions()
        {
            var energyManager = ModelProject.Manager<IEnergyManager>();
            var structureManager = ModelProject.Manager<IStructureManager>();
            var symmetricPairs = energyManager.DataAccess.Query(port => port.GetStablePairInteractions());
            var asymmetricPairs = energyManager.DataAccess.Query(port => port.GetUnstablePairInteractions());
            var symmetricResult = AssignPairInteractionsToPosition(symmetricPairs);
            var asymmetricResult = AssignPairInteractionsToPosition(asymmetricPairs);

            var result = symmetricResult
                .Concat(asymmetricResult)
                .ToDictionary(item => item.Key, item => (IReadOnlyList<IPairInteraction>) item.Value);

            foreach (var referencePosition in structureManager.DataAccess.Query(port => port.GetCellReferencePositions()))
                if (!result.ContainsKey(referencePosition))
                    result.Add(referencePosition, new List<IPairInteraction>());

            return result;
        }

        /// <summary>
        ///     Assigns a set of pair interactions to their unit cell positions. Asymmetric pairs are assigned to their first
        ///     position only
        /// </summary>
        /// <returns></returns>
        protected IDictionary<ICellSite, List<IPairInteraction>> AssignPairInteractionsToPosition<T>(
            IEnumerable<T> pairInteractions)
            where T : IPairInteraction
        {
            var localResult = new Dictionary<ICellSite, List<IPairInteraction>>();

            foreach (var pairInteraction in pairInteractions)
            {
                if (!localResult.TryGetValue(pairInteraction.Position0, out var pairList0))
                {
                    pairList0 = new List<IPairInteraction>();
                    localResult.Add(pairInteraction.Position0, pairList0);
                }

                pairList0.Add(pairInteraction);

                if (pairInteraction is IUnstablePairInteraction || pairInteraction.Position0 == pairInteraction.Position1)
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
        protected IReadOnlyDictionary<ICellSite, IReadOnlyList<IGroupInteraction>> CreatePositionGroupInteractions()
        {
            var manager = ModelProject.Manager<IEnergyManager>();
            var groupInteractions = manager.DataAccess.Query(port => port.GetGroupInteractions());
            var localResult = new Dictionary<ICellSite, List<IGroupInteraction>>();
            foreach (var groupInteraction in groupInteractions)
            {
                if (!localResult.TryGetValue(groupInteraction.CenterCellSite, out var list))
                {
                    list = new List<IGroupInteraction>();
                    localResult.Add(groupInteraction.CenterCellSite, list);
                }

                list.Add(groupInteraction);
            }

            return localResult.ToDictionary(item => item.Key, item => (IReadOnlyList<IGroupInteraction>) item.Value);
        }
    }
}