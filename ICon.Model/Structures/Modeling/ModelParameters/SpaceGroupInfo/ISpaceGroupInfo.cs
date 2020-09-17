using Mocassin.Model.Basic;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Basic interface for all space group information that contain group index,
    /// </summary>
    public interface ISpaceGroupInfo : IModelParameter
    {
        /// <summary>
        ///     The index of the space group in the "International tables of space groups"
        /// </summary>
        int GroupNumber { get; }

        /// <summary>
        ///     The literal name of the space group
        /// </summary>
        string GroupLiteral { get; }

        /// <summary>
        ///     The name of the specifier in cases where a space group has multiple specified versions for the same group index
        /// </summary>
        string SpecifierName { get; }

        /// <summary>
        ///     Get the space group entry for database lookup
        /// </summary>
        /// <returns></returns>
        SpaceGroupEntry GroupEntry { get; }
    }
}