using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     The simulation energy model. Stores all energy information that is required for a monte carlo simulation
    /// </summary>
    public class SimulationEnergyModel : InteropEntityBase
    {
        /// <summary>
        ///     Static backing field for the energy state change actions
        /// </summary>
        private static IList<StateChangeAction> _stateChangeDelegates;

        /// <inheritdoc />
        protected override IList<StateChangeAction> StateChangeActions
        {
            get => _stateChangeDelegates;
            set => _stateChangeDelegates = value;
        }

        /// <summary>
        ///     The number of pair energy tables within the energy model
        /// </summary>
        [Column("NumOfPairTables")]
        public int PairEnergyTableCount { get; set; }

        /// <summary>
        ///     The number of cluster energy tables within the energy model
        /// </summary>
        [Column("NumOfClusterTables")]
        public int ClusterEnergyTableCount { get; set; }

        /// <summary>
        ///     Defect background blob conversion backing property
        /// </summary>
        [Column("DefectBackground")]
        public byte[] DefectBackgroundBinary { get; set; }

        /// <summary>
        ///     Get or set the <see cref="DefectBackgroundEntity"/> that stores the 2D structure defect background
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(DefectBackgroundBinary))]
        public DefectBackgroundEntity DefectBackground { get; set; }

        /// <summary>
        ///     The list of affiliated pair energy table entities
        /// </summary>
        public List<PairEnergyTableEntity> PairEnergyTables { get; set; }

        /// <summary>
        ///     The list of affiliated cluster energy table entities
        /// </summary>
        public List<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }
    }
}