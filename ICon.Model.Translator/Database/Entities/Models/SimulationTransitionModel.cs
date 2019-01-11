using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     The simulation transition model. Defines and stores all transition information for the simulation database
    /// </summary>
    public class SimulationTransitionModel : InteropEntityBase
    {
        /// <summary>
        ///     Static backing field for the state change delegates
        /// </summary>
        private static IList<StateChangeAction> _stateChangeDelegates;

        /// <inheritdoc />
        protected override IList<StateChangeAction> StateChangeActions
        {
            get => _stateChangeDelegates;
            set => _stateChangeDelegates = value;
        }

        /// <summary>
        ///     The list of affiliated jump collection entities
        /// </summary>
        public List<JumpCollectionEntity> JumpCollections { get; set; }

        /// <summary>
        ///     The list of affiliated jump direction entities
        /// </summary>
        public List<JumpDirectionEntity> JumpDirections { get; set; }

        /// <summary>
        ///     Jump count table blob conversion backing property
        /// </summary>
        [Column("JumpCountTable")]
        public byte[] JumpCountTableBinary { get; set; }

        /// <summary>
        ///     Jump assign table blob conversion backing property
        /// </summary>
        [Column("JumpMappingTable")]
        public byte[] JumpAssignTableBinary { get; set; }

        /// <summary>
        ///     Static tracker assign table blob conversion backing property
        /// </summary>
        [Column("StaticTrackerMapping")]
        public byte[] StaticTrackerAssignTableBinary { get; set; }

        /// <summary>
        ///     Global tracker assign table blob conversion backing property
        /// </summary>
        [Column("GlobalTrackerMapping")]
        public byte[] GlobalTrackerAssignTableBinary { get; set; }

        /// <summary>
        ///     The 2D jump count table that assigns each position id/particle id combination the number of possible jumps
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(JumpCountTableBinary))]
        public JumpCountTableEntity JumpCountTable { get; set; }

        /// <summary>
        ///     The 3D jump assign table that assigns each position id/particle id combination its set of affiliated jump direction
        ///     ids
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(JumpAssignTableBinary))]
        public JumpAssignTableEntity JumpAssignTable { get; set; }

        /// <summary>
        ///     The 2D static tracker mapping table that assign each position id/particle id combination a static tracker index
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(StaticTrackerAssignTableBinary))]
        public TrackerAssignTableEntity StaticTrackerAssignTable { get; set; }

        /// <summary>
        ///     The 2D global tracker mapping table that assigns each jump collection id/particle id combination a global tracker
        ///     index
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(GlobalTrackerAssignTableBinary))]
        public TrackerAssignTableEntity GlobalTrackerAssignTable { get; set; }
    }
}