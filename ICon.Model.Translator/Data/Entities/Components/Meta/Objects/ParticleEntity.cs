using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Particle entity for the simulation that carries meta information about simulated species
    /// </summary>
    public class ParticleEntity : EntityBase
    {
        /// <summary>
        /// Navigation property for the mccs meta info this particle belongs to
        /// </summary>
        public MccsMetaInfo MccsMetaInfo { get; set; }

        /// <summary>
        /// The mccs meta info entity key
        /// </summary>
        [ForeignKey(nameof(MccsMetaInfo))]
        public int MccsMetaInfoId { get; set; }

        /// <summary>
        /// The index of the particle used during calculations
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The name of the particle
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The symbol of the particle
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The effective charge of the particle
        /// </summary>
        public double Charge { get; set; }
    }
}
