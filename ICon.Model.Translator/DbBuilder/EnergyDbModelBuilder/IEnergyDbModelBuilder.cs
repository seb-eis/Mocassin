using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <summary>
    ///     Database model builder that converts simulation model definitions from the model context into the required
    ///     energy database model objects
    /// </summary>
    public interface IEnergyDbModelBuilder
    {
        /// <summary>
        ///     Builds a new energy database model for the passed kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        EnergyModel BuildModel(IKineticSimulationModel simulationModel);

        /// <summary>
        ///     Builds a new energy database model for the passed metropolis simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        EnergyModel BuildModel(IMetropolisSimulationModel simulationModel);
    }
}