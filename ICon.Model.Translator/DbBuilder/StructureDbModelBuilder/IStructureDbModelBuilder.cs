using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <summary>
    ///     Database model builder that converts simulation model definitions from the model context into the required
    ///     structure database model objects
    /// </summary>
    public interface IStructureDbModelBuilder
    {
        /// <summary>
        ///     Builds a new structure database model for the passed kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        StructureModel BuildModel(IKineticSimulationModel simulationModel);

        /// <summary>
        ///     Builds a new structure database model for the passed metropolis simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        StructureModel BuildModel(IMetropolisSimulationModel simulationModel);
    }
}