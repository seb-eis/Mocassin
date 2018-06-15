using System;
using System.Collections.Generic;
using System.Text;

using ICon.Symmetry.SpaceGroups;
using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Basic interface for all space group informations that contain group index, 
    /// </summary>
    public interface ISpaceGroupInfo : IModelParameter
    {
        /// <summary>
        /// The index of the space group in the "International tables of space groups"
        /// </summary>
        int GroupIndex { get; }

        /// <summary>
        /// The literal name of the space group
        /// </summary>
        string GroupLiteral { get; }

        /// <summary>
        /// The name of the specififer in cases where a space group has multiple specififed versions for the same group index
        /// </summary>
        string SpecifierName { get; }

        /// <summary>
        /// Get the space group entry for database lookup
        /// </summary>
        /// <returns></returns>
        SpaceGroupEntry GroupEntry { get; }
    }
}
