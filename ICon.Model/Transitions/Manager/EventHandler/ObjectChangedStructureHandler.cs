using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object change events that the transition manager receives from the structure manager event port
    /// </summary>
    internal class ObjectChangedStructureHandler : ObjectChangedEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorSource"></param>
        /// <param name="eventManager"></param>
        public ObjectChangedStructureHandler(IProjectServices projectServices, DataAccessSource<TransitionModelData> dataAccessorSource, TransitionEventManager eventManager)
            : base(projectServices, dataAccessorSource, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a unit cell position change change in the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleUnitCellPositionChange(IModelObjectEventArgs<IUnitCellPosition> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }
    }
}
