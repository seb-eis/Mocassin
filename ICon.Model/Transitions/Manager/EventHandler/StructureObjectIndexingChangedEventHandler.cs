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
    internal class StructureObjectIndexingChangedEventHandler : ObjectAddedEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <inheritdoc />
        public StructureObjectIndexingChangedEventHandler(IModelProject modelProject, DataAccessorSource<TransitionModelData> dataAccessorSource, TransitionEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a changed unit cell position list indexing in the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleCellReferencePositionListReindexing(IModelIndexingEventArgs<ICellReferencePosition> eventArgs)
        {
            return new ConflictReport();
        }
    }
}
