using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    public class SimulationPackage : InteropEntityBase
    {
        public string ModelSystemVersion { get; set; }

        public string PackageGuid { get; set; }

        public string ProjectGuid { get; set; }

        public string Description { get; set; }

        public string ProjectXml { get; set; }

        [ForeignKey(nameof(StructureModel))]
        public int StructureModelId { get; set; }

        [ForeignKey(nameof(TransitionModel))]
        public int TransitionModelId { get; set; }

        [ForeignKey(nameof(EnergyModel))]
        public int EnergyModelId { get; set; }

        public StructureModel StructureModel { get; set; }

        public TransitionModel TransitionModel { get; set; }

        public EnergyModel EnergyModel { get; set; }

        public List<JobModel> JobModels { get; set; }

        public List<LatticeModel> LatticeModels { get; set; }
    }
}
