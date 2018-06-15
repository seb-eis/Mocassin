using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object removal events that the transition manager receives from the structure manager event port
    /// </summary>
    internal class RemovedStructureObjectsHandler : RemovedObjectsEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        public RemovedStructureObjectsHandler(IProjectServices projectServices, DataAccessProvider<TransitionModelData> dataAccessorProvider, TransitionEventManager eventManager)
            : base(projectServices, dataAccessorProvider, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a removed particle in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleUnitCellRemoval(IModelObjectEventArgs<IUnitCellPosition> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }
    }
}
