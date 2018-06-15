﻿using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object added events that the transition manager receives from the structure manager event port
    /// </summary>
    internal class NewStructureObjectsHandler : NewObjectsEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        public NewStructureObjectsHandler(IProjectServices projectServices, DataAccessProvider<TransitionModelData> dataAccessorProvider, TransitionEventManager eventManager)
            : base(projectServices, dataAccessorProvider, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a new unit cell position in the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleNewUnitCellPosition(IModelObjectEventArgs<IUnitCellPosition> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }
    }
}
