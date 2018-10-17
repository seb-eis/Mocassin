using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Generic super cell wrapper that combines a (a,b,c,p) encoded set of positions with a basic structure information to
    ///     describe a supercell lattice
    /// </summary>
    public class SupercellAdapter<T1> : IUnitCellProvider<T1>
    {
        /// <summary>
        ///     The supercell size information
        /// </summary>
        protected Coordinates<int, int, int, int> SizeInfo;

        /// <summary>
        ///     The 4D (a,b,c,p) cell entry array that is a three dimensional array of cell entry arrays
        /// </summary>
        protected T1[,,][] CellEntries { get; set; }

        /// <inheritdoc />
        public ref Coordinates<int, int, int, int> CellSizeInfo => ref SizeInfo;

        /// <inheritdoc />
        public IUnitCellVectorEncoder VectorEncoder { get; protected set; }

        /// <summary>
        ///     Create new supercell wrapper from cell entry array and unit cell vector encoder
        /// </summary>
        /// <param name="cellEntries"></param>
        /// <param name="vectorEncoder"></param>
        public SupercellAdapter(T1[,,][] cellEntries, IUnitCellVectorEncoder vectorEncoder)
        {
            CellEntries = cellEntries;
            VectorEncoder = vectorEncoder;
            SizeInfo = GetSupercellSizeInfo(cellEntries);
        }

        /// <inheritdoc />
        public CellEntry<T1> GetCellEntry(int a, int b, int c, int p)
        {
            return GetCellEntry(new Coordinates<int, int, int>(a, b, c), p);
        }

        /// <inheritdoc />
        public CellEntry<T1> GetCellEntry(in Coordinates<int, int, int> offset, int p)
        {
            var trimmedOffset = TrimByPeriodicBoundary(offset);
            var entry = CellEntries[trimmedOffset.A, trimmedOffset.B, trimmedOffset.C][p];
            return new CellEntry<T1>(new Fractional3D(offset.A, offset.B, offset.C) + VectorEncoder.PositionList[p], entry);
        }

        /// <inheritdoc />
        public IUnitCell<T1> GetUnitCell(int a, int b, int c)
        {
            return GetUnitCell(new Coordinates<int, int, int>(a, b, c));
        }

        /// <inheritdoc />
        public IUnitCell<T1> GetUnitCell(in Coordinates<int, int, int> offset)
        {
            var trimmedOffset = TrimByPeriodicBoundary(offset);
            return new UnitCellAdapter<T1>(CellEntries[trimmedOffset.A, trimmedOffset.B, trimmedOffset.C], VectorEncoder, offset);
        }

        /// <summary>
        ///     Corrects set of offset coordinates into the supercell by applying the periodic boundary conditions
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Coordinates<int, int, int> TrimByPeriodicBoundary(in Coordinates<int, int, int> offset)
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
        public Coordinates<int, int, int> TrimByPeriodicBoundary(int a, int b, int c)
        {
            // In the majority of cases while loops are much faster than modulo methods because actual required trimming is rare
            while (a < 0) a += CellSizeInfo.A;
            while (a >= CellSizeInfo.A) a -= CellSizeInfo.A;
            while (b < 0) b += CellSizeInfo.B;
            while (b >= CellSizeInfo.B) b -= CellSizeInfo.B;
            while (c < 0) c += CellSizeInfo.C;
            while (c >= CellSizeInfo.C) c -= CellSizeInfo.C;
            return new Coordinates<int, int, int>(a, b, c);
        }

        /// <summary>
        ///     Creates the supercell size info for a 4D entry set
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public Coordinates<int, int, int, int> GetSupercellSizeInfo(T1[,,][] entries)
        {
            return new Coordinates<int, int, int, int>(entries.GetLength(0), entries.GetLength(1), entries.GetLength(2),
                entries[0, 0, 0].GetLength(0));
        }

        /// <inheritdoc />
        public CellEntry<T1> GetCellEntry(in CrystalVector4D vector)
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
    }
}