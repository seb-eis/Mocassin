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
        ///     The simulation package navigation property
        /// </summary>
        public SimulationJobPackageModel SimulationJobPackageModel { get; set; }

        /// <summary>
        ///     The simulation package context id
        /// </summary>
        [Column("PackageId")]
        [ForeignKey(nameof(SimulationJobPackageModel))]
        public int SimulationPackageId { get; set; }

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
        ///     The list of affiliated pair energy table entities
        /// </summary>
        public List<PairEnergyTableEntity> PairEnergyTables { get; set; }

        /// <summary>
        ///     The list of affiliated cluster energy table entities
        /// </summary>
        public List<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }
    }
}