using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Comparer;
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
        ///     The unfiltered list of all symmetry operations that do not shift the origin point
        /// </summary>
        public List<SymmetryOperation> LocalSequenceOperations { get; set; }

        /// <summary>
        ///     The filtered list of all operations that yield unique vector sequences of the original point sequence (Does not
        ///     remove sequences where the only difference is the point order)
        /// </summary>
        public List<SymmetryOperation> OrderPreservingUniqueSequenceOperations { get; set; }

        /// <summary>
        ///     The filtered list of all operations that yield unique vector sequences of the original point sequence (Removes
        ///     sequences where the only difference is the point order)
        /// </summary>
        public List<SymmetryOperation> OrderIgnoringUniqueSequenceOperations { get; set; }

        /// <summary>
        ///     Get all operations that project the original point sequence onto itself in any order
        /// </summary>
        public List<SymmetryOperation> OrderIgnoringSelfProjectionOperations { get; set; }

        /// <inheritdoc />
        public int UniqueOriginSiteCount { get; set; }

        /// <inheritdoc />
        public int OrderPreservingExtensionCountPerSite => OrderPreservingUniqueSequenceOperations.Count;

        /// <inheritdoc />
        public int OrderIgnoringExtensionCountPerSite => OrderIgnoringUniqueSequenceOperations.Count;

        /// <inheritdoc />
        public bool IsFullSelfProjection => LocalSequenceOperations.Count == OrderIgnoringSelfProjectionOperations.Count;

        /// <summary>
        ///     Matrix that describes all possible equivalent orders of the vector sequence when performing a self projection (For
        ///     value permutations)
        /// </summary>
        public List<List<int>> SelfProjectionOrders { get; set; }

        /// <inheritdoc />
        public IEnumerable<IEnumerable<Fractional3D>> GetAllUniqueSequencesWithPreservedPointOrder()
        {
            var vectorSequence = GetPointSequence().ToList();
            foreach (var operation in OrderPreservingUniqueSequenceOperations)
                yield return operation.Transform(vectorSequence);
        }

        /// <inheritdoc />
        public IEnumerable<IEnumerable<Fractional3D>> GetUniqueSequencesWithoutPreservedPointOrder()
        {
            var vectorSequence = GetPointSequence().ToList();
            foreach (var operation in OrderIgnoringUniqueSequenceOperations)
                yield return operation.Transform(vectorSequence);
        }

        /// <inheritdoc />
        public IEnumerable<ISymmetryOperation> GetPointOperations() => LocalSequenceOperations.AsEnumerable();

        /// <inheritdoc />
        public IEnumerable<Fractional3D> GetPointSequence() => PointSequence.AsReadOnly();

        /// <inheritdoc />
        public IEnumerable<ISymmetryOperation> GetOrderPreservingUniqueSequenceOperations() => OrderPreservingUniqueSequenceOperations.AsEnumerable();

        /// <inheritdoc />
        public IEnumerable<ISymmetryOperation> GetOrderIgnoringUniqueSequenceOperations() => OrderIgnoringUniqueSequenceOperations.AsEnumerable();

        /// <inheritdoc />
        public IEnumerable<ISymmetryOperation> GetOrderIndependentSelfProjectionOperations() => OrderIgnoringSelfProjectionOperations.AsEnumerable();

        /// <inheritdoc />
        public IEnumerable<int[]> GetUniqueProjectionOrders()
        {
            return SelfProjectionOrders.Select(value => value.ToArray());
        }

        /// <inheritdoc />
        public IEnumerable<T1[]> GetUniquePermutations<T1>(IPermutationSource<T1> permutationSource, IEqualityComparer<T1> comparer,
            Func<T1, int> selector)
        {
            if (permutationSource.ResultLength != PointSequence.Count)
                throw new ArgumentException("Permutation provider does not match the point sequence");

            return !HasPermutationMultiplicity()
                ? permutationSource.AsEnumerable()
                : new HashSet<T1[]>(permutationSource, MakePermutationEqualityComparer(comparer, value => value.Sum(selector)));
        }

        /// <inheritdoc />
        public bool HasPermutationMultiplicity() => SelfProjectionOrders.Count != 1;

        /// <summary>
        ///     Checks if two permutations sets are directly identical or a equivalent within one of the existing equivalent self
        ///     projection orders
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="valueComparer"></param>
        /// <param name="hashFunction"></param>
        /// <returns></returns>
        protected IEqualityComparer<T1[]> MakePermutationEqualityComparer<T1>(IEqualityComparer<T1> valueComparer, Func<T1[], int> hashFunction)
        {
            bool Equals(T1[] lhs, T1[] rhs)
            {
                if (lhs.Length != rhs.Length) return false;

                foreach (var vectorOrder in SelfProjectionOrders)
                {
                    var index = -1;
                    var orderIsMatch = true;
                    foreach (var orderIndex in vectorOrder) orderIsMatch &= valueComparer.Equals(lhs[++index], rhs[orderIndex]);
                    if (orderIsMatch) return true;
                }

                return false;
            }

            return new RelayEqualityComparer<T1[]>(Equals, hashFunction);
        }
    }
}