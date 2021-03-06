﻿using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a kinetic simulation tracking model that describes tracking requirements for the simulation
    /// </summary>
    public interface ISimulationTrackingModel
    {
        /// <summary>
        ///     The simulation model this tracking model is valid for
        /// </summary>
        ISimulationModel SimulationModel { get; set; }

        /// <summary>
        ///     The list of static movement tracker models required for the simulation
        /// </summary>
        IList<IStaticMovementTrackerModel> StaticTrackerModels { get; set; }

        /// <summary>
        ///     The list of jump probability tracker models for the simulation
        /// </summary>
        IList<IGlobalTrackerModel> GlobalTrackerModels { get; set; }

        /// <summary>
        ///     2D Matrix that assigns each position index/particle index a static tracker index
        /// </summary>
        int[,] StaticTrackerMappingTable { get; set; }

        /// <summary>
        ///     2D Matrix that assigns each transition index/particle index combination a global tracker index (for global movement
        ///     and probability tracking)
        /// </summary>
        int[,] GlobalTrackerMappingTable { get; set; }

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