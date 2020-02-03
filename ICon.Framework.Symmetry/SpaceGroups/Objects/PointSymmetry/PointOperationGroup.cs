using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.Permutation;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <inheritdoc />
    public class PointOperationGroup : IPointOperationGroup
    {
        /// <inheritdoc />
        public SpaceGroupEntry SpaceGroupEntry { get; set; }

        /// <summary>
        ///     The origin point for the operation collection
        /// </summary>
        public Fractional3D OriginPoint { get; set; }

        /// <summary>
        ///     The point sequence the collection is valid for
        /// </summary>
        public List<Fractional3D> PointSequence { get; set; }

        /// <summary>
        ///     The unfiltered list of all point symmetry operations of the origin point
        /// </summary>
        public List<SymmetryOperation> PointOperations { get; set; }

        /// <summary>
        ///     The filtered list of all operations that yield unique vector sequences of the original point sequence
        /// </summary>
        /// <remarks> Unique in the sense that two sequences are not identical cannot trivially matched by inverting one </remarks>
        public List<SymmetryOperation> UniqueSequenceOperations { get; set; }

        /// <summary>
        ///     Get all operations that project the original point sequence onto itself
        /// </summary>
        public List<SymmetryOperation> SelfProjectionOperations { get; set; }

        /// <summary>
        ///     Matrix that describes all possible equivalent orders of the vector sequence when performing a self projection (For
        ///     value permutations)
        /// </summary>
        public List<List<int>> UniqueSelfProjectionOrders { get; set; }

        /// <inheritdoc />
        public IEnumerable<IEnumerable<Fractional3D>> GetUniquePointSequences()
        {
            var vectorSequence = GetPointSequence().ToList();
            foreach (var operation in UniqueSequenceOperations)
                yield return operation.Transform(vectorSequence);
        }

        /// <inheritdoc />
        public IEnumerable<ISymmetryOperation> GetPointOperations()
        {
            return PointOperations.AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> GetPointSequence()
        {
            return PointSequence.AsReadOnly();
        }

        /// <inheritdoc />
        public IEnumerable<ISymmetryOperation> GetUniqueSequenceOperations()
        {
            return UniqueSequenceOperations.AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<ISymmetryOperation> GetSelfProjectionOperations()
        {
            return SelfProjectionOperations.AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<int[]> GetUniqueProjectionOrders()
        {
            return UniqueSelfProjectionOrders.Select(value => value.ToArray());
        }

        /// <inheritdoc />
        public IEnumerable<T1[]> GetUniquePermutations<T1>(IPermutationSource<T1> permProvider, IEqualityComparer<T1> comparer,
            Func<T1, int> selector)
        {
            if (permProvider.ResultLength != PointSequence.Count)
                throw new ArgumentException("Permutation provider does not match the point sequence");

            return !HasPermutationMultiplicity()
                ? permProvider.AsEnumerable()
                : new HashSet<T1[]>(permProvider, MakePermutationEqualityComparer(comparer, value => value.Sum(selector)))
                    .AsEnumerable();
        }

        /// <inheritdoc />
        public bool HasPermutationMultiplicity()
        {
            return UniqueSelfProjectionOrders.Count != 1;
        }

        /// <summary>
        ///     Checks if two permutations sets are directly identical or a equivalent within one of the existing equivalent self
        ///     projection orders
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="valueComparer"></param>
        /// <param name="hashFunction"></param>
        /// <returns></returns>
        protected IEqualityComparer<T1[]> MakePermutationEqualityComparer<T1>(IEqualityComparer<T1> valueComparer,
            Func<T1[], int> hashFunction)
        {
            bool Equals(T1[] lhs, T1[] rhs)
            {
                if (lhs.Length != rhs.Length)
                    return false;

                foreach (var vectorOrder in UniqueSelfProjectionOrders)
                {
                    var index = -1;
                    var orderIsMatch = true;
                    foreach (var orderIndex in vectorOrder)
                        orderIsMatch = orderIsMatch & valueComparer.Equals(lhs[++index], rhs[orderIndex]);

                    if (orderIsMatch)
                        return true;
                }

                return false;
            }

            return new EqualityCompareAdapter<T1[]>(Equals, hashFunction);
        }
    }
}