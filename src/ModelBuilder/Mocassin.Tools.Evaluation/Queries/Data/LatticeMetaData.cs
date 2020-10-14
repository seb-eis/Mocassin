using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Stores the meta information for a single the simulation lattice
    /// </summary>
    public readonly struct LatticeMetaData
    {
        /// <summary>
        ///     Get a <see cref="Vector4I" /> that stores the 4D size information
        /// </summary>
        public Vector4I SizeInfo { get; }

        /// <summary>
        ///     Get the volume of the lattice in [m^3]
        /// </summary>
        public double Volume { get; }

        /// <inheritdoc />
        public LatticeMetaData(in Vector4I sizeInfo, double volume)
            : this()
        {
            SizeInfo = sizeInfo;
            Volume = volume;
        }

        /// <summary>
        ///     Get the number of unit cells in the lattice
        /// </summary>
        /// <returns></returns>
        public int GetUnitCellCount() => SizeInfo.A * SizeInfo.B * SizeInfo.C;

        /// <summary>
        ///     Get the number of positions in the lattice
        /// </summary>
        /// <returns></returns>
        public int GetPositionCount() => GetUnitCellCount() * SizeInfo.P;
    }
}