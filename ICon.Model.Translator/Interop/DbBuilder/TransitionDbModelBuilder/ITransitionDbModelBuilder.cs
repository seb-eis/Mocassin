using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <summary>
    ///     Database model builder that converts simulation model definitions from the model context into the required
    ///     transition database model objects
    /// </summary>
    public interface ITransitionDbModelBuilder
    {
        /// <summary>
        ///     Builds a new transition database model for the passed simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        SimulationTransitionModel BuildModel(ISimulationModel simulationModel);
    }
}