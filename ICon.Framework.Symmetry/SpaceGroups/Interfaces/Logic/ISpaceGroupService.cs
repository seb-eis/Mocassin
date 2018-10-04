using System;
using System.Collections.Generic;
using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;
using ICon.Symmetry.CrystalSystems;
using ICon.Mathematics.Comparers;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Common interface for all space groups service implementations
    /// </summary>
    public interface ISpaceGroupService
    {
        /// <summary>
        /// Get the currently loaded space group interface
        /// </summary>
        ISpaceGroup LoadedGroup { get; }

        /// <summary>
        /// The vector comparer used to compare fractional vectors
        /// </summary>
        IComparer<Fractional3D> Comparer { get; }

        /// <summary>
        /// Creates the crystal system to the currently loaded group from a crystal system provider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        CrystalSystem CreateCrystalSystem(ICrystalSystemProvider provider);

        /// <summary>
        /// Creates a wyckoff operation dictionary for the provided source vector that informs about which operations can be used to create which position
        /// </summary>
        /// <param name="sourceVector"></param>
        /// <returns></returns>
        IWyckoffOperationDictionary GetOperationDictionary(in Fractional3D sourceVector);

        /// <summary>
        /// Get a list interface of all symmetry operations that do not change the input vector (Optional with shift correction)
        /// </summary>
        /// <param name="sourceVector"></param>
        /// <param name="shiftCorrection"></param>
        /// <returns></returns>
        IList<ISymmetryOperation> GetMultiplicityOperations(in Fractional3D sourceVector, bool shiftCorrection);

        /// <summary>
        /// Get the point operation group for the provided origin point and point sequence based upon the currently loaded space group
        /// </summary>
        /// <param name="originPoint"></param>
        /// <param name="pointSequence"></param>
        /// <returns></returns>
        IPointOperationGroup GetPointOperationGroup(in Fractional3D originPoint, IEnumerable<Fractional3D> pointSequence);

        /// <summary>
        /// Gets the unfiltered and untrimmed list of all wyckoff extended sequences symmetry equivalent to the input sequence
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        IList<Fractional3D[]> GetAllWyckoffSequences(IEnumerable<Fractional3D> refSequence);

        /// <summary>
        /// Gets the filtered and trimmed list of all wyckoff extended seqeunces that begin in the same origin unit cell
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        SetList<Fractional3D[]> GetAllWyckoffOriginSequences(IEnumerable<Fractional3D> refSequence);

        /// <summary>
        /// Gets a sorted list of unique fractional vectors that represent all equivalent wyckoff positions to the original (Including original)
        /// </summary>
        /// <param name="refVector"></param>
        /// <returns></returns>
        SetList<Fractional3D> GetAllWyckoffPositions(Fractional3D refVector);

        /// <summary>
        /// Get multiple sorted unique lists of wyckoff extended positions that each includes the original refernce vector
        /// </summary>
        /// <param name="refVectors"></param>
        /// <returns></returns>
        List<SetList<Fractional3D>> GetAllWyckoffPositions(IEnumerable<Fractional3D> refVectors);

        /// <summary>
        /// Gets a sorted list of unique structs that implement the fractional vector interface that represent all equivalent wyckoff positions to the original (Including original)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="refVectors"></param>
        /// <returns></returns>
        SetList<TSource> GetAllWyckoffPositions<TSource>(TSource refVector) where TSource : struct, IFractional3D<TSource>;

        /// <summary>
        /// Get multiple sorted unique sets of wyckoff positions with the same type as the provided fractional vector type
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="refVectors"></param>
        /// <returns></returns>
        List<SetList<TSource>> GetAllWyckoffPositions<TSource>(IEnumerable<TSource> refVectors) where TSource : struct, IFractional3D<TSource>;

        /// <summary>
        /// Translates a reference sequence of positions onto each wyckoff 1 position
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        SortedDictionary<Fractional3D, List<Fractional3D>> CreateEnvironmentDictionary(IEnumerable<Fractional3D> refSequence);

        /// <summary>
        /// Gets a sorted list of all unique space group interfaces the service can provide
        /// </summary>
        /// <returns></returns>
        SetList<ISpaceGroup> GetFullGroupList();

        /// <summary>
        /// Tries to load a space group into the service using the provided search function (Returns false if no match is found)
        /// </summary>
        /// <param name="searchPredicate"></param>
        /// <returns></returns>
        bool TryLoadGroup(Predicate<ISpaceGroup> searchPredicate);

        /// <summary>
        /// Tries to load a space group into the service that matches the provided space group identifier (Returns false if no match is found)
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool TryLoadGroup(SpaceGroupEntry entry);

        /// <summary>
        /// Creates a vector comparer for a special type of 3D vector interface
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        IComparer<T1> GetSpecialVectorComparer<T1>() where T1 : IVector3D;

        /// <summary>
        /// Shifts a sequence of fractional vectors in a manner that the first vector in the sequence is in the (0,0,0) origin cell
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> ShiftFirstToOrigin(IEnumerable<Fractional3D> source, double tolerance);

        /// <summary>
        /// Creates the first possible operation that transforms the passed source vector onto the target.
        /// Returns null if none is found
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        ISymmetryOperation CreateOperationToTarget(in Fractional3D source, in Fractional3D target);
    }
}