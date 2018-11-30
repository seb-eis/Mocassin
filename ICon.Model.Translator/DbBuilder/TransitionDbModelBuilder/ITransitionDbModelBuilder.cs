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
        ///     Builds a new transition database model for the passed kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        TransitionModel BuildModel(IKineticSimulationModel simulationModel);

        /// <summary>
        ///     Builds a new transition database model for the passed metropolis simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        TransitionModel BuildModel(IMetropolisSimulationModel simulationModel);
    }
}