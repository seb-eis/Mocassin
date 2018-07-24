using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Mccs parent entity that carries the shared encoded base components of a simulation job set
    /// </summary>
    public class McsParent : EntityBase
    {
        /// <summary>
        /// Hash value for the mccs parent. Used to identify data corruption on the unmanged system
        /// </summary>
        public string MccsParentHash { get; set; }

        /// <summary>
        /// The mccs package context key
        /// </summary>
        [ForeignKey(nameof(McsPackage))]
        public int McsPackageId { get; set; }

        /// <summary>
        /// The mcs structure component context key
        /// </summary>
        [ForeignKey(nameof(McsStructure))]
        public int McsStructureId { get; set; }

        /// <summary>
        /// The mcs transition component context key
        /// </summary>
        [ForeignKey(nameof(McsTransitions))]
        public int McsTransitionsId { get; set; }

        /// <summary>
        /// The mcs energy component context key
        /// </summary>
        [ForeignKey(nameof(McsEnergies))]
        public int McsEnergiesId { get; set; }

        /// <summary>
        /// The mccs parent result cotext key
        /// </summary>
        [ForeignKey(nameof(McsParentResult))]
        public int McsParentResultId { get; set; }

        /// <summary>
        /// The mccs package this parent belongs to
        /// </summary>
        public McsPackage McsPackage { get; set; }

        /// <summary>
        /// The mcs structure entity of this simulation parent
        /// </summary>
        public McsStructure McsStructure { get; set; }

        /// <summary>
        /// The mcs transition entity of this simulation parent
        /// </summary>
        public McsTransitions McsTransitions { get; set; }

        /// <summary>
        /// The mcs energy entity of this simulation parent
        /// </summary>
        public McsEnergies McsEnergies { get; set; }

        /// <summary>
        /// The mcs parent result entity. Stores collective results information
        /// </summary>
        public McsParentResult McsParentResult { get; set; }
    }
}
