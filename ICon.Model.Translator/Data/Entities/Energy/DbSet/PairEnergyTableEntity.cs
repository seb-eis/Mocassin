using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class PairEnergyTableEntity : EnergyModelComponentBase
    {
        public int ObjectId { get; set; }

        public byte[] EnergyTableBinary { get; set; }

        [OwendBlobProperty(nameof(EnergyTableBinary))]
        EnergyTableEntity EnergyTable { get; set; }
    }
}
