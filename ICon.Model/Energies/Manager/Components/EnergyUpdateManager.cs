using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Energies.Handler;
using ICon.Model.Transitions;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Basic update manager for the energy module that handles pushed information on data changes in required modules
    /// </summary>
    internal class EnergyUpdateManager : ModelUpdateManager<EnergyModelData, EnergyEventManager>, IEnergyUpdatePort
    {
        /// <summary>
        /// Pipeline based handler for added model objects in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected NewTransitionObjectsHandler NewTransitionObjectsHandler { get; set; }

        /// <summary>
        /// Pipeline based handler for changed model objects in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected ChangedTransitionObjectsHandler ChangedTransitionObjectsHandler { get; set; }

        /// <summary>
        /// Pipeline based handler for removed model objects in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected RemovedTransitionObjectsHandler RemovedTransitionObjectsHandler { get; set; }

        /// <summary>
        /// Pipeline based handler for reindexed model object lists in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected ReindexedTransitionObjectsHandler ReindexedTransitionObjectsHandler { get; set; }

        /// <summary>
        /// Create new energy update manager for the provided data object, event manager and project services
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="eventManager"></param>
        /// <param name="projectServices"></param>
        public EnergyUpdateManager(EnergyModelData baseData, EnergyEventManager eventManager, IProjectServices projectServices)
            : base(baseData, eventManager, projectServices)
        {

        }
    }
}
