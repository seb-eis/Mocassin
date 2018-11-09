using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class KineticTrackingModel : IKineticTrackingModel
    {
        /// <inheritdoc />
        public int ModelId { get; set; }

        /// <inheritdoc />
        public IKineticSimulationModel SimulationModel { get; set; }

        /// <inheritdoc />
        public IList<IMovementTrackerModel> GlobalTrackerModels { get; set; }

        /// <inheritdoc />
        public IList<IMovementTrackerModel> StaticTrackerModels { get; set; }

        /// <inheritdoc />
        public IList<IProbabilityTrackerModel> ProbabilityTrackerModels { get; set; }

        /// <inheritdoc />
        public int[,] StaticTrackerMappingTable { get; set; }

        /// <inheritdoc />
        public int[,] ProbabilityTrackerMappingTable { get; set; }

        /// <inheritdoc />
        public int ProbabilityTrackerCount => ProbabilityTrackerModels.Count;

        /// <inheritdoc />
        public int GlobalTrackerCount => GlobalTrackerModels.Count;

        /// <inheritdoc />
        public int StaticTrackerCount => StaticTrackerModels.Count;
    }
}