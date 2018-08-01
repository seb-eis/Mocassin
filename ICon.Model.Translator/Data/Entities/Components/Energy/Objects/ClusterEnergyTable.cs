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
    public class ClusterEnergyTable : EntityBase
    {
        /// <summary>
        /// The simulation object id for the unmanaged simulation context
        /// </summary>
        public int McsObjectId { get; set; }

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
        /// The code matrix entity id
        /// </summary>
        [ForeignKey(nameof(CodeMatrix))]
        public int CodeMatrixId { get; set; }

        /// <summary>
        /// The cluster energy code 1D matrix. Contains all known coded surrounding descriptions
        /// </summary>
        public MatrixEntity<ulong> CodeMatrix { get; set; }

        /// <summary>
        /// The energy matrix entity id
        /// </summary>
        [ForeignKey(nameof(EnergyMatrix))]
        public int EnergyMatrixId { get; set; }

        /// <summary>
        /// The cluster energy 2D matrix. Assigns each possible center particle a list of energy values for the coded surroundings
        /// </summary>
        public MatrixEntity<double> EnergyMatrix { get; set; }

        /// <summary>
        /// The particle to energy indexing matrix entity id
        /// </summary>
        [ForeignKey(nameof(ParticleIndexMatrix))]
        public int ParticleIndexMatrixId { get; set; }

        /// <summary>
        /// The particle ID to energy matrix block index list. Redirects particle indices to energy matrix block indices
        /// </summary>
        /// <remarks>Contain the value -1 for non supported center particle indices</remarks>
        public MatrixEntity<int> ParticleIndexMatrix { get; set; }
    }
}
