using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class KineticIndexingModel : IKineticIndexingModel
    {
        /// <inheritdoc />
        public IKineticSimulationModel SimulationModel { get; set; }

        /// <inheritdoc />
        public IDictionary<IKineticTransitionModel, int> TransitionModelToJumpCollectionId { get; set; }

        /// <inheritdoc />
        public IDictionary<IKineticMappingModel, int> TransitionMappingToJumpDirectionId { get; set; }

        /// <inheritdoc />
        public int[,] JumpCountTable { get; set; }

        /// <inheritdoc />
        public int[,,,] JumpIndexAssignTable { get; set; }
    }
}