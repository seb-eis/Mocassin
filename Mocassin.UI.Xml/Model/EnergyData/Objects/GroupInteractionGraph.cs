using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.EnergyModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Energies.IGroupInteraction" /> model object creation
    /// </summary>
    [XmlRoot("GroupInteraction")]
    public class GroupInteractionGraph : ModelObjectGraph
    {
        private string centerUnitCellPositionKey;
        private List<VectorGraph3D> groupGeometry;

        /// <summary>
        ///     Get or set the key of the center unit cell position
        /// </summary>
        [XmlAttribute("CenterWyckoff")]
        public string CenterUnitCellPositionKey
        {
            get => centerUnitCellPositionKey;
            set => SetProperty(ref centerUnitCellPositionKey, value);
        }

        /// <summary>
        ///     Get or seth the list of surrounding position geometry vectors
        /// </summary>
        [XmlArray("BaseGeometry")]
        [XmlArrayItem("Position")]
        public List<VectorGraph3D> GroupGeometry
        {
            get => groupGeometry;
            set => SetProperty(ref groupGeometry, value);
        }

        /// <summary>
        ///     Creates new <see cref="GroupInteractionGraph" /> with empty component lists
        /// </summary>
        public GroupInteractionGraph()
        {
            GroupGeometry = new List<VectorGraph3D>();
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
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