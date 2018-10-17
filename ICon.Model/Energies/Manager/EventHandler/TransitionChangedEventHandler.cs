using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Energies.Handler
{
    /// <summary>
    ///     Event handler that manages the processing of object change events that the energy manager receives from the
    ///     transition manager event port
    /// </summary>
    internal class TransitionChangedEventHandler : ObjectChangedEventHandler<ITransitionEventPort, EnergyModelData, EnergyEventManager>
    {
        /// <inheritdoc />
        public TransitionChangedEventHandler(IModelProject modelProject, DataAccessSource<EnergyModelData> dataAccessorSource,
            EnergyEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
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
        ///     Event reaction to a change in one of the state exchange groups
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