using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Energy table entity that carries a pair energy set for a specific center particle index
    /// </summary>
    public class PairEnergyTable : EntityBase
    {
        /// <summary>
        /// The index of the center particle
        /// </summary>
        public int CenterParticleIndex { get; set; }

        /// <summary>
        /// Encoded version of the energy value string (Has to be separated to create the table)
        /// </summary>
        public string EnergyValueString { get; set; }
    }
}
