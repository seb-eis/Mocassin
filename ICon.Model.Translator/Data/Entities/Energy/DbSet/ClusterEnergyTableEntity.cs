using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class ClusterEnergyTableEntity : EnergyModelComponentBase
    {
        [NotMapped]
        [InteropProperty(nameof(ParticleToTableIdsBinary))]
        public ByteBuffer64 ParticleToTableIds { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(EnergyTableBinary), nameof(EnergyTableBinarySize))]
        public EnergyTableEntity EnergyTable { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(OccupationCodeListBinary), nameof(OccupationCodeListBinarySize))]
        public OccupationCodeListEntity OccupationCodeList { get; set; }

        [Column("ObjectId")]
        public int ObjectId { get; set; }

        [Column("EnergyTable")]
        public int EnergyTableBinarySize { get; set; }

        [Column("EnergyTableSize")]
        public byte[] EnergyTableBinary { get; set; }

        [Column("OccupationCodesSize")]
        public int OccupationCodeListBinarySize { get; set; }

        [Column("OccupationCodes")]
        public byte[] OccupationCodeListBinary { get; set; }

        [Column("TableIndexing")]
        public byte[] ParticleToTableIdsBinary { get; set; }
    }
}
