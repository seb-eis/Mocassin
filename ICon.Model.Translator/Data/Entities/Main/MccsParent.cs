using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Mccs parent entity that carries the shared encoded base components of a simulation job set
    /// </summary>
    public class MccsParent : EntityBase
    {
        /// <summary>
        /// Hash value for the mccs parent. Used to identify data corruption on the unmanged system
        /// </summary>
        public string MccsParentHash { get; set; }

        /// <summary>
        /// The mccs package context key
        /// </summary>
        [ForeignKey(nameof(MccsPackage))]
        public int PackageKey { get; set; }

        /// <summary>
        /// The mccs meta component context key
        /// </summary>
        [ForeignKey(nameof(MccsPackage))]
        public int MetaKey { get; set; }

        /// <summary>
        /// The mccs structure component context key
        /// </summary>
        [ForeignKey(nameof(MccsPackage))]
        public int StructureKey { get; set; }

        /// <summary>
        /// The mccs transition component context key
        /// </summary>
        [ForeignKey(nameof(MccsPackage))]
        public int TransitionKey { get; set; }

        /// <summary>
        /// The mccs energy component context key
        /// </summary>
        [ForeignKey(nameof(MccsPackage))]
        public int EnergyKey { get; set; }

        /// <summary>
        /// The mccs parent result cotext key
        /// </summary>
        [ForeignKey(nameof(MccsParentResult))]
        public int MccsParentResultKey { get; set; }

        /// <summary>
        /// The mccs package this parent belongs to
        /// </summary>
        public MccsPackage MccsPackage { get; set; }

        /// <summary>
        /// The mccs meta entity of this simulation parent
        /// </summary>
        public MccsMetaEntity MccsMetaEntity { get; set; }

        /// <summary>
        /// The mccs structure entity of this simulation parent
        /// </summary>
        public MccsStructureEntity MccsStructureEntity { get; set; }

        /// <summary>
        /// The mccs transition entity of this simulation parent
        /// </summary>
        public MccsTransitionEntity MccsTransitionEntity { get; set; }

        /// <summary>
        /// The mccs energy entity of this simulation parent
        /// </summary>
        public MccsEnergyEntity MccsEnergyEntity { get; set; }

        /// <summary>
        /// The mccs parent result entity. Stores collective results information
        /// </summary>
        public MccsParentResult MccsParentResult { get; set; }
    }
}
