using System;
using System.Collections.Generic;
using System.Text;
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
        [ForeignKey(nameof(Environment))]
        public int EnvironmentId { get; set; }

        /// <summary>
        /// Navigation property for the environment this cluster belongs to
        /// </summary>
        public Environment Environment { get; set; }

        /// <summary>
        /// The cluster energy table entity key
        /// </summary>
        [ForeignKey(nameof(ClusterEnergyTable))]
        public int ClusterEnergyTableId { get; set; }

        /// <summary>
        /// Navigation property for the cluster energy table describing the energy model of the cluster
        /// </summary>
        public ClusterEnergyTable ClusterEnergyTable { get; set; }

        /// <summary>
        /// The number of positions in this cluster
        /// </summary>
        public int PositionCount { get; set; }

        /// <summary>
        /// String encoded version of the environment position indices that belong to the cluster
        /// </summary>
        public string PositionIndexingString { get; set; }
    }
}
