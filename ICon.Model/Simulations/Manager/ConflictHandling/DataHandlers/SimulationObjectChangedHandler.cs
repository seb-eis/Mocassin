using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    ///     Simulation object change handler that handles internal data conflict in the simulation manager when a model object
    ///     is changed
    /// </summary>
    public class SimulationObjectChangedHandler : DataConflictHandler<SimulationModelData, ModelObject>
    {
        /// <inheritdoc />
        public SimulationObjectChangedHandler(IModelProject modelProject)
            : base(modelProject)
        {
        }
    }
}