using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Represents a position operation dictionary for lookup of the operation lists of specific positions
    /// </summary>
    public interface IPositionOperationDictionary : IReadOnlyDictionary<Fractional3D, IEnumerable<ISymmetryOperation>>
    {
        /// <summary>
        ///     The space group the dictionary belongs to
        /// </summary>
        ISpaceGroup SpaceGroup { get; }

        /// <summary>
        ///     The original source position
        /// </summary>
        Fractional3D SourcePosition { get; }
    }
}