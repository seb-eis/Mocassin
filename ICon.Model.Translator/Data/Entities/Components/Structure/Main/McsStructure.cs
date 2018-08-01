using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// The mcs structure component that carries all encoded simulation data affiliated with structure modelling
    /// </summary>
    public class McsStructure : McsComponent
    {
        /// <summary>
        /// The list of position environments for the simulation
        /// </summary>
        public List<Environment> Environments { get; set; }
        
        /// <summary>
        /// The number of required static trackers per unit cell
        /// </summary>
        public int PerCellTrackerCount { get; set; }

        /// <summary>
        /// The number of required global trackers
        /// </summary>
        public int GlobalTrackerCount { get; set; }

        /// <summary>
        /// The tracker offset matrix entity id
        /// </summary>
        [ForeignKey(nameof(TrackerOffsetMatrix))]
        public int TrackerOffsetMatrixId { get; set; }

        /// <summary>
        /// The 2D tracker indexing offset matrix. Contains the tracker index offset for each particle and cell position index combination
        /// </summary>
        /// <remarks> Combinations that do not have a static tracker have an index offset of -1 </remarks>
        public MatrixEntity<int> TrackerOffsetMatrix { get; set; }
    }
}
