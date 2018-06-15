using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Transitions;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object removal events that the energy manager receives from the transition manager event port
    /// </summary>
    internal class RemovedTransitionObjectsHandler : RemovedObjectsEventHandler<ITransitionEventPort, EnergyModelData, EnergyEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        public RemovedTransitionObjectsHandler(IProjectServices projectServices, DataAccessProvider<EnergyModelData> dataAccessorProvider, EnergyEventManager eventManager)
            : base(projectServices, dataAccessorProvider, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a change in on of the abstract transition objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleAbstractTransition(IModelObjectEventArgs<IAbstractTransition> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in on of the property state pair objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandlePropertyStatePair(IModelObjectEventArgs<IPropertyStatePair> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in on of the kinetic transition objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleKineticTransition(IModelObjectEventArgs<IKineticTransition> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in on of the metropolis transition objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleMetropolisStatePair(IModelObjectEventArgs<IMetropolisTransition> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in on of the property group objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandlePropertyGroup(IModelObjectEventArgs<IPropertyGroup> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }
    }
}
