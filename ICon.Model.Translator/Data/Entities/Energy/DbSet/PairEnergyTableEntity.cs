using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class PairEnergyTableEntity : EnergyModelComponentBase
    {
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        [Column("EnergyTableSize")]
        public int EnergyTableBinarySize { get; set; }

        [Column("EnergyTable")]
        public byte[] EnergyTableBinary { get; set; }

        [OwendBlobProperty(nameof(EnergyTableBinary), nameof(EnergyTableBinarySize))]
        EnergyTableEntity EnergyTable { get; set; }
    }
}
