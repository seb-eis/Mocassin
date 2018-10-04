using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Extensions;
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
        public static UnitCellWrapper<T1> CreateUnitCell<T1>(IList<T1> entries, IUnitCellVectorEncoder vectorEncoder)
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
        public static SupercellWrapper<T1> CreateSupercell<T1>(T1[,,][] entryLattice, IUnitCellVectorEncoder vectorEncoder)
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
            return CreateSupercell(unitCell.GetAllEntries().Select(value => value.Entry), size, unitCell.VectorEncoder);
        }

        /// <summary>
        /// Takes a set of unit cell entries and vector encoder and creates a supercell with identical unit cells
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="cellEntry"></param>
        /// <param name="size"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static SupercellWrapper<T1> CreateSupercell<T1>(IEnumerable<T1> cellEntry, in Coordinates<int,int,int> size, IUnitCellVectorEncoder vectorEncoder)
        {
            if (size.A < 1 || size.B < 1 || size.C < 1)
            {
                throw new ArgumentException("Invalid size information", nameof(size));
            }
            return CreateSupercell(new T1[size.A, size.B, size.C][].Populate(cellEntry.ToArray), vectorEncoder);
        }

        /// <summary>
        /// Creates a supercell wrapper from a set of unit cell entry sets that each describe the unit cell at the linearized index
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="cellEntries"></param>
        /// <param name="size"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static SupercellWrapper<T1> CreateSupercell<T1>(IEnumerable<T1[]> cellEntries, in Coordinates<int,int,int> size, UnitCellVectorEncoder vectorEncoder)
        {
            if (size.A < 1 || size.B < 1 || size.C < 1)
            {
                throw new ArgumentException("Invalid size information", nameof(size));
            }
            return CreateSupercell(new T1[size.A, size.B, size.C][].Populate(cellEntries), vectorEncoder);
        }
    }
}
