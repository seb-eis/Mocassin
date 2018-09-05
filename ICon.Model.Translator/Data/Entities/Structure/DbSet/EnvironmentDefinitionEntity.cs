using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class EnvironmentDefinitionEntity : StructureModelComponentBase
    {
        public int ObjectId { get; set; }

        public byte[] PositionParticleIdsBinary { get; set; }

        public byte[] UpdateableParticleIdsBinary { get; set; }

        public byte[] PairDefinitionListBinary { get; set; }

        public byte[] ClusterDefinitionListBinary { get; set; }

        [InteropProperty(nameof(PositionParticleIdsBinary))]
        public ByteBuffer64 PositionParticleIds { get; set; }

        [InteropProperty(nameof(UpdateableParticleIdsBinary))]
        public ByteBuffer64 UpdateableParticleIds { get; set; }

        [OwendBlobProperty(nameof(PairDefinitionListBinary))]
        public PairDefinitionListEntity PairDefinitionList { get; set; }

        [OwendBlobProperty(nameof(ClusterDefinitionListBinary))]
        public ClusterDefinitionListEntity GetClusterDefinitionList { get; set; }
    }
}
