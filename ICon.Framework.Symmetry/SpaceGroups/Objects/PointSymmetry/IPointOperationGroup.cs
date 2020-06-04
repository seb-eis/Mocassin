using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Permutation;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Represents a point operation group that provides various symmetry operation sets to deal with symmetry equivalent
    ///     point geometry sequences
    /// </summary>
    public interface IPointOperationGroup
    {
        /// <summary>
        ///     The space group entry this operation collection is valid fro
        /// </summary>
        SpaceGroupEntry SpaceGroupEntry { get; }

        /// <summary>
        ///     The origin point for the operation collection
        /// </summary>
        Fractional3D OriginPoint { get; }

        /// <summary>
        ///     Get the point sequence the collection is valid for
        /// </summary>
        IEnumerable<Fractional3D> GetPointSequence();

        /// <summary>
        ///     Get all existing unique point sequences of the group
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEnumerable<Fractional3D>> GetUniquePointSequences();

        /// <summary>
        ///     Get the unfiltered list of all point symmetry operations of the origin point
        /// </summary>
        IEnumerable<ISymmetryOperation> GetPointOperations();

        /// <summary>
        ///     Get the filtered list of all operations that yield unique vector sequences of the original point sequence
        /// </summary>
        /// <remarks> Unique in the sense that two sequences are not identical and cannot trivially matched by inverting one </remarks>
        IEnumerable<ISymmetryOperation> GetUniqueSequenceOperations();

        /// <summary>
        ///     Get the filtered list of symmetry operations that project the original sequence onto itself in any order
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISymmetryOperation> GetSelfProjectionOperations();

        /// <summary>
        ///     Get a sequence of index lists that describe the possible orders if the vector sequence is projected onto itself
        /// </summary>
        /// <returns></returns>
        IEnumerable<int[]> GetUniqueProjectionOrders();

        /// <summary>
        ///     Get all unique permutations (Value and geometry) within the provided permutation provider using the provided
        ///     equality comparer and single value selector
        ///     function for hash value generation
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="permProvider"></param>
        /// <param name="comparer"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IEnumerable<T1[]> GetUniquePermutations<T1>(IPermutationSource<T1> permProvider, IEqualityComparer<T1> comparer,
            Func<T1, int> selector);

        /// <summary>
        ///     Returns true if permuting the point sequence with values can show multiple equivalent values
        /// </summary>
        /// <returns></returns>
        bool HasPermutationMultiplicity();

        /// <summary>
        ///     Get the number of unique group extensions per site
        /// </summary>
        int ExtensionCountPerSite { get; }

        /// <summary>
        ///     Get the number of possible origin sites
        /// </summary>
        int UniqueOriginSiteCount { get; }
    }
}