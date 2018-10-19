using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Implementation of the simulation update manager that handles subscriptions and reactions to events on other
    ///     managers
    /// </summary>
    internal class SimulationUpdateManager : ModelUpdateManager<SimulationModelData, SimulationEventManager>, ISimulationUpdatePort
    {
        /// <inheritdoc />
        public SimulationUpdateManager(SimulationModelData modelData, SimulationEventManager eventManager, IModelProject modelProject)
            : base(modelData, eventManager, modelProject)
        {
        }
    }
}