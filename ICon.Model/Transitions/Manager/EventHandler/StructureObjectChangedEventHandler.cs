using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions.Handler
{
    /// <summary>
    ///     Event handler that manages the processing of object change events that the transition manager receives from the
    ///     structure manager event port
    /// </summary>
    internal class StructureObjectChangedEventHandler : ObjectChangedEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <inheritdoc />
        public StructureObjectChangedEventHandler(IModelProject modelProject, DataAccessorSource<TransitionModelData> dataAccessorSource,
            TransitionEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {
        }

        /// <summary>
        ///     Event reaction to a unit cell position change change in the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleCellReferencePositionChange(IModelObjectEventArgs<ICellReferencePosition> eventArgs)
        {
            return DummyHandleEvent(eventArgs);
        }
    }
}