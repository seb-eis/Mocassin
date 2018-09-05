using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class LatticeModel : InteropEntityBase
    {
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        public SimulationPackage SimulationPackage { get; set; }

        public byte[] LatticeInfoBinary { get; set; }

        public byte[] LatticeBinary { get; set; }

        public byte[] EnergyBackgroundBinary { get; set; }

        [InteropProperty(nameof(LatticeInfoBinary))]
        public LatticeInfo LatticeInfo { get; set; }

        [OwendBlobProperty(nameof(LatticeBinary))]
        public LatticeEntity Lattice { get; set; }

        [OwendBlobProperty(nameof(EnergyBackgroundBinary))]
        public EnergyBackgroundEntity EnergyBackground { get; set; }
    }
}
