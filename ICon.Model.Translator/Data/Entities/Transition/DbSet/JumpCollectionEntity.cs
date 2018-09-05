using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class JumpCollectionEntity : TransitionModelComponentBase
    {
        public int ObjectId { get; set; }

        public long ParticleMask { get; set; }

        public byte[] JumpRuleListBinary { get; set; }

        [OwendBlobProperty(nameof(JumpRuleListBinary))]
        public JumpRuleListEntity JumpRuleList { get; set; }

        public List<JumpDirectionEntity> JumpDirections { get; set; }

    }
}
