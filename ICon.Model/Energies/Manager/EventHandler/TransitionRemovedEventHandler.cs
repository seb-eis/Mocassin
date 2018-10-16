using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Transitions;

namespace ICon.Model.Energies.Handler
{
    /// <summary>
    ///     Event handler that manages the processing of object removal events that the energy manager receives from the
    ///     transition manager event port
    /// </summary>
    internal class TransitionRemovedEventHandler : ObjectRemovedEventHandler<ITransitionEventPort, EnergyModelData, EnergyEventManager>
    {
        /// <inheritdoc />
        public TransitionRemovedEventHandler(IProjectServices projectServices, DataAccessSource<EnergyModelData> dataAccessorSource,
            EnergyEventManager eventManager)
            : base(projectServices, dataAccessorSource, eventManager)
        {
        }

        /// <summary>
        ///     Event reaction to a change in one of the abstract transition objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleAbstractTransition(IModelObjectEventArgs<IAbstractTransition> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        ///     Event reaction to a change in one of the state exchange pair objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleStateExchangePair(IModelObjectEventArgs<IStateExchangePair> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        ///     Event reaction to a change in one of the kinetic transition objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleKineticTransition(IModelObjectEventArgs<IKineticTransition> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        ///     Event reaction to a change in one of the metropolis transition objects
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleMetropolisStatePair(IModelObjectEventArgs<IMetropolisTransition> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        ///     Event reaction to a removal of one of the state exchange groups
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