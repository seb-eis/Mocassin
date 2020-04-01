using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Generic super cell wrapper that combines a (a,b,c,p) encoded set of positions with a basic structure information to
    ///     describe a supercell lattice
    /// </summary>
    public class SuperCellAdapter<T1> : IUnitCellProvider<T1>
    {
        /// <summary>
        ///     The supercell size information
        /// </summary>
        protected Coordinates4I Sizes;

        /// <summary>
        ///     The 4D (a,b,c,p) cell entry array that is a three dimensional array of cell entry arrays
        /// </summary>
        protected T1[,,][] CellEntries { get; set; }

        /// <inheritdoc />
        public ref Coordinates4I CellSize => ref Sizes;

        /// <inheritdoc />
        public IUnitCellVectorEncoder VectorEncoder { get; protected set; }

        /// <summary>
        ///     Create new supercell wrapper from cell entry array and unit cell vector encoder
        /// </summary>
        /// <param name="cellEntries"></param>
        /// <param name="vectorEncoder"></param>
        public SuperCellAdapter(T1[,,][] cellEntries, IUnitCellVectorEncoder vectorEncoder)
        {
            CellEntries = cellEntries;
            VectorEncoder = vectorEncoder;
            Sizes = GetSupercellSizeInfo(cellEntries);
        }

        /// <inheritdoc />
        public LatticePoint<T1> GetCellEntry(int a, int b, int c, int p)
        {
            return GetCellEntry(new VectorI3(a, b, c), p);
        }

        /// <inheritdoc />
        public LatticePoint<T1> GetCellEntry(in VectorI3 offset, int p)
        {
            var trimmedOffset = TrimByPeriodicBoundary(offset);
            var entry = CellEntries[trimmedOffset.A, trimmedOffset.B, trimmedOffset.C][p];
            return new LatticePoint<T1>(new Fractional3D(offset.A, offset.B, offset.C) + VectorEncoder.PositionList[p], entry);
        }

        /// <inheritdoc />
        public IUnitCell<T1> GetUnitCell(int a, int b, int c)
        {
            return GetUnitCell(new VectorI3(a, b, c));
        }

        /// <inheritdoc />
        public IUnitCell<T1> GetUnitCell(in VectorI3 offset)
        {
            var trimmedOffset = TrimByPeriodicBoundary(offset);
            return new UnitCellAdapter<T1>(CellEntries[trimmedOffset.A, trimmedOffset.B, trimmedOffset.C], VectorEncoder, offset);
        }

        /// <inheritdoc />
        public LatticePoint<T1> GetCellEntry(in CrystalVector4D vector)
        {
            return GetCellEntry(vector.A, vector.B, vector.C, vector.P);
        }

        /// <inheritdoc />
        public T1 GetEntryValueAt(in Fractional3D vector)
        {
            return !VectorEncoder.TryEncode(vector, out var encoded)
                ? default
                : CellEntries[encoded.A, encoded.B, encoded.C][encoded.P];
        }

        /// <summary>
        ///     Corrects set of offset coordinates into the supercell by applying the periodic boundary conditions
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public VectorI3 TrimByPeriodicBoundary(in VectorI3 offset)
        {
            return TrimByPeriodicBoundary(offset.A, offset.B, offset.C);
        }

        /// <summary>
        ///     Corrects set of offset coordinates into the supercell by applying the periodic boundary conditions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public VectorI3 TrimByPeriodicBoundary(int a, int b, int c)
        {
            // In the majority of cases while loops are much faster than modulo methods because actual required trimming is rare
            while (a < 0) a += CellSize.A;
            while (a >= CellSize.A) a -= CellSize.A;
            while (b < 0) b += CellSize.B;
            while (b >= CellSize.B) b -= CellSize.B;
            while (c < 0) c += CellSize.C;
            while (c >= CellSize.C) c -= CellSize.C;
            return new VectorI3(a, b, c);
        }

        /// <summary>
        ///     Creates the supercell size info for a 4D entry set
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public Coordinates4I GetSupercellSizeInfo(T1[,,][] entries)
        {
            return new Coordinates4I(entries.GetLength(0), entries.GetLength(1), entries.GetLength(2),
                entries[0, 0, 0].GetLength(0));
        }
    }
}