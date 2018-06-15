using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ICon.Framework.Operations;
using ICon.Framework.Processing;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Basic implementation of the transition input manager that handles validated adding, removal and replacement of transition base data by an outside source
    /// </summary>
    internal class TransitionInputManager : ModelInputManager<TransitionModelData, ITransitionDataPort, TransitionEventManager>, ITransitionInputPort
    {
        /// <summary>
        /// Create a new transition input manager from data object, event manager and project services
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        /// <param name="services"></param>
        public TransitionInputManager(TransitionModelData data, TransitionEventManager manager, IProjectServices services) : base(data, manager, services)
        {

        }

        /// <summary>
        /// Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IPropertyStatePair newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.PropertyStatePairs));
        }

        /// <summary>
        /// Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IPropertyGroup newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.PropertyGroups));
        }

        /// <summary>
        /// Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IAbstractTransition newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.AbstractKineticTransitions));
        }

        /// <summary>
        /// Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IKineticTransition newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.KineticTransitions));
        }

        /// <summary>
        /// Tries to register the passed model object with the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewModelObject(IMetropolisTransition newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.MetropolisTransitions));
        }

        /// <summary>
        /// Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IPropertyStatePair newObj)
        {
            return DefaultRegisterModelObject(newObj, accessor => accessor.Query(data => data.PropertyStatePairs));
        }

        /// <summary>
        /// Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IPropertyGroup newObj)
        {
            return DefaultRemoveModelObject(newObj, accessor => accessor.Query(data => data.PropertyGroups));
        }

        /// <summary>
        /// Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IAbstractTransition newObj)
        {
            return DefaultRemoveModelObject(newObj, accessor => accessor.Query(data => data.AbstractKineticTransitions));
        }

        /// <summary>
        /// Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IKineticTransition newObj)
        {
            return DefaultRemoveModelObject(newObj, accessor => accessor.Query(data => data.KineticTransitions));
        }

        /// <summary>
        /// Tries to remove the passed model object from the manager (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveModelObject(IMetropolisTransition newObj)
        {
            return DefaultRemoveModelObject(newObj, accessor => accessor.Query(data => data.MetropolisTransitions));
        }

        /// <summary>
        /// Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IPropertyStatePair orgObj, IPropertyStatePair newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.PropertyStatePairs));
        }

        /// <summary>
        /// Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IPropertyGroup orgObj, IPropertyGroup newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.PropertyGroups));
        }

        /// <summary>
        /// Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IAbstractTransition orgObj, IAbstractTransition newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.AbstractKineticTransitions));
        }

        /// <summary>
        /// Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IKineticTransition orgObj, IKineticTransition newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.KineticTransitions));
        }

        /// <summary>
        /// Tries to replace an existing model object in the manager by a new one (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="newObj"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceModelObject(IMetropolisTransition orgObj, IMetropolisTransition newObj)
        {
            return DefaultReplaceModelObject(orgObj, newObj, accessor => accessor.Query(data => data.MetropolisTransitions));
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

        /// <summary>
        /// Get the conflict resolver provider for the internal transition data conflict handling
        /// </summary>
        /// <returns></returns>
        protected override IDataConflictHandlerProvider<TransitionModelData> MakeConflictHandlerProvider()
        {
            return new ConflictHandling.TransitionDataConflictHandlerProvider(ProjectServices);
        }
    }
}
