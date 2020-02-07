using System;
using System.Collections.Generic;
using Mocassin.Symmetry.CrystalSystems;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Common interface for all objects that represent a space group
    /// </summary>
    public interface ISpaceGroup : IComparable<ISpaceGroup>
    {
        /// <summary>
        ///     The index of the space group in the international space group tables
        /// </summary>
        int InternationalIndex { get; }

        /// <summary>
        ///     The Mauguin notation <see cref="string"/> of the group
        /// </summary>
        string MauguinNotation { get; }

        /// <summary>
        ///     The name of the <see cref="CrystalSystemVariation" />
        /// </summary>
        string VariationName { get; }

        /// <summary>
        ///     The <see cref="CrystalSystemVariation" /> of the group
        /// </summary>
        CrystalSystemVariation CrystalVariation { get; }

        /// <summary>
        ///     The <see cref="CrystalSystemType" /> of the group
        /// </summary>
        CrystalSystemType CrystalType { get; }

        /// <summary>
        ///     Gets the sequence of <see cref="ISymmetryOperation" /> operations defined in the <see cref="ISpaceGroup" />
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ISymmetryOperation> Operations { get; }

        /// <summary>
        ///     Get a <see cref="IEnumerable{T}" /> of the operation strings
        /// </summary>
        IEnumerable<string> OperationLiterals { get; }

        /// <summary>
        ///     Get the space group entry that contains group index, literal name and the specifier
        /// </summary>
        /// <returns></returns>
        SpaceGroupEntry GetGroupEntry();
    }
}