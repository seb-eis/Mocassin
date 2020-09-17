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
        ///     Resolver method that handles the required internal changes if the <see cref="KineticSimulation" /> changes
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport ResolveSpaceGroupChange(KineticSimulation simulation, IDataAccessor<SimulationModelData> dataAccess) =>
            new KineticSimulationChangeHandler(dataAccess, ModelProject).HandleConflicts(simulation);

        /// <summary>
        ///     Resolver method that handles the required internal changes if the <see cref="MetropolisSimulation" /> changes
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport ResolveCellParametersChange(MetropolisSimulation simulation, IDataAccessor<SimulationModelData> dataAccess) =>
            new MetropolisSimulationChangeHandler(dataAccess, ModelProject).HandleConflicts(simulation);
    }
}