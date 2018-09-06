using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class LatticeModel : InteropEntityBase
    {
        public SimulationPackage SimulationPackage { get; set; }

        [Column("PackageId")]
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        [Column("LatticeInfo")]
        public byte[] LatticeInfoBinary { get; set; }

        [Column("LatticeSize")]
        public int LatticeBinarySize { get; set; }

        [Column("Lattice")]
        public byte[] LatticeBinary { get; set; }

        [Column("EnergyBackgroundSize")]
        public int EnergyBackgroundBinarySize { get; set; }

        [Column("EnergyBackground")]
        public byte[] EnergyBackgroundBinary { get; set; }

        [NotMapped]
        [InteropProperty(nameof(LatticeInfoBinary))]
        public LatticeInfo LatticeInfo { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(LatticeBinary), nameof(LatticeBinarySize))]
        public LatticeEntity Lattice { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(EnergyBackgroundBinary), nameof(EnergyBackgroundBinarySize))]
        public EnergyBackgroundEntity EnergyBackground { get; set; }
    }
}
