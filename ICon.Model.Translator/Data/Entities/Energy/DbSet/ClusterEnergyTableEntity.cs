using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class ClusterEnergyTableEntity : EnergyModelComponentBase
    {
        [NotMapped]
        [InteropProperty(nameof(ParticleToTableIdsBinary))]
        public ByteBuffer64 ParticleToTableIds { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(EnergyTableBinary))]
        public EnergyTableEntity EnergyTable { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(OccupationCodeListBinary))]
        public OccupationCodeListEntity OccupationCodeList { get; set; }

        [Column("ObjectId")]
        public int ObjectId { get; set; }

        [Column("EnergyTable")]
        public byte[] EnergyTableBinary { get; set; }

        [Column("OccupationCodes")]
        public byte[] OccupationCodeListBinary { get; set; }

        [Column("TableIndexing")]
        public byte[] ParticleToTableIdsBinary { get; set; }
    }
}
