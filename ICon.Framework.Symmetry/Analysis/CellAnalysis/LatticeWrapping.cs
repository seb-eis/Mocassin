using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Factory for wrapped unit and super-cells that can be used with the unit cell entry locator
    /// </summary>
    public static class LatticeWrapping
    {
        /// <summary>
        ///     Creates new unit cell wrapper for the provided entries and vector encoder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entries"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static UnitCellAdapter<T> ToUnitCell<T>(IList<T> entries, IUnitCellVectorEncoder vectorEncoder) =>
            new UnitCellAdapter<T>(entries, vectorEncoder);

        /// <summary>
        ///     Creates new supercell for the provided entry lattice and vector encoder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entryLattice"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static SuperCellAdapter<T> ToSuperCell<T>(T[,,][] entryLattice, IUnitCellVectorEncoder vectorEncoder) =>
            new SuperCellAdapter<T>(entryLattice, vectorEncoder);

        /// <summary>
        ///     Extends the provided unit cell into a supercell of specified size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unitCell"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static SuperCellAdapter<T> ToSuperCell<T>(IUnitCell<T> unitCell, in VectorI3 size)
        {
            return ToSuperCell(unitCell.GetAllEntries().Select(value => value.Content), size, unitCell.VectorEncoder);
        }

        /// <summary>
        ///     Takes a set of unit cell entries and vector encoder and creates a supercell with identical unit cells
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cellEntry"></param>
        /// <param name="size"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static SuperCellAdapter<T> ToSuperCell<T>(IEnumerable<T> cellEntry, in VectorI3 size,
            IUnitCellVectorEncoder vectorEncoder)
        {
            if (size.A < 1 || size.B < 1 || size.C < 1)
                throw new ArgumentException("Invalid size information", nameof(size));

            return ToSuperCell(new T[size.A, size.B, size.C][].Populate(cellEntry.ToArray), vectorEncoder);
        }

        /// <summary>
        ///     Creates a supercell wrapper from a set of unit cell entry sets that each describe the unit cell at the linearized
        ///     index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cellEntries"></param>
        /// <param name="size"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static SuperCellAdapter<T> ToSuperCell<T>(IEnumerable<T[]> cellEntries, in VectorI3 size,
            IUnitCellVectorEncoder vectorEncoder)
        {
            if (size.A < 1 || size.B < 1 || size.C < 1)
                throw new ArgumentException("Invalid size information", nameof(size));

            return ToSuperCell(new T[size.A, size.B, size.C][].Populate(cellEntries), vectorEncoder);
        }
    }
}