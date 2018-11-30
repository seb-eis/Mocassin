using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     The simulation structure model. Defines and stores the structural information for the simulation database
    /// </summary>
    public class StructureModel : InteropEntityBase
    {
        /// <summary>
        ///     Static backing filed for the state change actions
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
        public JobPackageModel JobPackageModel { get; set; }

        /// <summary>
        ///     The simulation package context key
        /// </summary>
        [Column("PackageId")]
        [ForeignKey(nameof(JobPackageModel))]
        public int SimulationPackageId { get; set; }

        /// <summary>
        ///     The number of static tracker objects per unit cell
        /// </summary>
        [Column("NumOfTrackersPerCell")]
        public int NumOfTrackersPerCell { get; set; }

        /// <summary>
        ///     The number of global tracking objects
        /// </summary>
        [Column("NumOfGlobalTrackers")]
        public int NumOfGlobalTrackers { get; set; }

        /// <summary>
        ///     The number of probability trackers
        /// </summary>
        [Column("NumOfProbabilityTrackers")]
        public int NumOfProbabilityTrackers { get; set; }

        /// <summary>
        ///     Interaction range blob conversion backing property
        /// </summary>
        [Column("InteractionRange")]
        public byte[] InteractionRangeBinary { get; set; }

        /// <summary>
        ///     The list of affiliated environment definition entities
        /// </summary>
        public List<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        /// <summary>
        ///     The interaction range object for regular environment objects
        /// </summary>
        [NotMapped]
        [InteropProperty(nameof(InteractionRangeBinary))]
        public InteractionRange InteractionRange { get; set; }
    }
}