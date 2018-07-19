using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations.ConflictHandling
{
    /// <summary>
    /// Conflict handler provider for the simulation manager related model objects and parameters
    /// </summary>
    public class SimulationDataConflictHandlerProvider : DataConflictHandlerProvider<SimulationModelData>
    {
        /// <summary>
        /// Create new simulation conflict handler provider that uses the provided project services instance
        /// </summary>
        /// <param name="projectServices"></param>
        public SimulationDataConflictHandlerProvider(IProjectServices projectServices) : base(projectServices)
        {

        }
    }
}
