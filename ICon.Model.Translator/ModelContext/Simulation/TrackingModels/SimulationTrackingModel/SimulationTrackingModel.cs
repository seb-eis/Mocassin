using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class SimulationTrackingModel : ISimulationTrackingModel
    {
        /// <inheritdoc />
        public ISimulationModel SimulationModel { get; set; }

        /// <inheritdoc />
        public IList<IStaticMovementTrackerModel> StaticTrackerModels { get; set; }

        /// <inheritdoc />
        public IList<IGlobalTrackerModel> GlobalTrackerModels { get; set; }

        /// <inheritdoc />
        public int[,] StaticTrackerMappingTable { get; set; }

        /// <inheritdoc />
        public int[,] GlobalTrackerMappingTable { get; set; }

        /// <inheritdoc />
        public int GlobalTrackerCount => GlobalTrackerModels.Count;

        /// <inheritdoc />
        public int StaticTrackerCount => StaticTrackerModels.Count;

        /// <summary>
        ///     Get an empty simulation tracking model
        /// </summary>
        /// <returns></returns>
        public static SimulationTrackingModel GetEmpty()
        {
            return new SimulationTrackingModel
            {
                StaticTrackerModels = new List<IStaticMovementTrackerModel>(),
                GlobalTrackerModels = new List<IGlobalTrackerModel>(),
                StaticTrackerMappingTable = new int[0, 0],
                GlobalTrackerMappingTable = new int[0, 0]
            };
        }
    }
}