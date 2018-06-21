using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Transitions;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object change events that the energy manager receives from the transition manager event port
    /// </summary>
    internal class ChangedTransitionObjectsHandler : ChangedObjectsEventHandler<ITransitionEventPort, EnergyModelData, EnergyEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        public ChangedTransitionObjectsHandler(IProjectServices projectServices, DataAccessProvider<EnergyModelData> dataAccessorProvider, EnergyEventManager eventManager)
            : base(projectServices, dataAccessorProvider, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a change in one of the abstract transition objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleAbstractTransition(IModelObjectEventArgs<IAbstractTransition> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in one of the state exchange pair objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleStateExchangePair(IModelObjectEventArgs<IStateExchangePair> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in one of the kinetic transition objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleKineticTransition(IModelObjectEventArgs<IKineticTransition> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in one of the metropolis transition objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleMetropolisStatePair(IModelObjectEventArgs<IMetropolisTransition> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in one of the state exchange groups
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleStateExchangeGroup(IModelObjectEventArgs<IStateExchangeGroup> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }
    }
}
