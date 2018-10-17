using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class PairEnergyTableEntity : EnergyModelComponentBase
    {
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        [Column("EnergyTable")]
        public byte[] EnergyTableBinary { get; set; }

        [OwnedBlobProperty(nameof(EnergyTableBinary))]
        EnergyTableEntity EnergyTable { get; set; }
    }
}
