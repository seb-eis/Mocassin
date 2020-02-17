using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.StructureModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Structures.IStructureInfo" /> model parameter creation
    /// </summary>
    [XmlRoot]
    public sealed class StructureInfoData : ModelParameterObject
    {
        /// <summary>
        ///     Creates new default <see cref="StructureInfoData" />
        /// </summary>
        public StructureInfoData()
        {
            Name = "New Structure";
        }

        /// <inheritdoc />
        protected override ModelParameter GetModelObjectInternal()
        {
            return new StructureInfo {Name = Name};
        }
    }
}