using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Common interface for all objects that represent a space group
    /// </summary>
    public interface ISpaceGroup : IComparable<ISpaceGroup>
    {
        /// <summary>
        ///     The index of the space group
        /// </summary>
        int Index { get; }

        /// <summary>
        ///     The literal description of the space group
        /// </summary>
        string Literal { get; }

        /// <summary>
        ///     The specifier description of the group if multiple versions exist
        /// </summary>
        string Specifier { get; }

        /// <summary>
        ///     The index of the specifier
        /// </summary>
        int SpecifierIndex { get; }

        /// <summary>
        ///     The index of the crystal system affiliated with the group (0 to 6)
        /// </summary>
        int CrystalSystemIndex { get; }

        /// <summary>
        ///     Get a <see cref="IEnumerable{T}"/> of the operation strings
        /// </summary>
        IEnumerable<string> OperationLiterals { get; }

        /// <summary>
        ///     Get the space group entry that contains group index, literal name and the specifier
        /// </summary>
        /// <returns></returns>
        SpaceGroupEntry GetGroupEntry();

        /// <summary>
        ///     Gets the sequence of <see cref="ISymmetryOperation"/> operations defined in the <see cref="ISpaceGroup"/>
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ISymmetryOperation> GetOperations();
    }
}