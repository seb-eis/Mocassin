using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.Handler
{
    /// <summary>
    /// Event handler that manages the processing of parameter change events that the transition manager receives from the structure manager event port
    /// </summary>
    internal class ChangedStructureParametersHandler : ChangedParameterEventHandler<IStructureEventPort, TransitionModelData, TransitionEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        public ChangedStructureParametersHandler(IProjectServices projectServices, DataAccessProvider<TransitionModelData> dataAccessorProvider, TransitionEventManager eventManager)
            : base(projectServices, dataAccessorProvider, eventManager)
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
