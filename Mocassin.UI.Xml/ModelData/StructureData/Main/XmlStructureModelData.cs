using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.StructureData
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Structures.IStructureManager" />
    ///     system
    /// </summary>
    [XmlRoot("StructureModel")]
    public class XmlStructureModelData : XmlModelManagerData
    {
        /// <summary>
        ///     Get or set the xml structure info
        /// </summary>
        [XmlElement("StructureInfo")]
        public XmlStructureInfo StructureInfo { get; set; }

        /// <summary>
        ///     Get or set the xml space group info
        /// </summary>
        [XmlElement("SpaceGroup")]
        public XmlSpaceGroupInfo SpaceGroupInfo { get; set; }

        /// <summary>
        ///     Get or set the xml cell parameters
        /// </summary>
        [XmlElement("UnitCellGeometry")]
        public XmlCellParameters CellParameters { get; set; }

        /// <summary>
        ///     Get or set the list of stable and unstable wyckoff positions
        /// </summary>
        [XmlArray("WyckoffPositions")]
        [XmlArrayItem("Position")]
        public List<XmlUnitCellPosition> UnitCellPositions { get; set; }

        /// <summary>
        ///     Get or set the list of wyckoff dummy positions that are not part of the model
        /// </summary>
        [XmlArray("DummyPositions")]
        [XmlArrayItem("Position")]
        public List<XmlDummyPosition> DummyPositions { get; set; }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield return StructureInfo.GetInputObject();
            yield return SpaceGroupInfo.GetInputObject();
            yield return CellParameters.GetInputObject();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return UnitCellPositions
                .Select(x => x.GetInputObject())
                .Concat(DummyPositions.Select(x => x.GetInputObject()));
        }
    }
}