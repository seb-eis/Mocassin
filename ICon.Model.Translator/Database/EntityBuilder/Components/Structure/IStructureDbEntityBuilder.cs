using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.EntityBuilder
{
    /// <summary>
    ///     Database entity builder that converts simulation model definitions from the model context into the required
    ///     structure database model objects
    /// </summary>
    public interface IStructureDbEntityBuilder
    {
        /// <summary>
        ///     Builds a new structure database model for the passed simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        SimulationStructureModel BuildModel(ISimulationModel simulationModel);
    }
}