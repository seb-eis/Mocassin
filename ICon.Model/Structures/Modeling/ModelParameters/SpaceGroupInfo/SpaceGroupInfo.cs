using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Structures
{
    /// <summary>
    /// Space group info class (Simple warpper for the space group entry class to qualify as both a model parameter and a space group entry)
    /// </summary>
    [DataContract(Name ="SpaceGroupInfo")]
    public class SpaceGroupInfo : ModelParameter, ISpaceGroupInfo
    {
        /// <summary>
        /// The wrapped space group entry instance
        /// </summary>
        [DataMember]
        public SpaceGroupEntry GroupEntry { get; set; }

        /// <summary>
        /// The index of the group in the "International tables of space groups"
        /// </summary>
        [IgnoreDataMember]
        public int GroupIndex => GroupEntry.Index;

        /// <summary>
        /// The literal name of the space group
        /// </summary>
        [IgnoreDataMember]
        public string GroupLiteral => GroupEntry.Literal;

        /// <summary>
        /// The specifier name of the group in cases where multiple versions of the same group index exist
        /// </summary>
        [IgnoreDataMember]
        public string SpecifierName => GroupEntry.Specifier;

        /// <summary>
        /// Get the type name of the model parameter
        /// </summary>
        /// <returns></returns>
        public override string GetParameterName()
        {
            return "'Space Group Info'";
        }

        /// <summary>
        /// Implicit cast of a space group entry to the wrapper class object
        /// </summary>
        /// <param name="groupEntry"></param>
        public static implicit operator SpaceGroupInfo(SpaceGroupEntry groupEntry)
        {
            return new SpaceGroupInfo() { GroupEntry = groupEntry };
        }

        /// <summary>
        /// Copies values from model parameter interface if possible and returns filled object (Returns null if type mismatch)
        /// </summary>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (modelParameter is ISpaceGroupInfo casted)
            {
                GroupEntry = new SpaceGroupEntry(casted.GroupIndex, casted.GroupLiteral, casted.SpecifierName);
                return this;
            }
            return null;
        }

        /// <summary>
        /// Checks if the other model parameter interface contains the same space group info (Returns false if type mismatch)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IModelParameter other)
        {
            if (other is ISpaceGroupInfo casted)
            {
                return GroupIndex == casted.GroupIndex
                    && GroupLiteral == casted.GroupLiteral
                    && SpecifierName == casted.SpecifierName;
            }
            return false;
        }
    }
}
