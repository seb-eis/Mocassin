using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.StructureData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Structures.IStructureInfo" /> model parameter creation
    /// </summary>
    [XmlRoot("StructureInfo")]
    public class XmlStructureInfo : XmlModelParameter
    {
        /// <summary>
        ///     The name of the structure
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <inheritdoc />
        protected override ModelParameter GetPreparedModelObject()
        {
            return new StructureInfo {Name = Name};
        }
    }
}