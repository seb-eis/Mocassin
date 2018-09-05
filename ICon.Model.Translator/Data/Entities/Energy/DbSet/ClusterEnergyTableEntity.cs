using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class ClusterEnergyTableEntity : EnergyModelComponentBase
    {
        public int ObjectId { get; set; }

        public byte[] EnergyTableBinary { get; set; }

        public byte[] OccupationCodeListBinary { get; set; }

        public byte[] ParticleToTableIdsBinary { get; set; }

        [OwendBlobProperty(nameof(ParticleToTableIdsBinary))]
        public IndexRedirectionListEntity ParticleToTableIds { get; set; }

        [OwendBlobProperty(nameof(EnergyTableBinary))]
        public EnergyTableEntity EnergyTable { get; set; }

        [OwendBlobProperty(nameof(OccupationCodeListBinary))]
        public OccupationCodeListEntity OccupationCodeList { get; set; }
    }
}
