using System.Collections.Generic;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Kinetic simulation model builder. Extends kinetic simulation reference objects into a full data context
    /// </summary>
    public interface IKineticSimulationModelBuilder
    {
        /// <summary>
        ///     Creates the list of kinetic simulation models for the passed set of kinetic simulation objects
        /// </summary>
        /// <param name="kineticSimulations"></param>
        /// <returns></returns>
        IList<IKineticSimulationModel> BuildModels(IEnumerable<IKineticSimulation> kineticSimulations);

        /// <summary>
        ///     Builds the components on the kinetic simulation models that require completed linking
        /// </summary>
        /// <param name="simulationModels"></param>
        void BuildLinkingDependentComponents(IEnumerable<IKineticSimulationModel> simulationModels);
    }
}