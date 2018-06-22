using System;
using System.Collections.Generic;
using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Permutation;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Represents a point operation group that provides various symmetry operation sets to deal with symmetry equivalent point geoemtry sequences
    /// </summary>
    public interface IPointOperationGroup
    {
        /// <summary>
        /// The space group entry this operation collection is valid fro
        /// </summary>
        SpaceGroupEntry SpaceGroupEntry { get; }

        /// <summary>
        /// The origin point for the operation collection
        /// </summary>
        Fractional3D OriginPoint { get; }

        /// <summary>
        /// Get the point sequence the collection is valid for
        /// </summary>
        IEnumerable<Fractional3D> GetPointSequence();

        /// <summary>
        /// Get the unfiltered list of all point symmetry operations of the origin point
        /// </summary>
        IEnumerable<ISymmetryOperation> GetPointOperations();

        /// <summary>
        /// Get the filtered list of all operations that yield unique vector sequnces of the original point sequence
        /// </summary>
        /// <remarks> Unique in the sense that two sequences are not identical cannot trivially matched by inverting one </remarks>
        IEnumerable<ISymmetryOperation> GetUniqueSequenceOperations();

        /// <summary>
        /// Get the filtered list of symmetry operations that project the original sequence onto itself in any order
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISymmetryOperation> GetSelfProjectionOperations();

        /// <summary>
        /// Get a sequence of indices that assigns each vector point an index describing its projection set
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> GetProjectionIndexing();

        /// <summary>
        /// Returns true if permuting the point squence with values can show multiple equivalent values
        /// </summary>
        /// <returns></returns>
        bool HasPermutationMultiplicity();
    }
}
