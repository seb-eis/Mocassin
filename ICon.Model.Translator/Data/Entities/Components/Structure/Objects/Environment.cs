using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Cell position entity that describes a simulated
    /// </summary>
    public class Environment : EntityBase
    {
        /// <summary>
        /// The mcs structure info entity key
        /// </summary>
        [ForeignKey(nameof(McsStructure))]
        public int McsStructureId { get; set; }

        /// <summary>
        /// Navigation property for the mcs structure info this environment belongs to
        /// </summary>
        public McsStructure McsStructure { get; set; }

        /// <summary>
        /// The pair matrix entity id
        /// </summary>
        [ForeignKey(nameof(PairMatrix))]
        public int PairMatrixId { get; set; }

        /// <summary>
        /// The 1D pair interaction matrix that describes all positions by relative 4D vector information and pair table index
        /// </summary>
        public MatrixEntity<PairInteraction> PairMatrix { get; set; }

        /// <summary>
        /// The cluster matrix entity id
        /// </summary>
        [ForeignKey(nameof(ClusterMatrix))]
        public int ClusterMatrixId { get; set; }

        /// <summary>
        /// The 1D cluster interaction matrix that describes all clusters as blocks of relative pair interaction indices and a cluster table index
        /// </summary>
        public MatrixEntity<ClusterInteraction> ClusterMatrix { get; set; }
    }
}
