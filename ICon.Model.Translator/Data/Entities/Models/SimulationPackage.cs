using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    public class SimulationPackage : InteropEntityBase
    {
        private static IList<InteropStateChangeAction> stateChangeDelegates;

        protected override IList<InteropStateChangeAction> StateChangeActions
        {
            get => stateChangeDelegates;
            set => stateChangeDelegates = value;
        }

        public StructureModel StructureModel { get; set; }

        public TransitionModel TransitionModel { get; set; }

        public EnergyModel EnergyModel { get; set; }

        public List<JobModel> JobModels { get; set; }

        public List<LatticeModel> LatticeModels { get; set; }

        [Column("StructureModelId")]
        [ForeignKey(nameof(StructureModel))]
        public int StructureModelId { get; set; }

        [Column("TransitionModelId")]
        [ForeignKey(nameof(TransitionModel))]
        public int TransitionModelId { get; set; }

        [Column("EnergyModelId")]
        [ForeignKey(nameof(EnergyModel))]
        public int EnergyModelId { get; set; }

        [Column("ModelSystemVersion")]
        public string ModelSystemVersion { get; set; }

        [Column("PackageGuid")]
        public string PackageGuid { get; set; }

        [Column("ProjectGuid")]
        public string ProjectGuid { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("ProjectXml")]
        public string ProjectXml { get; set; }
    }
}
