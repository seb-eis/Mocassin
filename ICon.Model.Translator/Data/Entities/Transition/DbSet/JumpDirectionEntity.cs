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
        [OwnedBlobProperty(nameof(JumpSequenceBinary))]
        public JumpSequenceEntity JumpSequence { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(JumpLinkSequenceBinary))]
        public JumpLinkSequenceEntity JumpLinkSequence { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(LocalMoveSequenceBinary))]
        public MoveSequenceEntity LocalMoveSequence { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(GlobalMoveSequenceBinary))]
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

        [Column("JumpSequence")]
        public byte[] JumpSequenceBinary { get; set; }

        [Column("JumpLinkSequence")]
        public byte[] JumpLinkSequenceBinary { get; set; }

        [Column("LocalMoveSequence")]
        public byte[] LocalMoveSequenceBinary { get; set; }

        [Column("GlobalMoveSequence")]
        public byte[] GlobalMoveSequenceBinary { get; set; }
    }
}
