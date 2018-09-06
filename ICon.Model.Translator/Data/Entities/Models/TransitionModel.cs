using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class TransitionModel : InteropEntityBase
    {
        public SimulationPackage SimulationPackage { get; set; }

        public List<JumpCollectionEntity> JumpCollections { get; set; }

        public List<JumpDirectionEntity> JumpDirections { get; set; }

        [Column("PackageId")]
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        [Column("JumpCounTableSize")]
        public int JumpCountTableBinarySize { get; set; }

        [Column("JumpCountTable")]
        public byte[] JumpCountTableBinary { get; set; }

        [Column("JumpAssignTableSize")]
        public int JumpAssignTableBinarySize { get; set; }

        [Column("JumpAssignTable")]
        public byte[] JumpAssignTableBinary { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(JumpCountTableBinary), nameof(JumpCountTableBinarySize))]
        public JumpCountTableEntity JumpCountTable { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(JumpAssignTableBinary), nameof(JumpAssignTableBinarySize))]
        public JumpAssignTableEntity JumpAssignTable { get; set; }
    }
}
