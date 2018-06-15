using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.Coordinates;
using ICon.Mathematics.ValueTypes;
using ICon.Framework.Collections;

namespace ICon.Symmetry.Analysis
{
    /// <summary>
    /// Basic generic (1,1,1) unit cell wrapper that wraps a set of additional position informations and a vector encoder into unit cell provider
    /// </summary>
    public class UnitCellWrapper<T1> : IUnitCell<T1>, IUnitCellProvider<T1>
    {
        /// <summary>
        /// The unit cell size info (1,1,1,#Entries)
        /// </summary>
        protected Coordinates<int, int, int, int> SizeInfo;

        /// <summary>
        /// The list interface of unit cell entries without the vector information
        /// </summary>
        protected IList<T1> CellEntries { get; set; }

        /// <summary>
        /// The current offset of the cell
        /// </summary>
        protected Coordinates<int, int, int> Offset { get; set; }

        /// <summary>
        /// Get the vector encoded that contains the basic transformation and cell information
        /// </summary>
        public UnitCellVectorEncoder VectorEncoder { get; protected set; }

        /// <summary>
        /// Get the entry count of the cell
        /// </summary>
        public int EntryCount => CellEntries.Count;

        /// <summary>
        /// Get the cell size information (1,1,1,#Entries) by reference
        /// </summary>
        public ref Coordinates<int, int, int, int> CellSizeInfo => ref SizeInfo;

        /// <summary>
        /// Get the cell entry at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CellEntry<T1> this[int index] => GetCellEntry(Offset, index);

        /// <summary>
        /// Creates new unit cell wrapper for entry list and vector encoder
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="vectorEncoder"></param>
        public UnitCellWrapper(IList<T1> entries, UnitCellVectorEncoder vectorEncoder)
        {
            VectorEncoder = vectorEncoder ?? throw new ArgumentNullException(nameof(vectorEncoder));
            if (entries.Count != vectorEncoder.PositionCount)
            {
                throw new ArgumentException("Entry count does not match vector encoder position count", nameof(entries));
            }
            SizeInfo = new Coordinates<int, int, int, int>(1, 1, 1, entries.Count);
            CellEntries = entries;
        }

        /// <summary>
        /// Creates new unit cell wrapper for entry list and vector encoder with an additional initial offset
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="vectorEncoder"></param>
        /// <param name="offset"></param>
        public UnitCellWrapper(IList<T1> entries, UnitCellVectorEncoder vectorEncoder, in Coordinates<int, int, int> offset) : this(entries, vectorEncoder)
        {
            Offset = offset;
        }

        /// <summary>
        /// Protected generation of an empty cell wrapper for shifting this unit cell
        /// </summary>
        protected UnitCellWrapper()
        {
        }

        /// <summary>
        /// Get all entries shifted by the current offset
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CellEntry<T1>> GetAllEntries()
        {
            int index = -1;
            return CellEntries.Select((entry) => GetCellEntry(Offset, ++index));
        }

        /// <summary>
        /// Get the unit cell position entry specififed by the coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public CellEntry<T1> GetCellEntry(int a, int b, int c, int p)
        {
            return new CellEntry<T1>(new Fractional3D(a, b, c) + VectorEncoder.PositionList[p], CellEntries[p]);
        }

        /// <summary>
        /// Get the unit cell position entry specififed by the coordinates
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public CellEntry<T1> GetCellEntry(in Coordinates<int, int, int> offset, int p)
        {
            return new CellEntry<T1>(new Fractional3D(offset.A, offset.B, offset.C) + VectorEncoder.PositionList[p], CellEntries[p]);
        }

        /// <summary>
        /// Returns a new cell that is offset by the passed value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public IUnitCell<T1> GetUnitCell(int a, int b, int c)
        {
            var result = new UnitCellWrapper<T1>
            {
                CellEntries = CellEntries,
                Offset = new Coordinates<int, int, int>(a,b,c),
                SizeInfo = SizeInfo,
                VectorEncoder = VectorEncoder
            };
            return result;
        }

        /// <summary>
        /// Returns a new cell that is offset by the passed value
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public IUnitCell<T1> GetUnitCell(in Coordinates<int, int, int> offset)
        {
            var result = new UnitCellWrapper<T1>
            {
                CellEntries = CellEntries,
                Offset = offset,
                SizeInfo = SizeInfo,
                VectorEncoder = VectorEncoder
            };
            return result;
        }

        /// <summary>
        /// Get the cell entry at the specififed 4D crystal vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public CellEntry<T1> GetCellEntry(in CrystalVector4D vector)
        {
            return GetCellEntry(vector.A, vector.B, vector.C, vector.P);
        }

        /// <summary>
        /// Get the entry value at the specfified absolute fractional vector. Returns the default value if nothing is there
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T1 GetEntryValueAt(in Fractional3D vector)
        {
            if (!VectorEncoder.TryEncodeFractional(vector, out var encoded))
            {
                return default;
            }
            return CellEntries[encoded.P];
        }
    }
}
