using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IMetropolisSimulationModel" />
    public class MetropolisSimulationModel : SimulationModel, IMetropolisSimulationModel
    {
        /// <inheritdoc />
        public IMetropolisSimulation Simulation { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisTransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public IMetropolisMappingModel[,,] MappingAssignMatrix { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisLocalJumpModel> LocalJumpModels { get; set; }

        /// <inheritdoc />
        public override double MaxAttemptFrequency { get; set; }

        /// <inheritdoc />
        public override Cartesian3D NormalizedElectricFieldVector { get; set; }

        /// <summary>
        ///     Create new metropolis simulation model with empty tracking model
        /// </summary>
        public MetropolisSimulationModel()
        {
            SimulationTrackingModel = ModelContext.SimulationTrackingModel.GetEmpty();
        }

        /// <inheritdoc />
        public override IEnumerable<ITransitionModel> GetTransitionModels() => TransitionModels.AsEnumerable();

        /// <inheritdoc />
        public override IEnumerable<ILocalJumpModel> GetLocalJumpModels() => LocalJumpModels.AsEnumerable();
    }
}