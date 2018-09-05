using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class JumpDirectionEntity : TransitionModelComponentBase
    {
        [ForeignKey(nameof(JumpCollection))]
        public int JumpCollectionId { get; set; }

        public JumpCollectionEntity JumpCollection { get; set; }

        public int ObjectId { get; set; }

        public int PositionId { get; set; }

        public int CollectionId { get; set; }

        public int JumpLength { get; set; }

        public double FieldProjectionFactor { get; set; }

        public byte[] JumpSequenceBinary { get; set; }

        public byte[] JumpLinkSequenceBinary { get; set; }

        public byte[] LocalMoveSequenceBinary { get; set; }

        public byte[] GlobalMoveSequenceBinary { get; set; }

        [OwendBlobProperty(nameof(JumpSequenceBinary))]
        public JumpSequenceEntity JumpSequence { get; set; }

        [OwendBlobProperty(nameof(JumpLinkSequenceBinary))]
        public JumpLinkSequenceEntity JumpLinkSequence { get; set; }

        [OwendBlobProperty(nameof(LocalMoveSequenceBinary))]
        public MoveSequenceEntity LocalMoveSequence { get; set; }

        [OwendBlobProperty(nameof(GlobalMoveSequenceBinary))]
        public MoveSequenceEntity GlobalMoveSequence { get; set; }
    }
}
