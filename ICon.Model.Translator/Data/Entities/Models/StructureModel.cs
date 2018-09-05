using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class StructureModel : InteropEntityBase
    {
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        public SimulationPackage SimulationPackage { get; set; }

        public int PerCellMovementTrackerCount { get; set; }

        public int GlobalMovementTrackerCount { get; set; }

        public byte[] PositionIdsToCellTrackerIdOffsetBinary { get; set; }

        [OwendBlobProperty(nameof(PositionIdsToCellTrackerIdOffsetBinary))]
        public IndexRedirectionListEntity PositionIdsToCellTrackerIdOffset { get; set; }

        public List<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }
    }
}
