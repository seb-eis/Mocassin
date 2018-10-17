using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object reindexing events that the transition manager receives from the structure manager event port
    /// </summary>
    internal class ReindexedStructureObjectsHandler : ObjectAddedEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="dataAccessorSource"></param>
        /// <param name="eventManager"></param>
        public ReindexedStructureObjectsHandler(IModelProject modelProject, DataAccessSource<TransitionModelData> dataAccessorSource, TransitionEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a reindexed unit cell position list in the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleUnitCellPositionListListReindexing(IModelIndexingEventArgs<IUnitCellPosition> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }
    }
}
