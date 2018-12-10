using System;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Job lattice model interop data for a single simulation job that can be stored in the simulation database context
    /// </summary>
    public readonly struct LatticeModelData
    {
        /// <summary>
        ///     The lattice info interop object
        /// </summary>
        public readonly InteropObject<CLatticeInfo> LatticeInfo;

        /// <summary>
        ///     The lattice interop object
        /// </summary>
        public readonly LatticeEntity Lattice;

        /// <summary>
        ///     The energy background interop object
        /// </summary>
        public readonly EnergyBackgroundEntity EnergyBackground;

        /// <summary>
        /// Create new lattice model data from lattice info and lattice entity
        /// </summary>
        /// <param name="latticeInfo"></param>
        /// <param name="lattice"></param>
        public LatticeModelData(InteropObject<CLatticeInfo> latticeInfo, LatticeEntity lattice) : this()
        {
            LatticeInfo = latticeInfo ?? throw new ArgumentNullException(nameof(latticeInfo));
            Lattice = lattice ?? throw new ArgumentNullException(nameof(lattice));
            EnergyBackground = EnergyBackgroundEntity.Empty;
        }
    }
}