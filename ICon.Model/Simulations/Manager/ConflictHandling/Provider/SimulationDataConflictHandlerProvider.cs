using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    /// Conflict handler provider for the simulation manager related model objects and parameters
    /// </summary>
    public class SimulationDataConflictHandlerProvider : DataConflictHandlerProvider<SimulationModelData>
    {
        /// <summary>
        /// Create new simulation conflict handler provider that uses the provided project services instance
        /// </summary>
        /// <param name="modelProject"></param>
        public SimulationDataConflictHandlerProvider(IModelProject modelProject) : base(modelProject)
        {

        }
    }
}
