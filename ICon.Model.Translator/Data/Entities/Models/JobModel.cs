using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class JobModel : InteropEntityBase
    {
        public SimulationPackage SimulationPackage { get; set; }

        public StructureModel StructureModel { get; set; }

        public EnergyModel EnergyModel { get; set; }

        public TransitionModel TransitionModel { get; set; }

        public LatticeModel LatticeModel { get; set; }

        [Column("PackageId")]
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        [Column("LatticeModelId")]
        [ForeignKey(nameof(LatticeModel))]
        public int LatticeModelId { get; set; }

        [Column("StructureModelId")]
        [ForeignKey(nameof(StructureModel))]
        public int StructureModelId { get; set; }

        [Column("EnergyModelId")]
        [ForeignKey(nameof(EnergyModel))]
        public int EnergyModelId { get; set; }

        [Column("TransitionModelId")]
        [ForeignKey(nameof(TransitionModel))]
        public int TransitionModelId { get; set; }

        [Column("JobInfo")]
        public byte[] JobInfoBinary { get; set; }

        [Column("JobHeader")]
        public byte[] JobHeaderBinary { get; set; }

        [Column("SimulationState")]
        public byte[] SimulationState { get; set; }

        [NotMapped]
        [InteropProperty(nameof(JobInfoBinary))]
        public JobInfo JobInfo { get; set; }

        [NotMapped]
        [InteropProperty(nameof(JobHeaderBinary))]
        public InteropObject JobHeader { get; set; }
    }
}
