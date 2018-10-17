﻿using Mocassin.Model.Basic;
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
        protected TransitionAddedEventHandler TransitionAddedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based handler for changed model objects in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected TransitionChangedEventHandler TransitionChangedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based handler for removed model objects in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected TransitionRemovedEventHandler TransitionRemovedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based handler for reindexing of model object lists in the connected transition manager
        /// </summary>
        [UpdateHandler(typeof(ITransitionEventPort))]
        protected TransitionIndexingChangedHandler TransitionIndexingChangedHandler { get; set; }

        /// <inheritdoc />
        public EnergyUpdateManager(EnergyModelData baseData, EnergyEventManager eventManager, IModelProject modelProject)
            : base(baseData, eventManager, modelProject)
        {
        }
    }
}