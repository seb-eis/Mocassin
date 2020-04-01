using System;
using System.Collections.Generic;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Symmetry.Analysis;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Represents a data port for extended structure manager data that is calculated and updated on a 'on-demand' basis
    /// </summary>
    public interface IStructureCachePort : IModelCachePort
    {
        /// <summary>
        ///     Get the vector encoder that handles transformations between 3D coordinates and unit cell encoded 4D vectors
        /// </summary>
        /// <returns></returns>
        IUnitCellVectorEncoder GetVectorEncoder();

        /// <summary>
        ///     Extends the unit cell position at specified index into the full wyckoff one sorted list (Returns empty list if
        ///     cell position is deprecated)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        SetList<FractionalPosition> GetExtendedPositionList(int index);

        /// <summary>
        ///     Extends all unit cell positions into sorted wyckoff one position lists (Each position is extended into a separated
        ///     set of positions)
        /// </summary>
        /// <returns></returns>
        IList<SetList<FractionalPosition>> GetExtendedPositionLists();

        /// <summary>
        ///     Extends all unit cell positions into a sorted wyckoff one position list (All positions are extended and sorted into
        ///     the same list)
        /// </summary>
        /// <returns></returns>
        SetList<FractionalPosition> GetLinearizedExtendedPositionList();

        /// <summary>
        ///     Extends the unit cell position with the specified index into the 4D encoded wyckoff set (Returns empty list if
        ///     position is deprecated)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        SetList<CrystalVector4D> GetEncodedExtendedPositionList(int index);

        /// <summary>
        ///     Extends all unit cell positions into their 4D encoded counterparts (Each position is extended into a separate set
        ///     of positions)
        /// </summary>
        /// <returns></returns>
        IList<SetList<CrystalVector4D>> GetEncodedExtendedPositionLists();

        /// <summary>
        ///     Get the unit cell provider for the current unit cell
        /// </summary>
        /// <returns></returns>
        IUnitCellProvider<FractionalPosition> GetUnitCellProvider();

        /// <summary>
        ///     Get a unit cell provider that only carries the occupations of the positions as entry information
        /// </summary>
        /// <returns></returns>
        IUnitCellProvider<int> GetOccupationUnitCellProvider();

        /// <summary>
        ///     Get a unit cell provider that carries the unit cell position interfaces for advanced analysis queries
        /// </summary>
        /// <returns></returns>
        IUnitCellProvider<ICellSite> GetFullUnitCellProvider();

        /// <summary>
        ///     Gets all positions of the unit cell that are symmetry equivalent to the position at the provided 4D vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        SetList<FractionalPosition> GetExtendedPositionList(in CrystalVector4D vector);

        /// <summary>
        ///     Get read only list that assigns each extended position index the correct unit cell position interface
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ICellSite> GetExtendedIndexToPositionList();

        /// <summary>
        ///     Get a sorted unique list of all the wyckoff extension of the position dummy at the specified index
        /// </summary>
        /// <returns></returns>
        SetList<Fractional3D> GetExtendedDummyPositionList(int index);

        /// <summary>
        ///     Get a list interface of all extended wyckoff set lists for all dummy positions
        /// </summary>
        /// <returns></returns>
        IList<SetList<Fractional3D>> GetExtendedDummyPositionLists();

        /// <summary>
        ///     Get a 2D list set that assigns each wyckoff position its set of position indices in the unit cell
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<SetList<int>> GetWyckoffIndexingLists();

        /// <summary>
        ///     Get the number of existing positions if all unit cell positions are extended by the currently set space group
        /// </summary>
        /// <returns></returns>
        int GetLinearizedExtendedPositionCount();

        /// <summary>
        ///     Finds the array of <see cref="LatticeTarget"/> data around a <see cref="Fractional3D"/> position till a cutoff range in [Ang]
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="maxDistance"></param>
        /// <param name="targetAcceptor"></param>
        /// <returns></returns>
        LatticeTarget[] FindLatticeTargets(in Fractional3D origin, double maxDistance, Func<ICellSite, bool> targetAcceptor);

        /// <summary>
        ///     Finds all <see cref="LatticeTarget"/> data around all positions in the unit cell till cutoff distance in [Ang]
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <param name="originAcceptor"></param>
        /// <param name="targetAcceptor"></param>
        /// <returns></returns>
        IDictionary<int, LatticeTarget[]> FindUnitCellLatticeTargets(double maxDistance, Func<FractionalPosition, bool> originAcceptor, Func<ICellSite, bool> targetAcceptor);
    }
}