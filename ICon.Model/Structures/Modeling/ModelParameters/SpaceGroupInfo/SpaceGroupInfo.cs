using Mocassin.Model.Basic;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.ISpaceGroupInfo" />
    public class SpaceGroupInfo : ModelParameter, ISpaceGroupInfo
    {
        /// <inheritdoc />
        public SpaceGroupEntry GroupEntry { get; set; }

        /// <inheritdoc />
        public int GroupNumber => GroupEntry.GroupNumber;

        /// <inheritdoc />
        public string GroupLiteral => GroupEntry.Literal;

        /// <inheritdoc />
        public string SpecifierName => GroupEntry.CrystalVariation.ToString();

        /// <inheritdoc />
        public override string GetParameterName() => "Space Group Info";

        /// <inheritdoc />
        public override bool Equals(IModelParameter other)
        {
            if (other is ISpaceGroupInfo otherInfo)
            {
                return GroupNumber == otherInfo.GroupNumber
                       && GroupLiteral == otherInfo.GroupLiteral
                       && SpecifierName == otherInfo.SpecifierName;
            }

            return false;
        }

        /// <summary>
        ///     Implicit cast of a space group entry to the wrapper class object
        /// </summary>
        /// <param name="groupEntry"></param>
        public static implicit operator SpaceGroupInfo(SpaceGroupEntry groupEntry) => new SpaceGroupInfo {GroupEntry = groupEntry};

        /// <inheritdoc />
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (!(modelParameter is ISpaceGroupInfo groupInfo))
                return null;

            GroupEntry = new SpaceGroupEntry(groupInfo.GroupNumber, groupInfo.GroupLiteral, groupInfo.GroupEntry.CrystalVariation);
            return this;
        }
    }
}