using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     The simulation job model. Defines and stores the specifications and dependencies for a single simulation job run
    /// </summary>
    public class SimulationJobModel : InteropEntityBase
    {
        /// <summary>
        ///     Static backing field for the state change actions
        /// </summary>
        private static IList<StateChangeAction> _stateChangeDelegates;

        /// <inheritdoc />
        protected override IList<StateChangeAction> StateChangeActions
        {
            get => _stateChangeDelegates;
            set => _stateChangeDelegates = value;
        }

        /// <summary>
        ///     The simulation package navigation property
        /// </summary>
        public SimulationJobPackageModel SimulationJobPackageModel { get; set; }

        /// <summary>
        ///     The structure model navigation property
        /// </summary>
        public SimulationStructureModel SimulationStructureModel { get; set; }

        /// <summary>
        ///     The energy model navigation property
        /// </summary>
        public SimulationEnergyModel SimulationEnergyModel { get; set; }

        /// <summary>
        ///     The transition model navigation property
        /// </summary>
        public SimulationTransitionModel SimulationTransitionModel { get; set; }

        /// <summary>
        ///     The lattice model navigation property
        /// </summary>
        public SimulationLatticeModel SimulationLatticeModel { get; set; }

        /// <summary>
        ///     The meta data navigation property
        /// </summary>
        public JobMetaDataEntity JobMetaData { get; set; }

        /// <summary>
        ///     The simulation package context id
        /// </summary>
        [Column("PackageId")]
        [ForeignKey(nameof(SimulationJobPackageModel))]
        public int SimulationPackageId { get; set; }

        /// <summary>
        ///     The lattice model context id
        /// </summary>
        [Column("LatticeModelId")]
        [ForeignKey(nameof(SimulationLatticeModel))]
        public int LatticeModelId { get; set; }

        /// <summary>
        ///     The structure model context id
        /// </summary>
        [Column("StructureModelId")]
        [ForeignKey(nameof(SimulationStructureModel))]
        public int StructureModelId { get; set; }

        /// <summary>
        ///     The energy model context id
        /// </summary>
        [Column("EnergyModelId")]
        [ForeignKey(nameof(SimulationEnergyModel))]
        public int EnergyModelId { get; set; }

        /// <summary>
        ///     The transition model context id
        /// </summary>
        [Column("TransitionModelId")]
        [ForeignKey(nameof(SimulationTransitionModel))]
        public int TransitionModelId { get; set; }

        /// <summary>
        ///     Job info blob conversion backing property
        /// </summary>
        [Column("JobInfo")]
        public byte[] JobInfoBinary { get; set; }

        /// <summary>
        ///     Hob header blob conversion backing property
        /// </summary>
        [Column("JobHeader")]
        public byte[] JobHeaderBinary { get; set; }

        /// <summary>
        ///     Simulation state blob conversion backing property
        /// </summary>
        [Column("SimulationState")]
        public byte[] SimulationState { get; set; }

        /// <summary>
        ///     The job info object that describes the job specifications
        /// </summary>
        [NotMapped]
        [InteropProperty(nameof(JobInfoBinary))]
        public InteropObject<CJobInfo> JobInfo { get; set; }

        /// <summary>
        ///     The job header object that describes the job type and behavior
        /// </summary>
        [NotMapped]
        [InteropProperty(nameof(JobHeaderBinary))]
        public InteropObject JobHeader { get; set; }
    }
}