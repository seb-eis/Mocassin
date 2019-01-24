using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.BaseData;
using Mocassin.UI.Xml.StructureData;

namespace Mocassin.UI.Xml.EnergyData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Energies.IUnstableEnvironment" /> model object creation
    /// </summary>
    [XmlRoot("UnstableEnvironment")]
    public class XmlUnstableEnvironment : XmlModelObject
    {
        /// <summary>
        /// Get or set the maximum interaction range
        /// </summary>
        [XmlAttribute("InteractionRadius")]
        public double MaxInteractionRange { get; set; }

        /// <summary>
        /// Get or set the key of the center unit cell position
        /// </summary>
        [XmlAttribute("WyckoffPosition")]
        public string UnitCellPositionKey { get; set; }

        /// <summary>
        /// Get or set the list of ignored wyckoff positions during interaction search
        /// </summary>
        [XmlArray("IgnoredInteractions")]
        [XmlArrayItem("Position")]
        public List<XmlUnitCellPosition> IgnoredUnitCellPositions { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetPreparedModelObject()
        {
            var obj = new UnstableEnvironment
            {
                MaxInteractionRange = MaxInteractionRange,
                UnitCellPosition = new UnitCellPosition {Key = UnitCellPositionKey},
                IgnoredPositions = IgnoredUnitCellPositions.Select(x => x.GetInputObject()).Cast<IUnitCellPosition>().ToList()
            };
            return obj;
        }
    }
}