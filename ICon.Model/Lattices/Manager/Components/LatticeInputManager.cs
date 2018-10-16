using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Basic implementation of the lattice input manager that handles validated adding, removal and replacement of lattice base data by an outside source
    /// </summary>
    internal class LatticeInputManager : ModelInputManager<LatticeModelData, ILatticeDataPort, LatticeEventManager>, ILatticeInputPort
    {
        /// <summary>
        /// Create new lattice input manager from data object, event manager and project services
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        /// <param name="services"></param>
        public LatticeInputManager(LatticeModelData data, LatticeEventManager manager, IProjectServices services) : base(data, manager, services)
        {

        }

        /// <summary>
        /// Get the lattice conflict resolver provider that provides conflicts resolvers for internal data conflicts in this manager
        /// </summary>
        /// <returns></returns>
        protected override IDataConflictHandlerProvider<LatticeModelData> CreateDataConflictHandlerProvider()
        {
            return new LatticeDataConflictResolverProvider(ProjectServices);
        }

        /// <summary>
        /// Tries to clean deprecated data by removing deprecated model objects and reindexing the model object lists. Distributes affiliated eventy on operation success
        /// </summary>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData()
        {
            return DefaultCleanDeprecatedData();
        }

        /// <summary>
        /// Tries to set new lattice info if it passes validation (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="latticeInfo"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ParameterChange)]
        protected IOperationReport TrySetLatticeInfo(ILatticeInfo latticeInfo)
        {
            var result = DefaultSetModelParameter(latticeInfo, accessor => accessor.Query(data => data.LatticeInfo), true);
            return result;
        }

        /// <summary>
        /// Registers a new BuildingBlock to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewBuildingBlock(IBuildingBlock buildingBlock)
        {
            var result = DefaultRegisterModelObject(buildingBlock, accessor => accessor.Query(data => data.BuildingBlocks));
            return result;
        }

        /// <summary>
        /// Removes a BuildingBlock from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveBuildingBlock(IBuildingBlock buildingBlock)
        {
            return DefaultRemoveModelObject(buildingBlock, accessor => accessor.Query(data => data.BuildingBlocks), 0);
        }

        /// <summary>
        /// Replaces a BuildingBlock in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgBuildingBlock"></param>
        /// <param name="newBuildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceBuildingBlock(IBuildingBlock orgBuildingBlock, IBuildingBlock newBuildingBlock)
        {
            return DefaultReplaceModelObject(orgBuildingBlock, newBuildingBlock, accessor => accessor.Query(data => data.BuildingBlocks));
        }

        /// <summary>
        /// Registers a new BlockInfo to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewBlockInfo(IBlockInfo blockInfo)
        {
            var result = DefaultRegisterModelObject(blockInfo, accessor => accessor.Query(data => data.BlockInfos));
            return result;
        }

        /// <summary>
        /// Removes a BlockInfo from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveBlockInfo(IBlockInfo blockInfo)
        {
            return DefaultRemoveModelObject(blockInfo, accessor => accessor.Query(data => data.BlockInfos), 0);
        }

        /// <summary>
        /// Replaces a BlockInfo in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgBuildingBlock"></param>
        /// <param name="newBuildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceBlockInfo(IBlockInfo orgBlockInfo, IBlockInfo newBlockInfo)
        {
            return DefaultReplaceModelObject(orgBlockInfo, newBlockInfo, accessor => accessor.Query(data => data.BlockInfos));
        }

        /// <summary>
        /// Registers a new Doping to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewDopingCombination(IDopingCombination dopingCombination)
        {
            var result = DefaultRegisterModelObject(dopingCombination, accessor => accessor.Query(data => data.DopingCombinations));
            return result;
        }

        /// <summary>
        /// Removes a Doping from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveDopingCombination(IDopingCombination dopingCombination)
        {
            return DefaultRemoveModelObject(dopingCombination, accessor => accessor.Query(data => data.DopingCombinations), 0);
        }

        /// <summary>
        /// Replaces a Doping in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgBuildingBlock"></param>
        /// <param name="newBuildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceDopingCombination(IDopingCombination orgDopingCombination, IDopingCombination newDopingCombination)
        {
            return DefaultReplaceModelObject(orgDopingCombination, newDopingCombination, accessor => accessor.Query(data => data.DopingCombinations));
        }

        /// <summary>
        /// Registers a new Doping to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewDoping(IDoping doping)
        {
            var result =  DefaultRegisterModelObject(doping, accessor => accessor.Query(data => data.Dopings));
            return result;
        }

        /// <summary>
        /// Removes a Doping from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveDoping(IDoping doping)
        {
            return DefaultRemoveModelObject(doping, accessor => accessor.Query(data => data.Dopings), 0);
        }

        /// <summary>
        /// Replaces a Doping in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgBuildingBlock"></param>
        /// <param name="newBuildingBlock"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceDoping(IDoping orgDoping, IDoping newDoping)
        {
            return DefaultReplaceModelObject(orgDoping, newDoping, accessor => accessor.Query(data => data.Dopings));
        }
    }

}