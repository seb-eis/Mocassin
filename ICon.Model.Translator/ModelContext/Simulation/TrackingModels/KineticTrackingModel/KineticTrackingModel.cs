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
        public IList<IStaticMovementTrackerModel> StaticTrackerModels { get; set; }

        /// <inheritdoc />
        public IList<IGlobalTrackerModel> GlobalTrackerModels { get; set; }

        /// <inheritdoc />
        public int[,] StaticTrackerMappingTable { get; set; }

        /// <inheritdoc />
        public int[,] GlobalTrackerMappingTable { get; set; }

        /// <inheritdoc />
        public int ProbabilityTrackerCount => GlobalTrackerModels.Count;

        /// <inheritdoc />
        public int GlobalTrackerCount => GlobalTrackerModels.Count;

        /// <inheritdoc />
        public int StaticTrackerCount => StaticTrackerModels.Count;
    }
}