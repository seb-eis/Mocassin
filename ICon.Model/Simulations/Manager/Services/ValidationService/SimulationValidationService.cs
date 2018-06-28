using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Implementation of the validation service for simulation manager related model objects and parameters
    /// </summary>
    public class SimulationValidationService : ValidationService<ISimulationDataPort>
    {
        /// <summary>
        /// The simulation settings used in the validation methods
        /// </summary>
        protected BasicSimulationSettings Settings { get; }

        /// <summary>
        /// Create a new simulation validation service that uses the provider project services and simulation settings object
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="projectServices"></param>
        public SimulationValidationService(BasicSimulationSettings settings, IProjectServices projectServices) : base(projectServices)
        {

        }
    }
}
