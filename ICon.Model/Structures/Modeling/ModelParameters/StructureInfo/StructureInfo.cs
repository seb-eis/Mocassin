using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.IStructureInfo"/>
    [DataContract(Name = "StructureInfo")]
    public class StructureInfo : ModelParameter, IStructureInfo
    {
        /// <inheritdoc />
        [DataMember]
        public string Name { get; set; }

        /// <inheritdoc />
        public override string GetParameterName()
        {
            return "'Structure Info'";
        }

        /// <summary>
        ///     Creates the default initialized structure info
        /// </summary>
        /// <returns></returns>
        public static StructureInfo CreateDefault()
        {
            return new StructureInfo {Name = "Unnamed"};
        }

        /// <inheritdoc />
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (!(modelParameter is IStructureInfo structureInfo)) 
                return null;

            Name = structureInfo.Name;
            return this;

        }

        /// <inheritdoc />
        public override bool Equals(IModelParameter other)
        {
            if (other is IStructureInfo otherInfo)
                return Name == otherInfo.Name;

            return false;
        }
    }
}