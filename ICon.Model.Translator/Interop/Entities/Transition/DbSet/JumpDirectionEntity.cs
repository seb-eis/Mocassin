﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation jump direction entity. Defines and stores a single transition behavior for the simulation database
    /// </summary>
    public class JumpDirectionEntity : TransitionModelComponentBase
    {
        /// <summary>
        ///     The jump collection navigation property
        /// </summary>
        public JumpCollectionEntity JumpCollection { get; set; }

        /// <summary>
        ///     The jump sequence that describes the jump geometry
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(JumpSequenceBinary))]
        public JumpSequenceEntity JumpSequence { get; set; }

        /// <summary>
        ///     The local movement sequence that describes the data for the dynamic tracking system
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(LocalMoveSequenceBinary))]
        public MoveSequenceEntity LocalMoveSequence { get; set; }

        /// <summary>
        ///     The global movement sequence that describes the data for the global tracking
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(GlobalMoveSequenceBinary))]
        public MoveSequenceEntity GlobalMoveSequence { get; set; }

        /// <summary>
        ///     The jump collection context key
        /// </summary>
        [Column("JumpCollectionId")]
        [ForeignKey(nameof(JumpCollection))]
        public int JumpCollectionId { get; set; }

        /// <summary>
        ///     The object id in the simulation context
        /// </summary>
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        /// <summary>
        ///     The position id the jump direction in the simulation context is valid for
        /// </summary>
        [Column("PositionId")]
        public int PositionId { get; set; }

        /// <summary>
        ///     The jump collection id in the simulation context
        /// </summary>
        [Column("CollectionId")]
        public int CollectionId { get; set; }

        /// <summary>
        ///     The number of positions in the jump direction
        /// </summary>
        [Column("JumpLength")]
        public int JumpLength { get; set; }

        /// <summary>
        ///     The electric field projection factor of the jump
        /// </summary>
        [Column("FieldProjection")]
        public double FieldProjectionFactor { get; set; }

        /// <summary>
        ///     The jump sequence blob conversion backing property
        /// </summary>
        [Column("JumpSequence")]
        public byte[] JumpSequenceBinary { get; set; }

        /// <summary>
        ///     The local movement sequence blob conversion backing property
        /// </summary>
        [Column("LocalMoveSequence")]
        public byte[] LocalMoveSequenceBinary { get; set; }

        /// <summary>
        ///     The global movement sequence blob conversion backing property
        /// </summary>
        [Column("GlobalMoveSequence")]
        public byte[] GlobalMoveSequenceBinary { get; set; }
    }
}