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
        ///     Get the number of unique group extensions per site where the order of participants is preserved
        /// </summary>
        int OrderPreservingExtensionCountPerSite { get; }

        /// <summary>
        ///     Get the number of unique group extensions per site where the order of participants is ignored
        /// </summary>
        int OrderIgnoringExtensionCountPerSite { get; }

        /// <summary>
        ///     Get the number of possible origin sites
        /// </summary>
        int UniqueOriginSiteCount { get; }

        /// <summary>
        ///     Get a boolean flag that indicates if all point operations are self projections
        /// </summary>
        bool IsFullSelfProjection { get; }

        /// <summary>
        ///     Get the point sequence the collection is valid for
        /// </summary>
        IEnumerable<Fractional3D> GetPointSequence();

        /// <summary>
        ///     Get all existing unique point sequences of the group that consider original order of participants (May contain
        ///     entries where only the order of points is different)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEnumerable<Fractional3D>> GetAllUniqueSequencesWithPreservedPointOrder();

        /// <summary>
        ///     Get all existing unique point sequences of the group that do not consider original order of participants (Cannot
        ///     contain entries where only the order of points is different)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEnumerable<Fractional3D>> GetUniqueSequencesWithoutPreservedPointOrder();

        /// <summary>
        ///     Get the unfiltered list of all point symmetry operations of the origin point
        /// </summary>
        IEnumerable<ISymmetryOperation> GetPointOperations();

        /// <summary>
        ///     Get all existing unique sequence operations of the group that consider original order of participants (May contain
        ///     entries where only the order of points is different)
        /// </summary>
        IEnumerable<ISymmetryOperation> GetOrderPreservingUniqueSequenceOperations();

        /// <summary>
        ///     Get all existing unique sequence operations of the group that consider original order of participants (May contain
        ///     entries where only the order of points is different)
        /// </summary>
        IEnumerable<ISymmetryOperation> GetOrderIgnoringUniqueSequenceOperations();

        /// <summary>
        ///     Get the filtered list of symmetry operations that project the original sequence onto itself in any order
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISymmetryOperation> GetOrderIndependentSelfProjectionOperations();

        /// <summary>
        ///     Get a sequence of index lists that describe the possible orders if the vector sequence is projected onto itself
        /// </summary>
        /// <returns></returns>
        IEnumerable<int[]> GetUniqueProjectionOrders();

        /// <summary>
        ///     Get all unique permutations (considering value and geometry) within the provided permutation provider using the
        ///     provided equality comparer and single value selector function for hash value generation
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="permutationSource"></param>
        /// <param name="comparer"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IEnumerable<T1[]> GetUniquePermutations<T1>(IPermutationSource<T1> permutationSource, IEqualityComparer<T1> comparer, Func<T1, int> selector);

        /// <summary>
        ///     Returns true if permuting the point sequence with values can show multiple equivalent values
        /// </summary>
        /// <returns></returns>
        bool HasPermutationMultiplicity();
    }
}