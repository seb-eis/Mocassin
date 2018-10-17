﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class JumpCollectionEntity : TransitionModelComponentBase
    {
        public List<JumpDirectionEntity> JumpDirections { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(JumpRuleListBinary))]
        public JumpRuleListEntity JumpRuleList { get; set; }

        [Column("ObjectId")]
        public int ObjectId { get; set; }

        [Column("ParticleMask")]
        public long ParticleMask { get; set; }

        [Column("JumpRules")]
        public byte[] JumpRuleListBinary { get; set; }
    }
}
