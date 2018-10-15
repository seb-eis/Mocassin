using System.Collections.Generic;
using ICon.Model.Simulations;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IKineticSimulationModel"/>
    public class KineticSimulationModel : ModelComponentBase, IKineticSimulationModel
    {
        /// <inheritdoc />
        public IKineticSimulation Simulation { get; set; }

        /// <inheritdoc />
        public IList<IKineticTransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public IKineticTrackingModel KineticTrackingModel { get; set; }

        /// <inheritdoc />
        public IKineticMappingModel[,,] MappingAssignMatrix { get; set; }
    }
}