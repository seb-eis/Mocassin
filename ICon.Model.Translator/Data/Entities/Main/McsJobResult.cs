using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// The mcs job result entity that stores the simulation state at the end of the simulation for processing
    /// </summary>
    public class McsJobResult : EntityBase
    {
        /// <summary>
        /// The mcs job entity key
        /// </summary>
        [ForeignKey(nameof(McsJob))]
        public int McsJobId { get; set; }

        /// <summary>
        /// Navigation property for the job object this result belongs to
        /// </summary>
        public McsJob McsJob { get; set; }

        /// <summary>
        /// The simulation state as a byte array. Conatins the raw result information
        /// </summary>
        public byte[] State { get; set; }
    }
}
