using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Basic generic (1,1,1) unit cell wrapper that wraps a set of additional position information and a vector encoder
    ///     into unit cell provider
    /// </summary>
    public class UnitCellAdapter<T1> : IUnitCell<T1>, IUnitCellProvider<T1>
    {
        /// <summary>
        ///     The unit cell size info (1,1,1,#Entries)
        /// </summary>
        protected Coordinates4I Sizes;

        /// <summary>
        ///     The list interface of unit cell entries without the vector information
        /// </summary>
        protected IList<T1> CellEntries { get; set; }

        /// <summary>
        ///     The current offset of the cell
        /// </summary>
        protected VectorI3 Offset { get; set; }

        /// <inheritdoc cref="IUnitCellProvider{T1}.VectorEncoder" />
        public IUnitCellVectorEncoder VectorEncoder { get; protected set; }

        /// <inheritdoc />
        public int EntryCount => CellEntries.Count;

        /// <inheritdoc />
        public ref Coordinates4I CellSize => ref Sizes;

        /// <inheritdoc />
        public LatticePoint<T1> this[int index] => GetCellEntry(Offset, index);

        /// <summary>
        ///     Creates new unit cell wrapper for entry list and vector encoder
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="vectorEncoder"></param>
        public UnitCellAdapter(IList<T1> entries, IUnitCellVectorEncoder vectorEncoder)
        {
            VectorEncoder = vectorEncoder ?? throw new ArgumentNullException(nameof(vectorEncoder));
            if (entries.Count != vectorEncoder.PositionCount)
                throw new ArgumentException("Entry count does not match vector encoder position count", nameof(entries));

            Sizes = new Coordinates4I(1, 1, 1, entries.Count);
            CellEntries = entries;
        }

        /// <summary>
        ///     Creates new unit cell wrapper for entry list and vector encoder with an additional initial offset
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="vectorEncoder"></param>
        /// <param name="offset"></param>
        public UnitCellAdapter(IList<T1> entries, IUnitCellVectorEncoder vectorEncoder, in VectorI3 offset)
            : this(entries, vectorEncoder)
        {
            Offset = offset;
        }

        /// <summary>
        ///     Protected generation of an empty cell wrapper for shifting this unit cell
        /// </summary>
        protected UnitCellAdapter()
        {
        }

        /// <inheritdoc />
        public IEnumerable<LatticePoint<T1>> GetAllEntries()
        {
            var index = -1;
            return CellEntries.Select(entry => GetCellEntry(Offset, ++index));
        }

        /// <inheritdoc />
        public LatticePoint<T1> GetCellEntry(int a, int b, int c, int p) =>
            new LatticePoint<T1>(new Fractional3D(a, b, c) + VectorEncoder.PositionList[p], CellEntries[p]);

        /// <inheritdoc />
        public LatticePoint<T1> GetCellEntry(in VectorI3 offset, int p) =>
            new LatticePoint<T1>(new Fractional3D(offset.A, offset.B, offset.C) + VectorEncoder.PositionList[p], CellEntries[p]);

        /// <inheritdoc />
        public IUnitCell<T1> GetUnitCell(int a, int b, int c)
        {
            var result = new UnitCellAdapter<T1>
            {
                CellEntries = CellEntries,
                Offset = new VectorI3(a, b, c),
                Sizes = Sizes,
                VectorEncoder = VectorEncoder
            };
            return result;
        }

        /// <inheritdoc />
        public IUnitCell<T1> GetUnitCell(in VectorI3 offset)
        {
            var result = new UnitCellAdapter<T1>
            {
                CellEntries = CellEntries,
                Offset = offset,
                Sizes = Sizes,
                VectorEncoder = VectorEncoder
            };
            return result;
        }

        /// <inheritdoc />
        public LatticePoint<T1> GetCellEntry(in Vector4I vector) => GetCellEntry(vector.A, vector.B, vector.C, vector.P);

        /// <inheritdoc />
        public T1 GetEntryValueAt(in Fractional3D vector) =>
            !VectorEncoder.TryEncode(vector, out var encoded)
                ? default
                : CellEntries[encoded.P];
    }
}