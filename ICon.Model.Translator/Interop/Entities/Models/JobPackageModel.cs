using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Job package database entity. Defines and stores a full set of dependent simulation database sets
    /// </summary>
    public class JobPackageModel : InteropEntityBase
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
        ///     The structure model navigation property
        /// </summary>
        public StructureModel StructureModel { get; set; }

        /// <summary>
        ///     The transition model navigation property
        /// </summary>
        public TransitionModel TransitionModel { get; set; }

        /// <summary>
        ///     The energy model navigation property
        /// </summary>
        public EnergyModel EnergyModel { get; set; }

        /// <summary>
        ///     The list of affiliated job models
        /// </summary>
        public List<JobModel> JobModels { get; set; }

        /// <summary>
        ///     The list of affiliated lattice models
        /// </summary>
        public List<LatticeModel> LatticeModels { get; set; }

        /// <summary>
        ///     The structure model context key
        /// </summary>
        [Column("StructureModelId")]
        [ForeignKey(nameof(StructureModel))]
        public int StructureModelId { get; set; }

        /// <summary>
        ///     The transition model context key
        /// </summary>
        [Column("TransitionModelId")]
        [ForeignKey(nameof(TransitionModel))]
        public int TransitionModelId { get; set; }

        /// <summary>
        ///     The energy model context key
        /// </summary>
        [Column("EnergyModelId")]
        [ForeignKey(nameof(EnergyModel))]
        public int EnergyModelId { get; set; }

        /// <summary>
        ///     The model system version string
        /// </summary>
        [Column("ModelSystemVersion")]
        public string ModelSystemVersion { get; set; }

        /// <summary>
        ///     The simulation package guid
        /// </summary>
        [Column("PackageGuid")]
        public string PackageGuid { get; set; }

        /// <summary>
        ///     The project guid the package was created with
        /// </summary>
        [Column("ProjectGuid")]
        public string ProjectGuid { get; set; }

        /// <summary>
        ///     The user defined package description
        /// </summary>
        [Column("Description")]
        public string Description { get; set; }

        /// <summary>
        ///     The full model project xml definition
        /// </summary>
        [Column("ProjectXml")]
        public string ProjectXml { get; set; }
    }
}