using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     The simulation job model. Defines and stores the specifications and dependencies for a single simulation job run
    /// </summary>
    public class JobModel : InteropEntityBase
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
        public SimulationPackage SimulationPackage { get; set; }

        /// <summary>
        ///     The structure model navigation property
        /// </summary>
        public StructureModel StructureModel { get; set; }

        /// <summary>
        ///     The energy model navigation property
        /// </summary>
        public EnergyModel EnergyModel { get; set; }

        /// <summary>
        ///     The transition model navigation property
        /// </summary>
        public TransitionModel TransitionModel { get; set; }

        /// <summary>
        ///     The lattice model navigation property
        /// </summary>
        public LatticeModel LatticeModel { get; set; }

        /// <summary>
        ///     The job number in the simulation package
        /// </summary>
        [Column("JobNumber")]
        public int JobNumber { get; set; }

        /// <summary>
        ///     The simulation package context id
        /// </summary>
        [Column("PackageId")]
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        /// <summary>
        ///     The lattice model context id
        /// </summary>
        [Column("LatticeModelId")]
        [ForeignKey(nameof(LatticeModel))]
        public int LatticeModelId { get; set; }

        /// <summary>
        ///     The structure model context id
        /// </summary>
        [Column("StructureModelId")]
        [ForeignKey(nameof(StructureModel))]
        public int StructureModelId { get; set; }

        /// <summary>
        ///     The energy model context id
        /// </summary>
        [Column("EnergyModelId")]
        [ForeignKey(nameof(EnergyModel))]
        public int EnergyModelId { get; set; }

        /// <summary>
        ///     The transition model context id
        /// </summary>
        [Column("TransitionModelId")]
        [ForeignKey(nameof(TransitionModel))]
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
        public JobInfo JobInfo { get; set; }

        /// <summary>
        ///     The job header object that describes the job type and behavior
        /// </summary>
        [NotMapped]
        [InteropProperty(nameof(JobHeaderBinary))]
        public InteropObject JobHeader { get; set; }
    }
}