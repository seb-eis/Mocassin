using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a kinetic simulation tracking model that describes tracking requirements for the simulation
    /// </summary>
    public interface IKineticTrackingModel : IModelComponent
    {
        /// <summary>
        ///     The simulation model this tracking model is valid for
        /// </summary>
        IKineticSimulationModel SimulationModel { get; set; }

        /// <summary>
        ///     The list of global movement tracker models required for the simulation
        /// </summary>
        IList<IMovementTrackerModel> GlobalTrackerModels { get; set; }

        /// <summary>
        ///     The list of static movement tracker models required for the simulation
        /// </summary>
        IList<IStaticMovementTrackerModel> StaticTrackerModels { get; set; }

        /// <summary>
        ///     The list of jump probability tracker models for the simulation
        /// </summary>
        IList<IProbabilityTrackerModel> ProbabilityTrackerModels { get; set; }

        /// <summary>
        ///     2D Matrix that assigns each position index/particle index a static tracker index
        /// </summary>
        int[,] StaticTrackerMappingTable { get; set; }

        /// <summary>
        ///     2D Matrix that assigns each transition index/particle index combination a probability tracker index
        /// </summary>
        int[,] ProbabilityTrackerMappingTable { get; set; }

        /// <summary>
        ///     The number of required jump probability trackers
        /// </summary>
        int ProbabilityTrackerCount { get; }

        /// <summary>
        ///     The number of required global tracker entries
        /// </summary>
        int GlobalTrackerCount { get; }

        /// <summary>
        ///     The number of required static trackers per unit cell
        /// </summary>
        int StaticTrackerCount { get; }
    }
}