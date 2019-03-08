using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.StructureModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Structures.IStructureManager" />
    ///     system
    /// </summary>
    [XmlRoot("StructureModel")]
    public class StructureModelGraph : ModelManagerGraph
    {
        /// <summary>
        ///     Get or set the xml structure info
        /// </summary>
        [XmlElement("StructureInfo")]
        public StructureInfoGraph StructureInfo { get; set; }

        /// <summary>
        ///     Get or set the xml space group info
        /// </summary>
        [XmlElement("SpaceGroup")]
        public SpaceGroupInfoGraph SpaceGroupInfo { get; set; }

        /// <summary>
        ///     Get or set the xml cell parameters
        /// </summary>
        [XmlElement("UnitCellGeometry")]
        public CellParametersGraph CellParameters { get; set; }

        /// <summary>
        ///     Get or set the list of stable and unstable wyckoff positions
        /// </summary>
        [XmlArray("WyckoffPositions")]
        [XmlArrayItem("Position")]
        public List<UnitCellPositionGraph> UnitCellPositions { get; set; }

        /// <summary>
        ///     Get or set the list of wyckoff dummy positions that are not part of the model
        /// </summary>
        [XmlArray("DummyPositions")]
        [XmlArrayItem("Position")]
        public List<DummyPositionGraph> DummyPositions { get; set; }

        /// <summary>
        ///     Creates a new <see cref="StructureModelGraph"/> with empty component lists
        /// </summary>
        public StructureModelGraph()
        {
            StructureInfo = new StructureInfoGraph();
            CellParameters = new CellParametersGraph();
            SpaceGroupInfo = new SpaceGroupInfoGraph();
            UnitCellPositions = new List<UnitCellPositionGraph>();
            DummyPositions = new List<DummyPositionGraph>();
        }

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