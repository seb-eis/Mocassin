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
        /// The tracking header entity id
        /// </summary>
        [ForeignKey(nameof(TrackingHeader))]
        public int TrackingHeaderId { get; set; }

        /// <summary>
        /// Navigation property for the tracking header
        /// </summary>
        public TrackingHeader TrackingHeader { get; set; }

        /// <summary>
        /// The list of positions environments for the simulation
        /// </summary>
        public List<Environment> Envrionments { get; set; }

        /// <summary>
        /// The list of cell positions for the simulation
        /// </summary>
        public List<CellPosition> CellPositions { get; set; }
    }
}
