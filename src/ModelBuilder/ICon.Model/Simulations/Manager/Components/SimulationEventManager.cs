using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Implementation of the simulation event manager that offers subscriptions based push notifications about simulation
    ///     model data changes
    /// </summary>
    internal class SimulationEventManager : ModelEventManager, ISimulationEventPort
    {
    }
}