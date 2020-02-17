using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IKineticSimulationModel" />
    public class KineticSimulationModel : SimulationModel, IKineticSimulationModel
    {
        /// <inheritdoc />
        public IKineticSimulation Simulation { get; set; }

        /// <inheritdoc />
        public override double MaxAttemptFrequency { get; set; }

        /// <inheritdoc cref="IKineticSimulationModel.NormalizedElectricFieldVector" />
        public override Cartesian3D NormalizedElectricFieldVector { get; set; }

        /// <inheritdoc />
        public IList<IKineticTransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public IKineticMappingModel[,,] MappingAssignMatrix { get; set; }

        /// <inheritdoc />
        public IList<IKineticLocalJumpModel> LocalJumpModels { get; set; }

        /// <inheritdoc />
        public override IEnumerable<ITransitionModel> GetTransitionModels()
        {
            return TransitionModels.AsEnumerable();
        }

        /// <inheritdoc />
        public override IEnumerable<ILocalJumpModel> GetLocalJumpModels()
        {
            return LocalJumpModels.AsEnumerable();
        }
    }
}