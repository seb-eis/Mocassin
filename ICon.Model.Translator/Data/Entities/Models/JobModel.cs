using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class JobModel : InteropEntityBase
    {
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        public SimulationPackage SimulationPackage { get; set; }

        [ForeignKey(nameof(LatticeModel))]
        public int LatticeModelId { get; set; }

        public LatticeModel LatticeModel { get; set; }

        public byte[] JobInfoBinary { get; set; }

        public byte[] JobHeaderBinary { get; set; }

        public byte[] SimulationState { get; set; }

        [InteropProperty(nameof(JobInfoBinary))]
        public JobInfo JobInfo { get; set; }

        [InteropProperty(nameof(JobHeaderBinary))]
        public InteropObjectBase JobHeader { get; set; }
    }
}
