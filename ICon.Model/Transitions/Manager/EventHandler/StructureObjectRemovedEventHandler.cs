using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions.Handler
{
    /// <summary>
    ///     Event handler that manages the processing of object removal events that the transition manager receives from the
    ///     structure manager event port
    /// </summary>
    internal class StructureObjectRemovedEventHandler : ObjectRemovedEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <inheritdoc />
        public StructureObjectRemovedEventHandler(IModelProject modelProject, DataAccessorSource<TransitionModelData> dataAccessorSource,
            TransitionEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {
        }

        /// <summary>
        ///     Event reaction to a removed particle in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleUnitCellRemoval(IModelObjectEventArgs<ICellSite> eventArgs)
        {
            Console.WriteLine($"{eventArgs} received on {ToString()}");
            return new ConflictReport();
        }
    }
}