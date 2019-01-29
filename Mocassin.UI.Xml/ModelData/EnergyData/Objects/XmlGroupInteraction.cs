using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.EnergyData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Energies.IGroupInteraction" /> model object creation
    /// </summary>
    [XmlRoot("GroupInteraction")]
    public class XmlGroupInteraction : XmlModelObject
    {
        /// <summary>
        ///     Get or set the key of the center unit cell position
        /// </summary>
        [XmlAttribute("CenterWyckoff")]
        public string CenterUnitCellPositionKey { get; set; }

        /// <summary>
        ///     Get or seth the list of surrounding position geometry vectors
        /// </summary>
        [XmlArray("BaseGeometry")]
        [XmlArrayItem("Position")]
        public List<XmlVector3D> GroupGeometry { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetPreparedModelObject()
        {
            var obj = new GroupInteraction
            {
                CenterUnitCellPosition = new UnitCellPosition {Key = CenterUnitCellPositionKey},
                GeometryVectors = GroupGeometry.Select(x => x.AsDataVector3D()).ToList()
            };
            return obj;
        }
    }
}