using System;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Common interface for all objects that represent a space group
    /// </summary>
    public interface ISpaceGroup : IComparable<ISpaceGroup>
    {
        /// <summary>
        /// The index of the space group
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The literal description of the space group
        /// </summary>
        string Literal { get; }

        /// <summary>
        /// The specifier description of the group if multiple versions exist
        /// </summary>
        string Specifier { get; }

        /// <summary>
        /// The index of the specifier
        /// </summary>
        int SpecifierIndex { get; }

        /// <summary>
        /// The index of the crystal system affiliated with the group (0 to 6)
        /// </summary>
        int CrystalSystemIndex { get; }

        /// <summary>
        /// Get the space group entry that contains group index, literal name and the spcifier
        /// </summary>
        /// <returns></returns>
        SpaceGroupEntry GetGroupEntry();
    }
}