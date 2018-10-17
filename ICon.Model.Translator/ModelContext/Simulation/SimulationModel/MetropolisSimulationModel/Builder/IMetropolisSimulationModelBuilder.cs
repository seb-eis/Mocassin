using System.Collections.Generic;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Builder for metropolis simulation models. Extends reference metropolis data of the project into a full data context
    /// </summary>
    public interface IMetropolisSimulationModelBuilder
    {
        /// <summary>
        /// Builds the metropolis simulation models for the passed sequence of metropolis simulations
        /// </summary>
        /// <param name="metropolisSimulations"></param>
        /// <returns></returns>
        IList<IMetropolisSimulationModel> BuildModels(IEnumerable<IMetropolisSimulation> metropolisSimulations);

        /// <summary>
        /// Builds the components on the metropolis simulation models that require completed linking
        /// </summary>
        /// <param name="simulationModels"></param>
        void BuildLinkingDependentComponents(IEnumerable<IMetropolisSimulationModel> simulationModels);
    }
}