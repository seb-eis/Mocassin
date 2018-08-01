using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// A mcs jump collection that carries all explicit jump directions for a specific KMC or MMC transition
    /// </summary>
    public class JumpCollection : EntityBase
    {
        /// <summary>
        /// The simulation object id
        /// </summary>
        public int McsObjectId { get; set; }

        /// <summary>
        /// The unsigned long particle index mask. Describes which starting elements are supported by the jump collection
        /// </summary>
        public ulong ParticleMask { get; set; }

        /// <summary>
        /// The list of al affiliated jump directions for all supported cell positions
        /// </summary>
        public List<JumpDirection> JumpDirections { get; set; }

        /// <summary>
        /// The jump rule matrix entity id
        /// </summary>
        [ForeignKey(nameof(JumpRuleMatrix))]
        public int JumpRuleMatrixId { get; set; }

        /// <summary>
        /// The 1D jump rule matrix. Contains all encoded rule descriptions for the jump collection
        /// </summary>
        public MatrixEntity<JumpRule> JumpRuleMatrix { get; set; }
    }
}
