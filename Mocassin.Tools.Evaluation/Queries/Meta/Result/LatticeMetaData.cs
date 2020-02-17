using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Stores the meta information for a single the simulation lattice
    /// </summary>
    public readonly struct LatticeMetaData
    {
        /// <summary>
        ///     Get a <see cref="CrystalVector4D" /> that stores the 4D size information
        /// </summary>
        public CrystalVector4D SizeInfo { get; }

        /// <summary>
        ///     Get the volume of the lattice in [m^3]
        /// </summary>
        public double Volume { get; }

        public LatticeMetaData(in CrystalVector4D sizeInfo, double volume)
            : this()
        {
            SizeInfo = sizeInfo;
            Volume = volume;
        }

        /// <summary>
        ///     Get the number of unit cells in the lattice
        /// </summary>
        /// <returns></returns>
        public int GetUnitCellCount()
        {
            return SizeInfo.A * SizeInfo.B * SizeInfo.C;
        }

        /// <summary>
        ///     Get the number of positions in the lattice
        /// </summary>
        /// <returns></returns>
        public int GetPositionCount()
        {
            return GetUnitCellCount() * SizeInfo.P;
        }
    }
}