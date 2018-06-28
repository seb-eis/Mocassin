using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ICon.Mathematics.Permutation;
using ICon.Mathematics.ValueTypes;
using ICon.Framework.Collections;
using ICon.Framework.Extensions;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Represents a point operation group for a geometric sequence of points. Collection deescribes multiple relevant sets of point symmetry operations
    /// </summary>
    [DataContract]
    public class PointOperationGroup :  IPointOperationGroup
    {
        /// <summary>
        /// The space group entry this operation collection is valid fro
        /// </summary>
        [DataMember]
        public SpaceGroupEntry SpaceGroupEntry { get; set; }

        /// <summary>
        /// The origin point for the operation collection
        /// </summary>
        [DataMember]
        public DataVector3D OriginPoint { get; set; }

        /// <summary>
        /// The point sequence the collection is valid for
        /// </summary>
        [DataMember]
        public List<DataVector3D> PointSequence { get; set; }

        /// <summary>
        /// The unfiltered list of all point symmetry operations of the origin point
        /// </summary>
        [DataMember]
        public List<SymmetryOperation> PointOperations { get; set; }

        /// <summary>
        /// The filtered list of all operations that yield unique vector sequnces of the original point sequence
        /// </summary>
        /// <remarks> Unique in the sense that two sequences are not identical cannot trivially matched by inverting one </remarks>
        [DataMember]
        public List<SymmetryOperation> UniqueSequenceOperations { get; set; }

        /// <summary>
        /// Get all operations that project the original point sequence onto itself
        /// </summary>
        [DataMember]
        public List<SymmetryOperation> SelfProjectionOperations { get; set; }

        /// <summary>
        /// Matrix that descibes all possible equivalent orders of the vector sequence when performing a self projection (For value permutations)
        /// </summary>
        [DataMember]
        public List<List<int>> UniqueSelfProjectionOrders { get; set; }

        /// <summary>
        /// Interface access to the readonly representation of the origin point
        /// </summary>
        [IgnoreDataMember]
        Fractional3D IPointOperationGroup.OriginPoint => OriginPoint.AsFractional();

        /// <summary>
        /// Enumerable interface access to the point operations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISymmetryOperation> GetPointOperations()
        {
            return PointOperations.AsEnumerable();
        }

        /// <summary>
        /// Enumerable interface access to the point sequence
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Fractional3D> GetPointSequence()
        {
            return PointSequence.Select(value => value.AsFractional());
        }

        /// <summary>
        /// Enumerable interface access to the unique sequence operations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISymmetryOperation> GetUniqueSequenceOperations()
        {
            return UniqueSequenceOperations.AsEnumerable();
        }

        /// <summary>
        /// Enumerable interface access to the self projection operations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISymmetryOperation> GetSelfProjectionOperations()
        {
            return SelfProjectionOperations.AsEnumerable();
        }

        /// <summary>
        /// Get all unqiue orders of the vector sequence when it is projected onto itself with the self projection operations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int[]> GetUniqueProjectionOrders()
        {
            return UniqueSelfProjectionOrders.Select(value => value.ToArray());
        }

        /// <summary>
        /// Generate all unique permutations of the point sequence found within the passed permutation provider
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="permProvider"></param>
        /// <param name="comparer"></param>
        /// <param name="selector"></param>
        /// <returns> Value equality comparer and hash value selector are used for the internal hash set filetring </returns>
        public IEnumerable<T1[]> GetUniquePermutations<T1>(IPermutationProvider<T1> permProvider, IEqualityComparer<T1> comparer, Func<T1, int> selector)
        {
            if (permProvider.ResultLength != PointSequence.Count)
            {
                throw new ArgumentException("Permutation provider does not match the point sequence");
            }

            if (!HasPermutationMultiplicity())
            {
                return permProvider.AsEnumerable();
            }
            return new HashSet<T1[]>(permProvider, MakePermutationEqualityComparer(comparer, value => value.Sum(a => selector(a)))).AsEnumerable();
        }

        /// <summary>
        /// Returns true if permuting the point squence with values can contain multiple equivalent sequences
        /// </summary>
        /// <remarks> This is the case if more than one unique order of the self projection exists </remarks>
        /// <returns></returns>
        public bool HasPermutationMultiplicity()
        {
            return UniqueSelfProjectionOrders.Count != 1;
        }

        /// <summary>
        /// Checks if two permutations sets are directly identical or a equivalent within one of the existing equivalent self projection orders
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="valueComparer"></param>
        /// <returns></returns>
        protected IEqualityComparer<T1[]> MakePermutationEqualityComparer<T1>(IEqualityComparer<T1> valueComparer, Func<T1[], int> hashFunction)
        {
            bool compareEquality(T1[] lhs, T1[] rhs)
            {
                if (lhs.Length == rhs.Length)
                {
                    foreach (var vectorOrder in UniqueSelfProjectionOrders)
                    {
                        int index = -1;
                        bool orderIsMatch = true;
                        foreach (var orderIndex in vectorOrder)
                        {
                            orderIsMatch = orderIsMatch & valueComparer.Equals(lhs[++index], rhs[orderIndex]);
                        }
                        if (orderIsMatch == true)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return new EqualityCompareAdapter<T1[]>(compareEquality, hashFunction);
        }
    }
}
