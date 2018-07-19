using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations
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
        /// <param name="projectServices"></param>
        public SimulationUpdateManager(SimulationModelData baseData, SimulationEventManager eventManager, IProjectServices projectServices)
            : base(baseData, eventManager, projectServices)
        {

        }
    }
}
