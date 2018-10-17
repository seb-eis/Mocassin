using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class EnvironmentDefinitionEntity : StructureModelComponentBase
    {
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        [Column("PositionParticleIds")]
        public byte[] PositionParticleIdsBinary { get; set; }

        [Column("UpdateParticleIds")]
        public byte[] UpdateParticleIdsBinary { get; set; }

        [Column("PairDefinitions")]
        public byte[] PairDefinitionListBinary { get; set; }

        [Column("ClusterDefinitions")]
        public byte[] ClusterDefinitionListBinary { get; set; }

        [NotMapped]
        [InteropProperty(nameof(PositionParticleIdsBinary))]
        public ByteBuffer64 PositionParticleIds { get; set; }

        [NotMapped]
        [InteropProperty(nameof(UpdateParticleIdsBinary))]
        public ByteBuffer64 UpdateParticleIds { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(PairDefinitionListBinary))]
        public PairDefinitionListEntity PairDefinitionList { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(ClusterDefinitionListBinary))]
        public ClusterDefinitionListEntity GetClusterDefinitionList { get; set; }
    }
}
