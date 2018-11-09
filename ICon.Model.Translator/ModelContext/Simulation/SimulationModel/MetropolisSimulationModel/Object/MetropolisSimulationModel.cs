using System.Collections.Generic;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IMetropolisSimulationModel" />
    public class MetropolisSimulationModel : ModelComponentBase, IMetropolisSimulationModel
    {
        /// <inheritdoc />
        public IMetropolisSimulation Simulation { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisTransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public IMetropolisMappingModel[,,] MappingAssignMatrix { get; set; }

        /// <inheritdoc />
        public IMetropolisLocalJumpModel[,,] JumpModelMatrix { get; set; }
    }
}