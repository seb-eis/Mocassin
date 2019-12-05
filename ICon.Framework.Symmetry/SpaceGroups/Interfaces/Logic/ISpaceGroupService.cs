using System;
using System.Collections.Generic;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Symmetry.CrystalSystems;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Common interface for all space groups service implementations
    /// </summary>
    public interface ISpaceGroupService
    {
        /// <summary>
        ///     Get a boolean flag if the service has a database connection to load space groups
        /// </summary>
        bool HasDbConnection { get; }

        /// <summary>
        ///     Get the currently loaded space group interface
        /// </summary>
        ISpaceGroup LoadedGroup { get; }

        /// <summary>
        ///     The vector comparer used to compare fractional vectors
        /// </summary>
        IComparer<Fractional3D> Comparer { get; }

        /// <summary>
        ///     Creates the crystal system to the currently loaded group from a crystal system provider
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        CrystalSystem CreateCrystalSystem(ICrystalSystemSource source);

        /// <summary>
        ///     Creates a wyckoff operation dictionary for the provided source vector that informs about which operations can be
        ///     used to create which position
        /// </summary>
        /// <param name="sourceVector"></param>
        /// <returns></returns>
        IWyckoffOperationDictionary GetOperationDictionary(in Fractional3D sourceVector);

        /// <summary>
        ///     Get a list interface of all symmetry operations that do not change the input vector (Optional with shift
        ///     correction)
        /// </summary>
        /// <param name="sourceVector"></param>
        /// <param name="shiftCorrection"></param>
        /// <returns></returns>
        IList<ISymmetryOperation> GetMultiplicityOperations(in Fractional3D sourceVector, bool shiftCorrection);

        /// <summary>
        ///     Get the point operation group for the provided origin point and point sequence based upon the currently loaded
        ///     space group
        /// </summary>
        /// <param name="originPoint"></param>
        /// <param name="pointSequence"></param>
        /// <returns></returns>
        IPointOperationGroup GetPointOperationGroup(in Fractional3D originPoint, IEnumerable<Fractional3D> pointSequence);

        /// <summary>
        ///     Gets the unfiltered and untrimmed list of all wyckoff extended sequences symmetry equivalent to the input sequence
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        IList<Fractional3D[]> GetFullP1PathExtension(IEnumerable<Fractional3D> refSequence);

        /// <summary>
        ///     Gets the ordered, filtered and trimmed list of all wyckoff extended sequences that begin in the (0,0,0) origin unit
        ///     cell
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        SetList<Fractional3D[]> GetUnitCellP1PathExtension(IEnumerable<Fractional3D> refSequence);

        /// <summary>
        ///     Gets a minimal set of <see cref="ISymmetryOperation" /> instances required to fully describe the passed vector set
        ///     in a P1 extended unit cell context (Warning: Removing the inverses is only valid for display purposes)
        /// </summary>
        /// <param name="refSequence"></param>
        /// <param name="filterInverses"></param>
        /// <returns></returns>
        IList<ISymmetryOperation> GetMinimalUnitCellP1PathExtensionOperations(IEnumerable<Fractional3D> refSequence,
            bool filterInverses = false);

        /// <summary>
        ///     Gets a sorted list of unique fractional vectors that represent all equivalent wyckoff positions to the original
        ///     (Including original)
        /// </summary>
        /// <param name="refVector"></param>
        /// <returns></returns>
        SetList<Fractional3D> GetUnitCellP1PositionExtension(in Fractional3D refVector);

        /// <summary>
        ///     Get multiple sorted unique lists of wyckoff extended positions that each includes the original reference vector
        /// </summary>
        /// <param name="refVectors"></param>
        /// <returns></returns>
        List<SetList<Fractional3D>> GetUnitCellP1PositionExtensions(IEnumerable<Fractional3D> refVectors);

        /// <summary>
        ///     Gets a sorted list of unique structs that implement the fractional vector interface that represent all equivalent
        ///     wyckoff positions to the original (Including original)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="refVector"></param>
        /// <returns></returns>
        SetList<Fractional3D> GetUnitCellP1PositionExtension<TSource>(TSource refVector) where TSource : IFractional3D;

        /// <summary>
        ///     Get multiple sorted unique sets of wyckoff positions with the same type as the provided fractional vector type
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="refVectors"></param>
        /// <returns></returns>
        List<SetList<Fractional3D>> GetUnitCellP1PositionExtensions<TSource>(IEnumerable<TSource> refVectors) where TSource : IFractional3D;

        /// <summary>
        ///     Translates a reference sequence of positions onto each wyckoff 1 position
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        SortedDictionary<Fractional3D, List<Fractional3D>> GetEnvironmentDictionary(IEnumerable<Fractional3D> refSequence);

        /// <summary>
        ///     Gets a sorted list of all unique space group interfaces the service can provide
        /// </summary>
        /// <returns></returns>
        SetList<ISpaceGroup> GetFullGroupList();

        /// <summary>
        ///     Tries to load a space group into the service using the provided search function (Returns false if no match is
        ///     found)
        /// </summary>
        /// <param name="searchPredicate"></param>
        /// <returns></returns>
        bool TryLoadGroup(Predicate<ISpaceGroup> searchPredicate);

        /// <summary>
        ///     Tries to load a space group into the service that matches the provided space group identifier (Returns false if no
        ///     match is found)
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool TryLoadGroup(SpaceGroupEntry entry);

        /// <summary>
        ///     Loads the passed <see cref="ISpaceGroup" /> into the service
        /// </summary>
        /// <param name="spaceGroup"></param>
        void LoadGroup(ISpaceGroup spaceGroup);

        /// <summary>
        ///     Creates a vector comparer for a special type of 3D vector interface
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        IComparer<T1> GetSpecialVectorComparer<T1>() where T1 : IVector3D;

        /// <summary>
        ///     Shifts a sequence of fractional vectors in a manner that the first vector in the sequence is in the (0,0,0) origin
        ///     cell
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> ShiftFirstToOriginCell(IEnumerable<Fractional3D> source, double tolerance);

        /// <summary>
        ///     Checks if the passed <see cref="ISymmetryOperation" /> pushes the provided vector outside of the origin cell and
        ///     returns either the operation itself or a origin shift corrected version
        /// </summary>
        /// <param name="start"></param>
        /// <param name="operation"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        ISymmetryOperation GetOriginCellShiftedOperations(in Fractional3D start, ISymmetryOperation operation, double tolerance);

        /// <summary>
        ///     Creates the first possible operation that transforms the passed source vector onto the target.
        ///     Returns null if none is found
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        ISymmetryOperation GetOperationToTarget(in Fractional3D source, in Fractional3D target);

        /// <summary>
        ///     Gets all symmetry equivalent positions to the source that are within the limits of the edges of a cuboid
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IList<Fractional3D> GetPositionsInCuboid(in Fractional3D source, in Fractional3D start, in Fractional3D end);

        /// <summary>
        ///     Checks a pair interaction geometry for chirality within the current space group. Caller has to ensure that both
        ///     coordinates belong to the same sub-lattice
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        bool CheckInteractionGeometryIsChiral(in Fractional3D left, in Fractional3D right);

        /// <summary>
        ///     Checks two pair interaction geometries for being a chiral pair within the current space group. Caller has to ensure
        ///     that all coordinates belong to the same sub-lattice
        /// </summary>
        /// <param name="left0"></param>
        /// <param name="right0"></param>
        /// <param name="left1"></param>
        /// <param name="right1"></param>
        /// <returns></returns>
        bool CheckInteractionGeometryIsChiralPair(in Fractional3D left0, in Fractional3D right0, in Fractional3D left1, in Fractional3D right1);
    }
}