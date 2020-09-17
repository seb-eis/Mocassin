using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     The simulation lattice model. Defines and stores the state of the simulation lattice occupation
    /// </summary>
    public class SimulationLatticeModel : InteropEntityBase
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
        ///     The simulation package context key
        /// </summary>
        [Column("PackageId"), ForeignKey(nameof(SimulationJobPackageModel))]
        public int SimulationPackageId { get; set; }

        /// <summary>
        ///     Lattice info blob conversion backing property
        /// </summary>
        [Column("LatticeInfo")]
        public byte[] LatticeInfoBinary { get; set; }

        /// <summary>
        ///     Lattice state blob conversion backing property
        /// </summary>
        [Column("Lattice")]
        public byte[] LatticeBinary { get; set; }

        /// <summary>
        ///     Energy background blob conversion backing property
        /// </summary>
        [Column("EnergyBackground")]
        public byte[] EnergyBackgroundBinary { get; set; }

        /// <summary>
        ///     The lattice information that carries simulation run infos about the lattice
        /// </summary>
        [NotMapped, InteropProperty(nameof(LatticeInfoBinary))]
        public InteropObject<CLatticeInfo> LatticeInfo { get; set; }

        /// <summary>
        ///     The lattice occupation entity
        /// </summary>
        [NotMapped, OwnedBlobProperty(nameof(LatticeBinary))]
        public LatticeEntity Lattice { get; set; }

        /// <summary>
        ///     The energy background entity
        /// </summary>
        [NotMapped, OwnedBlobProperty(nameof(EnergyBackgroundBinary))]
        public EnergyBackgroundEntity EnergyBackground { get; set; }
    }
}