using System.Collections.Generic;
using ICon.Model.Simulations;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IMetropolisSimulationModel"/>
    public class MetropolisSimulationModel : ModelComponentBase, IMetropolisSimulationModel
    {
        /// <inheritdoc />
        public IMetropolisSimulation Simulation { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisTransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public IMetropolisMappingModel[,,] MappingAssignMatrix { get; set; }
    }
}