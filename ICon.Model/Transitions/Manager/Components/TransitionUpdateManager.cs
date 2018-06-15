using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;
using ICon.Model.Transitions.Handler;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Basic update manager for the transition module that handles pushed information on data changes in required modules
    /// </summary>
    internal class TransitionUpdateManager : ModelUpdateManager<TransitionModelData, TransitionEventManager>, ITransitionUpdatePort
    {
        /// <summary>
        /// Pipeline based event handler for added model objects events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected NewStructureObjectsHandler NewStructureObjectsHandler { get; set; }

        /// <summary>
        /// Pipeline based event handler for changed model objects events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected ChangedStructureObjectsHandler ChangedStructureObjectsHandler { get; set; }

        /// <summary>
        /// Pipeline based event handler for removed model objects events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected RemovedStructureObjectsHandler RemovedStructureObjectsHandler { get; set; }

        /// <summary>
        /// Pipeline based event handler for model object list reindexing events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected ReindexedStructureObjectsHandler ReindexedStructureObjectsHandler { get; set; }

        /// <summary>
        /// Pipeline based event handler for changed model parameter events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected ChangedStructureParametersHandler ChangedStructureParametersHandler { get; set; }

        /// <summary>
        /// Creates new transition update manager for provided model data, event manager and project services
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="eventManager"></param>
        /// <param name="projectServices"></param>
        public TransitionUpdateManager(TransitionModelData baseData, TransitionEventManager eventManager, IProjectServices projectServices)
            : base(baseData, eventManager, projectServices)
        {

        }
    }
}
