using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

using ICon.Framework.Collections;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Cluster energy table that carries the energy permutation information for a single position cluster (max. 8 positions) and all center permutations
    /// </summary>
    public class ClusterEnergySet : EntityBase
    {
        /// <summary>
        /// Mccs energy info entity id
        /// </summary>
        [ForeignKey(nameof(MccsEnergyInfo))]
        public int MccsEnergyInfoId { get; set; }

        /// <summary>
        /// Navigation property for the mccs energy info this cluster energy table belongs to
        /// </summary>
        public McsEnergies MccsEnergyInfo { get; set; }

        /// <summary>
        /// The environment cluster entity key
        /// </summary>
        [ForeignKey(nameof(EnvironmentCluster))]
        public int EnvironmentClusterId { get; set; }

        /// <summary>
        /// Navigation property for the environment cluster this energy matrix belongs to
        /// </summary>
        public EnvironmentCluster EnvironmentCluster { get; set; }

        /// <summary>
        /// The index of the particle at the center of the cluster
        /// </summary>
        public int ParticleIndex { get; set; }

        /// <summary>
        /// The code matrix entity id
        /// </summary>
        [ForeignKey(nameof(CodeMatrix))]
        public int CodeMatrixId { get; set; }
     
        /// <summary>
        /// The cluster energy code 1D matrix. Contains all possible codes and their affiliated energy values
        /// </summary>
        public MatrixEntity<EnergyCode> CodeMatrix { get; set; }
    }
}
