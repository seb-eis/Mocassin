using Mocassin.Model.Basic;
using Mocassin.Model.Energies.Handler;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Basic update manager for the energy module that handles pushed information on data changes in required modules
    /// </summary>
    internal class EnergyUpdateManager : ModelUpdateManager<EnergyModelData, EnergyEventManager>, IEnergyUpdatePort
    {
        /// <summary>
        ///     Pipeline based handler for added model objects in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected TransitionObjectAddedEventHandler TransitionObjectAddedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based handler for changed model objects in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected TransitionObjectChangedEventHandler TransitionObjectChangedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based handler for removed model objects in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected TransitionObjectRemovedEventHandler TransitionObjectRemovedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based handler for reindexing of model object lists in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected TransitionObjectIndexingChangedHandler TransitionObjectIndexingChangedHandler { get; set; }

        /// <inheritdoc />
        public EnergyUpdateManager(EnergyModelData modelData, EnergyEventManager eventManager, IModelProject modelProject)
            : base(modelData, eventManager, modelProject)
        {
        }
    }
}