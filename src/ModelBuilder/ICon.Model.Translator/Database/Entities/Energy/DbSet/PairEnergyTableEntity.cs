using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Two dimensional pair energy table entity that stores the pair energy model information of a single pair interaction
    ///     for the simulation database
    /// </summary>
    public class PairEnergyTableEntity : EnergyModelComponentBase
    {
        /// <summary>
        ///     Static backing field for the state change actions of energy model components
        /// </summary>
        private static IList<StateChangeAction> _stateChangeDelegates;

        /// <inheritdoc />
        protected override IList<StateChangeAction> StateChangeActions
        {
            get => _stateChangeDelegates;
            set => _stateChangeDelegates = value;
        }

        /// <summary>
        ///     The object id in the simulation context
        /// </summary>
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        /// <summary>
        ///     The energy table blob conversion property
        /// </summary>
        [Column("EnergyTable")]
        public byte[] EnergyTableBinary { get; set; }

        /// <summary>
        ///     2D energy table that stores energy values for each position id and particle id
        /// </summary>
        [NotMapped, OwnedBlobProperty(nameof(EnergyTableBinary))]
        public EnergyTableEntity EnergyTable { get; set; }
    }
}