using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions.ConflictHandling;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Basic implementation of the transition input manager that handles validated adding, removal and replacement of
    ///     transition base data by an outside source
    /// </summary>
    internal class TransitionInputManager : ModelInputManager<TransitionModelData, ITransitionDataPort, TransitionEventManager>,
        ITransitionInputPort
    {
        /// <inheritdoc />
        public TransitionInputManager(TransitionModelData modelData, TransitionEventManager eventManager, IModelProject project)
            : base(modelData, eventManager, project)
        {
        }

        /// <summary>
        ///     Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IStateExchangePair newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.StateExchangePairs));
        }

        /// <summary>
        ///     Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IStateExchangeGroup newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.StateExchangeGroups));
        }

        /// <summary>
        ///     Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IAbstractTransition newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.AbstractTransitions));
        }

        /// <summary>
        ///     Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IKineticTransition newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.KineticTransitions));
        }

        /// <summary>
        ///     Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IMetropolisTransition newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.MetropolisTransitions));
        }

        /// <summary>
        ///     Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IStateExchangePair newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.StateExchangePairs));
        }

        /// <summary>
        ///     Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IStateExchangeGroup newObj)
        {
            return DefaultRemoveModelObject(newObj, accessor => accessor.Query(data => data.StateExchangeGroups));
        }

        /// <summary>
        ///     Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IAbstractTransition newObj)
        {
            return DefaultRemoveModelObject(newObj, accessor => accessor.Query(data => data.AbstractTransitions));
        }

        /// <summary>
        ///     Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IKineticTransition newObj)
        {
            return DefaultRemoveModelObject(newObj, accessor => accessor.Query(data => data.KineticTransitions));
        }

        /// <summary>
        ///     Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IMetropolisTransition newObj)
        {
            return DefaultRemoveModelObject(newObj, accessor => accessor.Query(data => data.MetropolisTransitions));
        }

        /// <summary>
        ///     Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on
        ///     operation success)
        /// </summary>
        /// <param name="orgObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IStateExchangePair orgObj, IStateExchangePair newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.StateExchangePairs));
        }

        /// <summary>
        ///     Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on
        ///     operation success)
        /// </summary>
        /// <param name="orgObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IStateExchangeGroup orgObj, IStateExchangeGroup newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.StateExchangeGroups));
        }

        /// <summary>
        ///     Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on
        ///     operation success)
        /// </summary>
        /// <param name="orgObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IAbstractTransition orgObj, IAbstractTransition newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.AbstractTransitions));
        }

        /// <summary>
        ///     Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on
        ///     operation success)
        /// </summary>
        /// <param name="orgObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IKineticTransition orgObj, IKineticTransition newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.KineticTransitions));
        }

        /// <summary>
        ///     Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on
        ///     operation success)
        /// </summary>
        /// <param name="orgObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IMetropolisTransition orgObj, IMetropolisTransition newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.MetropolisTransitions));
        }

        /// <inheritdoc />
        [DataOperation(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData() => DefaultCleanDeprecatedData();

        /// <inheritdoc />
        protected override IDataConflictHandlerProvider<TransitionModelData> CreateDataConflictHandlerProvider() =>
            new TransitionDataConflictHandlerProvider(ModelProject);
    }
}