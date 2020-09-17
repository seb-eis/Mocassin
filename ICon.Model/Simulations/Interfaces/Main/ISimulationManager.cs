using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Represents a simulation manager that handles definition and packaging of simulation sets for simulator encoding
    /// </summary>
    public interface ISimulationManager : IModelManager<ISimulationInputPort, ISimulationEventPort, ISimulationQueryPort>
    {
    }
}