using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class TransitionModel : InteropEntityBase
    {
        private static IList<InteropStateChangeAction> stateChangeDelegates;

        protected override IList<InteropStateChangeAction> StateChangeActions
        {
            get => stateChangeDelegates;
            set => stateChangeDelegates = value;
        }

        public SimulationPackage SimulationPackage { get; set; }

        public List<JumpCollectionEntity> JumpCollections { get; set; }

        public List<JumpDirectionEntity> JumpDirections { get; set; }

        [Column("PackageId")]
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        [Column("JumpCountTable")]
        public byte[] JumpCountTableBinary { get; set; }

        [Column("JumpAssignTable")]
        public byte[] JumpAssignTableBinary { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(JumpCountTableBinary))]
        public JumpCountTableEntity JumpCountTable { get; set; }

        [NotMapped]
        [OwnedBlobProperty(nameof(JumpAssignTableBinary))]
        public JumpAssignTableEntity JumpAssignTable { get; set; }
    }
}
