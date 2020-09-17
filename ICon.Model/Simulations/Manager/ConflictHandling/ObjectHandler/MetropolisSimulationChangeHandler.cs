using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    ///     Conflict Handler implementation of <see cref="SimulationChangeHandlerBase{T}" /> for changes in
    ///     <see cref="MetropolisSimulation" /> objects
    /// </summary>
    public class MetropolisSimulationChangeHandler : SimulationChangeHandlerBase<MetropolisSimulation>
    {
        /// <inheritdoc />
        public MetropolisSimulationChangeHandler(IDataAccessor<SimulationModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }
    }
}