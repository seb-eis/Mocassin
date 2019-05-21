using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class SimulationEncodingModel : ISimulationEncodingModel
    {
        /// <inheritdoc />
        public IDictionary<ITransitionModel, int> TransitionModelToJumpCollectionId { get; set; }

        /// <inheritdoc />
        public IDictionary<ITransitionMappingModel, int> TransitionMappingToJumpDirectionId { get; set; }

        /// <inheritdoc />
        public IDictionary<ITransitionRuleModel, double> TransitionRuleToElectricFieldFactors { get; set; }

        /// <inheritdoc />
        public IList<MobilityType[]> PositionIndexToMobilityTypesSet { get; set; }

        /// <inheritdoc />
        public IDictionary<ITransitionMappingModel, double> TransitionMappingToElectricFieldFactors { get; set; }

        /// <inheritdoc />
        public int[,] JumpCountMappingTable { get; set; }

        /// <inheritdoc />
        public int[,,] JumpDirectionIdMappingTable { get; set; }
    }
}