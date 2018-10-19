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

        /// <summary>
        /// Handles a metropolis simulation series object change
        /// </summary>
        /// <param name="series"></param>
        /// <param name="accessor"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        private IConflictReport HandleObjectChange(MetropolisSimulationSeries series, IDataAccessor<SimulationModelData> accessor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles a kinetic simulation series object change
        /// </summary>
        /// <param name="series"></param>
        /// <param name="accessor"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        private IConflictReport HandleObjectChange(KineticSimulationSeries series, IDataAccessor<SimulationModelData> accessor)
        {
            throw new NotImplementedException();
        }
    }
}