using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.Coordinates;
using ICon.Mathematics.ValueTypes;

namespace ICon.Symmetry.Analysis
{
    /// <summary>
    /// Generic super cell wrapper that combines a (a,b,c,p) encoded set of positions with a basic structure information to describe a supercell lattice
    /// </summary>
    public class SupercellWrapper<T1> : IUnitCellProvider<T1>
    {
        /// <summary>
        /// The supercell size information
        /// </summary>
        protected Coordinates<int, int, int, int> SizeInfo;

        /// <summary>
        /// The 4D (a,b,c,p) cell entry array that is a three dimensional array of cell entry arrays
        /// </summary>
        protected T1[,,][] CellEntries { get; set; }

        /// <summary>
        /// Get the supercell size information by refernce
        /// </summary>
        public ref Coordinates<int, int, int, int> CellSizeInfo => ref SizeInfo;

        /// <summary>
        /// Get the vector encoder that supplies the basic unit cell information geoemtry information and encoding functionalities
        /// </summary>
        public UnitCellVectorEncoder VectorEncoder { get; protected set; }

        /// <summary>
        /// Create new supercell wrapper from cell entry array and unit cell vector encoder
        /// </summary>
        /// <param name="cellEntries"></param>
        /// <param name="vectorEncoder"></param>
        public SupercellWrapper(T1[,,][] cellEntries, UnitCellVectorEncoder vectorEncoder)
        {
            CellEntries = cellEntries;
            VectorEncoder = vectorEncoder;
            SizeInfo = GetSupercellSizeInfo(cellEntries);
        }

        /// <summary>
        /// Get a cell entry by coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public CellEntry<T1> GetCellEntry(int a, int b, int c, int p)
        {
            return GetCellEntry(new Coordinates<int, int, int>(a, b, c), p);
        }

        /// <summary>
        /// Get a cell entry by coordinates
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public CellEntry<T1> GetCellEntry(in Coordinates<int, int, int> offset, int p)
        {
            var trimmedOffset = TrimByPeriodicBoundary(offset);
            T1 entry = CellEntries[trimmedOffset.A, trimmedOffset.B, trimmedOffset.C][p];
            return new CellEntry<T1>(new Fractional3D(offset.A, offset.B, offset.C) + VectorEncoder.PositionList[p], entry);
        }

        /// <summary>
        /// Get a unit cell by (a,b,c) offset
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public IUnitCell<T1> GetUnitCell(int a, int b, int c)
        {
            return GetUnitCell(new Coordinates<int, int, int>(a,b,c));
        }

        /// <summary>
        /// Get a unit cell by (a,b,c) offset
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public IUnitCell<T1> GetUnitCell(in Coordinates<int, int, int> offset)
        {
            var trimmedOffset = TrimByPeriodicBoundary(offset);
            return new UnitCellWrapper<T1>(CellEntries[trimmedOffset.A, trimmedOffset.B, trimmedOffset.C], VectorEncoder, offset);
        }

        /// <summary>
        /// Corrects set of offset coordinates into the supercell by applying the periodic boundary conditions
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Coordinates<int, int, int> TrimByPeriodicBoundary(in Coordinates<int, int, int> offset)
        {
            return TrimByPeriodicBoundary(offset.A, offset.B, offset.C);
        }

        /// <summary>
        /// Corrects set of offset coordinates into the supercell by applying the periodic boundary conditions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public Coordinates<int, int, int> TrimByPeriodicBoundary(int a, int b, int c)
        {
            // In the majority of cases while loops are much faster than modulo methods because actual required trimming is rare
            while (a < 0)
            {
                a += CellSizeInfo.A;
            }
            while (a >= CellSizeInfo.A)
            {
                a -= CellSizeInfo.A;
            }
            while (b < 0)
            {
                b += CellSizeInfo.B;
            }
            while (b >= CellSizeInfo.B)
            {
                b -= CellSizeInfo.B;
            }
            while (c < 0)
            {
                c += CellSizeInfo.C;
            }
            while (c >= CellSizeInfo.C)
            {
                c -= CellSizeInfo.C;
            }
            return new Coordinates<int, int, int>(a, b, c);
        }

        /// <summary>
        /// Creates the supercell size info for a 4D entry set
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public Coordinates<int, int, int, int> GetSupercellSizeInfo(T1[,,][] entries)
        {
            return new Coordinates<int, int, int, int>(entries.GetLength(0), entries.GetLength(1), entries.GetLength(2), entries[0, 0, 0].GetLength(0));
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
            return CellEntries[encoded.A, encoded.B, encoded.C][encoded.P];
        }
    }
}
