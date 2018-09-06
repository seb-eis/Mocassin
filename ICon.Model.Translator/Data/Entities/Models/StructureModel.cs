using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class StructureModel : InteropEntityBase
    {
        public SimulationPackage SimulationPackage { get; set; }

        [Column("PackageId")]
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        [Column("NumOfTrackersPerCell")]
        public int NumOfTrackersPerCell { get; set; }

        [Column("NumOfGlobalTrackers")]
        public int NumOfGlobalTrackers { get; set; }

        [Column("CellTrackerIndexingSize")]
        public int PositionCellTrackerIndexingBinarySize { get; set; }

        [Column("CellTrackerIndexing")]
        public byte[] PositionCellTrackerIndexingBinary { get; set; }

        [Column("InteractionRange")]
        public byte[] InteractionRangeBinary { get; set; }

        public List<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        [NotMapped]
        [OwendBlobProperty(nameof(PositionCellTrackerIndexingBinary), nameof(PositionCellTrackerIndexingBinarySize))]
        public IndexRedirectionListEntity PositionCellTrackerIndexing { get; set; }

        [NotMapped]
        [InteropProperty(nameof(InteractionRangeBinary))]
        public InteractionRange InteractionRange { get; set; }
    }
}
