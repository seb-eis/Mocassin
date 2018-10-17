using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class LatticeModel : InteropEntityBase
    {
        private static IList<InteropStateChangeAction> stateChangeDelegates;

        protected override IList<InteropStateChangeAction> StateChangeActions
        {
            get => stateChangeDelegates;
            set => stateChangeDelegates = value;
        }

        public SimulationPackage SimulationPackage { get; set; }

        [Column("PackageId")]
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        [Column("LatticeInfo")]
        public byte[] LatticeInfoBinary { get; set; }

        [Column("Lattice")]
        public byte[] LatticeBinary { get; set; }

        [Column("EnergyBackground")]
        public byte[] EnergyBackgroundBinary { get; set; }

        [NotMapped]
        [InteropProperty(nameof(LatticeInfoBinary))]
        public LatticeInfo LatticeInfo { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(LatticeBinary))]
        public LatticeEntity Lattice { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(EnergyBackgroundBinary))]
        public EnergyBackgroundEntity EnergyBackground { get; set; }
    }
}
