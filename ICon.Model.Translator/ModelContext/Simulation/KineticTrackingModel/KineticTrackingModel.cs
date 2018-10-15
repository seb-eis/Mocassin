using System.Collections.Generic;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class KineticTrackingModel : IKineticTrackingModel
    {
        /// <inheritdoc />
        public int ModelId { get; set; }

        /// <inheritdoc />
        public IKineticSimulationModel SimulationModel { get; set; }

        /// <inheritdoc />
        public IList<IGlobalTrackerModel> GlobalTrackerModels { get; set; }

        /// <inheritdoc />
        public IList<IStaticTrackerModel> StaticTrackerModels { get; set; }

        /// <inheritdoc />
        public IList<IProbabilityTrackerModel> ProbabilityTrackerModels { get; set; }

        /// <inheritdoc />
        public int[,] StaticTrackerMappingTable { get; set; }

        /// <inheritdoc />
        public int[,] ProbabilityTrackerMappingTable { get; set; }

        /// <inheritdoc />
        public int NumOfProbabilityTrackers => ProbabilityTrackerModels.Count;

        /// <inheritdoc />
        public int NumOfGlobalTrackers => GlobalTrackerModels.Count;

        /// <inheritdoc />
        public int NumOfStaticTrackersPerCell => StaticTrackerModels.Count;
    }
}