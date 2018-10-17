using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    /// Simulation object change hanlder that handles inernal data conflict in the simulation manager when a model object is changed
    /// </summary>
    public class SimulationObjectChangedHandler : DataConflictHandler<SimulationModelData, ModelObject>
    {
        /// <summary>
        /// Create new simulation object changed hadler that uses the provide project services
        /// </summary>
        /// <param name="modelProject"></param>
        public SimulationObjectChangedHandler(IModelProject modelProject) : base(modelProject)
        {
        }

        [ConflictHandlingMethod]
        IConflictReport HandleObjectChange(MetropolisSimulationSeries series, IDataAccessor<SimulationModelData> accessor)
        {
            throw new NotImplementedException();
        }

        [ConflictHandlingMethod]
        IConflictReport HandleObjectChange(KineticSimulationSeries series, IDataAccessor<SimulationModelData> accessor)
        {
            throw new NotImplementedException();
        }
    }
}
