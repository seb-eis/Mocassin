using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation jump direction entity. Defines and stores a single transition behavior for the simulation database
    /// </summary>
    public class JumpDirectionEntity : TransitionModelComponentBase
    {
        /// <summary>
        ///     Static backing field for the transition model state change actions
        /// </summary>
        private static IList<StateChangeAction> _stateChangeDelegates;

        /// <inheritdoc />
        protected override IList<StateChangeAction> StateChangeActions
        {
            get => _stateChangeDelegates;
            set => _stateChangeDelegates = value;
        }

        /// <summary>
        ///     The jump collection navigation property
        /// </summary>
        public JumpCollectionEntity JumpCollection { get; set; }

        /// <summary>
        ///     The jump sequence that describes the jump geometry relative to the origin point
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(JumpSequenceBinary))]
        public JumpSequenceEntity JumpSequence { get; set; }

        /// <summary>
        ///     The movement sequence that describes the data for the movement tracking system
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(MovementSequenceBinary))]
        public MoveSequenceEntity MovementSequence { get; set; }

        /// <summary>
        ///     The jump collection context key
        /// </summary>
        [Column("JumpCollectionId")]
        [ForeignKey(nameof(JumpCollection))]
        public int JumpCollectionId { get; set; }

        /// <summary>
        ///     The object id in the simulation context
        /// </summary>
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        /// <summary>
        ///     The position id the jump direction in the simulation context is valid for
        /// </summary>
        [Column("PositionId")]
        public int PositionId { get; set; }

        /// <summary>
        ///     The jump collection id in the simulation context
        /// </summary>
        [Column("CollectionId")]
        public int CollectionId { get; set; }

        /// <summary>
        ///     The number of positions in the jump direction
        /// </summary>
        [Column("JumpLength")]
        public int PathLength { get; set; }

        /// <summary>
        ///     The electric field projection factor of the jump
        /// </summary>
        [Column("FieldProjection")]
        public double FieldProjectionFactor { get; set; }

        /// <summary>
        ///     The jump sequence blob conversion backing property
        /// </summary>
        [Column("JumpSequence")]
        public byte[] JumpSequenceBinary { get; set; }

        /// <summary>
        ///     The movement sequence blob conversion backing property
        /// </summary>
        [Column("LocalMoveSequence")]
        public byte[] MovementSequenceBinary { get; set; }
    }
}