using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.EntityBuilder
{
    /// <summary>
    ///     Database entity builder that converts simulation model definitions from the model context into the required
    ///     energy database model objects
    /// </summary>
    public interface IEnergyDbEntityBuilder
    {
        /// <summary>
        ///     Builds a new energy database model for the passed simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        SimulationEnergyModel BuildModel(ISimulationModel simulationModel);
    }
}