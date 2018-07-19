using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Environment cluster that carries an encoded description of sets of atoms that have a unique energetic description
    /// </summary>
    public class EnvironmentCluster : EntityBase
    {
        /// <summary>
        /// The environment entity key
        /// </summary>
        [ForeignKey(nameof(CellPositionEntity))]
        public int CellPositionEntityId { get; set; }

        /// <summary>
        /// Navigation property for the environment this cluster belongs to
        /// </summary>
        public Environment CellPositionEntity { get; set; }

        /// <summary>
        /// The cluster energy table entity key
        /// </summary>
        [ForeignKey(nameof(ClusterEnergyMatrix))]
        public int ClusterEnergyMatrixId { get; set; }

        /// <summary>
        /// Navigation property for the cluster energy table describing the energy model of the cluster
        /// </summary>
        public ClusterEnergyMatrix ClusterEnergyMatrix { get; set; }

        /// <summary>
        /// The position matrix blob entity key
        /// </summary>
        [ForeignKey(nameof(PositionMatrix))]
        public int PositionMatrixId { get; set; }

        /// <summary>
        /// The 1D position index matrix that describes which position indices of the environment form the cluster
        /// </summary>
        public MatrixEntity<int> PositionMatrix { get; set; }
    }
}
