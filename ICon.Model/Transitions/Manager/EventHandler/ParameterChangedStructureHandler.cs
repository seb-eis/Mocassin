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
    internal class ParameterChangedStructureHandler : ParameterChangedEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="dataAccessorSource"></param>
        /// <param name="eventManager"></param>
        public ParameterChangedStructureHandler(IModelProject modelProject, DataAccessSource<TransitionModelData> dataAccessorSource, TransitionEventManager eventManager)
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
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }

        /// <summary>
        /// Event reaction to a change in the space group info of the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleChangedSpaceGroupInfo(IModelParameterEventArgs<ISpaceGroupInfo> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }

        /// <summary>
        /// Event reaction to a change in the structure info of the structure manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleStructureInfoChange(IModelParameterEventArgs<IStructureInfo> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }
    }
}
