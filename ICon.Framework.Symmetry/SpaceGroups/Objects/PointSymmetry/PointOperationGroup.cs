using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ICon.Mathematics.Permutation;
using ICon.Mathematics.ValueTypes;
using ICon.Symmetry.Analysis;

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
        /// List that assigns each position a projection index meaning the positions can be created from the same original vector
        /// </summary>
        [DataMember]
        public List<int> ProjectionPointIndexing { get; set; }

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
        /// Enumerbale interface access to the self projection indexing
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetProjectionIndexing()
        {
            return ProjectionPointIndexing.AsEnumerable();
        }

        /// <summary>
        /// Generate all geoemtric unique permutations of the point sequence with the provided values
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="permProvider"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<T1[]> GetGeometryUniquePermutations<T1>(IPermutationProvider<T1> permProvider, IEqualityComparer<T1> comparer)
        {
            return null;
        }

        /// <summary>
        /// Returns true if permuting the point squence with values can show multiple equivalent values
        /// </summary>
        /// <remarks> This is the case if the  </remarks>
        /// <returns></returns>
        public bool HasPermutationMultiplicity()
        {
            return false;
        }

        /// <summary>
        /// CEchks if two permutations sets are identical by means of the point projection indexing sequence (Geoemtric equivalency)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected bool AreIdenticalPermutations<T1>(T1[] lhs, T1[] rhs, IEqualityComparer<T1> comparer)
        {
            for (int i = 0; i < lhs.Length; i++)
            {
                if (!comparer.Equals(lhs[i], rhs[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
