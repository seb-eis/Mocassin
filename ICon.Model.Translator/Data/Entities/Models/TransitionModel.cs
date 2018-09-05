using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class TransitionModel : InteropEntityBase
    {
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        public SimulationPackage SimulationPackage { get; set; }

        public List<JumpCollectionEntity> JumpCollections { get; set; }

        public List<JumpDirectionEntity> JumpDirections { get; set; }

        public byte[] JumpCountTableBinary { get; set; }

        public byte[] JumpAssignTableBinary { get; set; }

        [OwendBlobProperty(nameof(JumpCountTableBinary))]
        public JumpCountTableEntity JumpCountTable { get; set; }

        [OwendBlobProperty(nameof(JumpAssignTableBinary))]
        public JumpAssignTableEntity JumpAssignTable { get; set; }
    }
}
