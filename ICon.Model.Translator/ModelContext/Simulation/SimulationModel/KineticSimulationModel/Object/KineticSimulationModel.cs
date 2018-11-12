using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IKineticSimulationModel" />
    public class KineticSimulationModel : ModelComponentBase, IKineticSimulationModel
    {
        /// <inheritdoc />
        public IKineticSimulation Simulation { get; set; }

        /// <inheritdoc />
        public Cartesian3D NormalizedElectricFieldVector { get; set; }

        /// <inheritdoc />
        public IList<IKineticTransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public IKineticTrackingModel KineticTrackingModel { get; set; }

        /// <inheritdoc />
        public IKineticMappingModel[,,] MappingAssignMatrix { get; set; }

        /// <inheritdoc />
        public IList<IKineticLocalJumpModel> LocalJumpModels { get; set; }
    }
}