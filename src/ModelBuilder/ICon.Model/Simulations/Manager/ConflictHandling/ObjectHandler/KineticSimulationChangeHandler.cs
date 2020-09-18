using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    ///     Conflict Handler implementation of <see cref="SimulationChangeHandlerBase{T}" /> for changes in
    ///     <see cref="KineticSimulation" /> objects
    /// </summary>
    public class KineticSimulationChangeHandler : SimulationChangeHandlerBase<KineticSimulation>
    {
        /// <inheritdoc />
        public KineticSimulationChangeHandler(IDataAccessor<SimulationModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }
    }
}