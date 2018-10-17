using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Implementation of the simulation update manager that handles subscriptions and reactions to events on other managers
    /// </summary>
    internal class SimulationUpdateManager : ModelUpdateManager<SimulationModelData, SimulationEventManager>, ISimulationUpdatePort
    {
        /// <summary>
        /// Create new simulation update manager using the provided model data object, event manager and project services
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="eventManager"></param>
        /// <param name="modelProject"></param>
        public SimulationUpdateManager(SimulationModelData baseData, SimulationEventManager eventManager, IModelProject modelProject)
            : base(baseData, eventManager, modelProject)
        {

        }
    }
}
