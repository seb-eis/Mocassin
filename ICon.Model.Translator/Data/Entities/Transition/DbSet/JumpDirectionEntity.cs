using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class JumpDirectionEntity : TransitionModelComponentBase
    {
        public JumpCollectionEntity JumpCollection { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(JumpSequenceBinary), nameof(JumpSequenceBinarySize))]
        public JumpSequenceEntity JumpSequence { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(JumpLinkSequenceBinary), nameof(JumpLinkSequenceBinarySize))]
        public JumpLinkSequenceEntity JumpLinkSequence { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(LocalMoveSequenceBinary), nameof(LocalMoveSequenceBinarySize))]
        public MoveSequenceEntity LocalMoveSequence { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(GlobalMoveSequenceBinary), nameof(GlobalMoveSequenceBinarySize))]
        public MoveSequenceEntity GlobalMoveSequence { get; set; }

        [Column("JumpCollectionId")]
        [ForeignKey(nameof(JumpCollection))]
        public int JumpCollectionId { get; set; }

        [Column("ObjectId")]
        public int ObjectId { get; set; }

        [Column("PositionId")]
        public int PositionId { get; set; }

        [Column("CollectionId")]
        public int CollectionId { get; set; }

        [Column("JumpLength")]
        public int JumpLength { get; set; }

        [Column("FieldProjection")]
        public double FieldProjectionFactor { get; set; }

        [Column("JumpSequenceSize")]
        public int JumpSequenceBinarySize { get; set; }

        [Column("JumpSequence")]
        public byte[] JumpSequenceBinary { get; set; }

        [Column("JumpLinkSequenceSize")]
        public int JumpLinkSequenceBinarySize { get; set; }

        [Column("JumpLinkSequence")]
        public byte[] JumpLinkSequenceBinary { get; set; }

        [Column("LocalMoveSequenceSize")]
        public int LocalMoveSequenceBinarySize { get; set; }

        [Column("LocalMoveSequence")]
        public byte[] LocalMoveSequenceBinary { get; set; }

        [Column("GlobalMoveSequenceSize")]
        public int GlobalMoveSequenceBinarySize { get; set; }

        [Column("GlobalMoveSequence")]
        public byte[] GlobalMoveSequenceBinary { get; set; }
    }
}
