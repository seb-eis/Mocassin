using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations.ConflictHandling
{
    /// <summary>
    /// Simulation object change hanlder that handles inernal data conflict in the simulation manager when a model object is changed
    /// </summary>
    public class SimulationObjectChangedHandler : DataConflictHandler<SimulationModelData, ModelObject>
    {
        /// <summary>
        /// Create new simulation object changed hadler that uses the provide project services
        /// </summary>
        /// <param name="projectServices"></param>
        public SimulationObjectChangedHandler(IProjectServices projectServices) : base(projectServices)
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
