using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Describes a single MMC or KMC transition jump direction. Direction belongs to a jump collection and is only valid for a specific cell position
    /// </summary>
    public class JumpDirection : EntityBase
    {
        /// <summary>
        /// The jump collection entity id
        /// </summary>
        [ForeignKey(nameof(JumpCollection))]
        public int JumpCollectionId { get; set; }

        /// <summary>
        /// Navigation property for the affiliated parent jump collection
        /// </summary>
        public JumpCollection JumpCollection { get; set; }

        /// <summary>
        /// The simulation object id
        /// </summary>
        public int McsObjectId { get; set; }

        /// <summary>
        /// The cell position id this jump direction is valid for
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// The electric filed projection factor from range [-1;1]
        /// </summary>
        public double FieldProjectionFactor { get; set; }

        /// <summary>
        /// The jump matrix entity id
        /// </summary>
        [ForeignKey(nameof(JumpMatrix))]
        public int JumpMatrixId { get; set; }

        /// <summary>
        /// The 1D jump matrix. Describes the jump geometry as N-1 4D vectors relative to the original position
        /// </summary>
        public MatrixEntity<CrystalVector4D> JumpMatrix { get; set; }

        /// <summary>
        /// The local movement matrix entity id
        /// </summary>
        [ForeignKey(nameof(LocalMovementMatrix))]
        public int LocalMovementMatrixId { get; set; }

        /// <summary>
        /// The 1D local movement matrix. Describes which and how position static and dynamic trackers are affected by a successful jump
        /// </summary>
        public MatrixEntity<MoveVector> LocalMovementMatrix { get; set; }

        /// <summary>
        /// The global movement matrix entity id
        /// </summary>
        [ForeignKey(nameof(GlobalMovementMatrix))]
        public int GlobalMovementMatrixId { get; set; }

        /// <summary>
        /// The 1D global movement matrix. Describes which and how global trackers are affected by a successful jump
        /// </summary>
        public MatrixEntity<MoveVector> GlobalMovementMatrix { get; set; }
    }
}
