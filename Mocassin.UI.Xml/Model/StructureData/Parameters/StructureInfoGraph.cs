using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.StructureModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Structures.IStructureInfo" /> model parameter creation
    /// </summary>
    [XmlRoot("StructureInfo")]
    public class StructureInfoGraph : ModelParameterGraph
    {
        /// <summary>
        ///     Creates new default <see cref="StructureInfoGraph"/>
        /// </summary>
        public StructureInfoGraph()
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