using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.Coordinates;
using ICon.Mathematics.ValueTypes;

namespace ICon.Symmetry.Analysis
{
    /// <summary>
    /// Factory for wrapped unit and supercellss that can be used with the unit cell entry locator
    /// </summary>
    public static class CellWrapperFactory
    {
        /// <summary>
        /// Creates new unit cell wrapper for the provided entries and vector encoder
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="entries"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static UnitCellWrapper<T1> CreateUnitCell<T1>(IList<T1> entries, UnitCellVectorEncoder vectorEncoder)
        {
            return new UnitCellWrapper<T1>(entries, vectorEncoder);
        }

        /// <summary>
        /// Creates new supercell for the provided entry lattice and vector encoder
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="entryLattice"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static SupercellWrapper<T1> CreateSupercell<T1>(T1[,,][] entryLattice, UnitCellVectorEncoder vectorEncoder)
        {
            return new SupercellWrapper<T1>(entryLattice, vectorEncoder);
        }

        /// <summary>
        /// Extends the provided unit cell into a supercell of specififed size
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="unitCell"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static SupercellWrapper<T1> CreateSupercell<T1>(IUnitCell<T1> unitCell, in Coordinates<int, int, int> size)
        {
            return CreateSupercell(unitCell.GetAllEntries().Select(value => value.Entry).ToList(), size, unitCell.VectorEncoder);
        }

        /// <summary>
        /// Takes a set of unit cell entries and evctor encoder
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="entries"></param>
        /// <param name="size"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static SupercellWrapper<T1> CreateSupercell<T1>(IList<T1> entries, in Coordinates<int,int,int> size, UnitCellVectorEncoder vectorEncoder)
        {
            if (size.A < 1 || size.B < 1 || size.C < 1)
            {
                throw new ArgumentException("One of the size values is negative", nameof(size));
            }
            var entryLattice = new T1[size.A, size.B, size.C][];
            for (int i = 0; i < size.A; i++)
            {
                for (int j = 0; j < size.B; j++)
                {
                    for (int k = 0; k < size.C; k++)
                    {
                        entryLattice[i, j, k] = new T1[entries.Count];
                        entries.CopyTo(entryLattice[i, j, k], 0);
                    }
                }
            }
            return CreateSupercell(entryLattice, vectorEncoder);
        }
    }
}
