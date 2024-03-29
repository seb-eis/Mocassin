﻿using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies.ConflictHandling;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Basic implementation of the energy input manager that handles validated adding, removal and replacement of energy
    ///     base data by an outside source
    /// </summary>
    internal class EnergyInputManager : ModelInputManager<EnergyModelData, IEnergyDataPort, EnergyEventManager>, IEnergyInputPort
    {
        /// <inheritdoc />
        public EnergyInputManager(EnergyModelData modelData, EnergyEventManager eventManager, IModelProject project)
            : base(modelData, eventManager, project)
        {
        }

        /// <summary>
        ///     Registers a new stable group info object with the manager if it passes validation (Awaits distribution of
        ///     affiliated events in case of operation success)
        /// </summary>
        /// <param name="newObject"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewStableGroupInfo(IGroupInteraction newObject)
        {
            return DefaultRegisterModelObject(newObject, accessor => accessor.Query(data => data.GroupInteractions));
        }

        /// <summary>
        ///     Replaces an existing stable group info with a new one if it passes validation (Awaits distribution of affiliated
        ///     events in case of operation success)
        /// </summary>
        /// <param name="orgObject"></param>
        /// <param name="newObject"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceStableGroupInfo(IGroupInteraction orgObject, IGroupInteraction newObject)
        {
            return DefaultReplaceModelObject(orgObject, newObject, accessor => accessor.Query(data => data.GroupInteractions));
        }

        /// <summary>
        ///     Removes an existing stable group info by deprecating it within the indexed list (Awaits distribution of affiliated
        ///     events in case of operation success)
        /// </summary>
        /// <param name="orgObject"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveStableGroupInfo(IGroupInteraction orgObject)
        {
            return DefaultRemoveModelObject(orgObject, accessor => accessor.Query(data => data.GroupInteractions));
        }

        /// <summary>
        ///     Registers a new unstable environment info object with the manager if it passes validation (Awaits distribution of
        ///     affiliated events in case of operation success)
        /// </summary>
        /// <param name="newObject"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewUnstableEnvironmentInfo(IUnstableEnvironment newObject)
        {
            return DefaultRegisterModelObject(newObject, accessor => accessor.Query(data => data.UnstableEnvironments));
        }

        /// <summary>
        ///     Replaces an existing unstable environment info with a new one if it passes validation (Awaits distribution of
        ///     affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgObject"></param>
        /// <param name="newObject"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceUnstableEnvironmentInfo(IUnstableEnvironment orgObject, IUnstableEnvironment newObject)
        {
            return DefaultReplaceModelObject(orgObject, newObject, accessor => accessor.Query(data => data.UnstableEnvironments));
        }

        /// <summary>
        ///     Removes an existing unstable environment info by deprecating it within the indexed list (Awaits distribution of
        ///     affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgObject"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveUnstableEnvironmentInfo(IUnstableEnvironment orgObject)
        {
            return DefaultRemoveModelObject(orgObject, accessor => accessor.Query(data => data.UnstableEnvironments));
        }

        /// <summary>
        ///     Replaces the currently set environment info parameter if the new one passes validation (Awaits distribution of
        ///     affiliated events in case of operation success)
        /// </summary>
        /// <param name="newParameter"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ParameterChange)]
        protected IOperationReport TrySetEnvironmentInformation(IStableEnvironmentInfo newParameter)
        {
            return DefaultSetModelParameter(newParameter, accessor => accessor.Query(data => data.StableEnvironmentInfo), true);
        }


        /// <inheritdoc />
        protected override IDataConflictHandlerProvider<EnergyModelData> CreateDataConflictHandlerProvider() =>
            new EnergyDataConflictHandlerProvider(ModelProject);

        /// <inheritdoc />
        [DataOperation(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData() => DefaultCleanDeprecatedData();
    }
}