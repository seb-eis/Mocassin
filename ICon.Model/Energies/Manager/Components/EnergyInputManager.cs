﻿using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Basic implementation of the energy input manager that handles validated adding, removal and replacement of energy base data by an outside source
    /// </summary>
    internal class EnergyInputManager : ModelInputManager<EnergyModelData, IEnergyDataPort, EnergyEventManager>, IEnergyInputPort
    {
        /// <summary>
        /// Create new energy input manager from data object, event manager and project services
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        /// <param name="services"></param>
        public EnergyInputManager(EnergyModelData data, EnergyEventManager manager, IProjectServices services) : base(data, manager, services)
        {

        }

        /// <summary>
        /// Registers a new stable group info object with the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="newObject"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewStableGroupInfo(IGroupInteraction newObject)
        {
            return DefaultRegisterModelObject(newObject, accessor => accessor.Query(data => data.GroupInteractions));
        }

        /// <summary>
        /// Replaces an existing stable group info with a new one if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgObject"></param>
        /// <param name="newObject"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceStableGroupInfo(IGroupInteraction orgObject, IGroupInteraction newObject)
        {
            return DefaultReplaceModelObject(orgObject, newObject, accessor => accessor.Query(data => data.GroupInteractions));
        }

        /// <summary>
        /// Removes an existing stable group info by deprecating it within the indexed list (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgObject"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveStableGroupInfo(IGroupInteraction orgObject)
        {
            return DefaultRemoveModelObject(orgObject, accessor => accessor.Query(data => data.GroupInteractions));
        }

        /// <summary>
        /// Registers a new unstable environment info object with the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="newObject"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewUnstableEnvironmentInfo(IUnstableEnvironment newObject)
        {
            return DefaultRegisterModelObject(newObject, accessor => accessor.Query(data => data.UnstableEnvironmentInfos));
        }

        /// <summary>
        /// Replaces an existing unstable environment info with a new one if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgObject"></param>
        /// <param name="newObject"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceUnstableEnvironmentInfo(IUnstableEnvironment orgObject, IUnstableEnvironment newObject)
        {
            return DefaultReplaceModelObject(orgObject, newObject, accessor => accessor.Query(data => data.UnstableEnvironmentInfos));
        }

        /// <summary>
        /// Removes an existing unstable environment info by deprecating it within the indexed list (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgObject"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveUnstableEnvironmentInfo(IUnstableEnvironment orgObject)
        {
            return DefaultRemoveModelObject(orgObject, accessor => accessor.Query(data => data.UnstableEnvironmentInfos));
        }

        /// <summary>
        /// Replaces the currentlys set environment info parameter if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="newParameter"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ParameterChange)]
        protected IOperationReport TrySetEnvironmentInformation(IStableEnvironmentInfo newParameter)
        {
            return DefaultSetModelParameter(newParameter, accessor => accessor.Query(data => data.StableEnvironmentInfo), true);
        }


        /// <summary>
        /// Get the energy conflict resolver provider that provides conflicts resolvers for internal data conflicts in this manager
        /// </summary>
        /// <returns></returns>
        protected override IDataConflictHandlerProvider<EnergyModelData> MakeConflictHandlerProvider()
        {
            return new ConflictHandling.EnergyDataConflictHandlerProvider(ProjectServices);
        }

        /// <summary>
        /// Tries to clean deprecated data by removing deprecated model objects and reindexing the model object lists. Distributes affiliated eventy on operation success
        /// </summary>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData()
        {
            return DefaultCleanDeprecatedData();
        }
    }
}
