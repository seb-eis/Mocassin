using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions.Handler
{
    /// <summary>
    /// Event handler that manages the processing of parameter change events that the transition manager receives from the structure manager event port
    /// </summary>
    internal class StructureParameterChangedEventHandler : ParameterChangedEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <inheritdoc />
        public StructureParameterChangedEventHandler(IModelProject modelProject, DataAccessorSource<TransitionModelData> dataAccessorSource, TransitionEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a change in the cell parameters of the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleChangedCellParameters(IModelParameterEventArgs<ICellParameters> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in the space group info of the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleChangedSpaceGroupInfo(IModelParameterEventArgs<ISpaceGroupInfo> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }

        /// <summary>
        /// Event reaction to a change in the structure info of the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleStructureInfoChange(IModelParameterEventArgs<IStructureInfo> eventArgs)
        {
            return EventTestReaction(eventArgs);
        }
    }
}
