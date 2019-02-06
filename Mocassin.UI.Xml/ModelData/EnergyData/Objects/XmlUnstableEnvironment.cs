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
    ///     Serializable data object for <see cref="Mocassin.Model.Energies.IUnstableEnvironment" /> model object creation
    /// </summary>
    [XmlRoot("UnstableEnvironment")]
    public class XmlUnstableEnvironment : XmlModelObject
    {
        /// <summary>
        ///     Get or set the maximum interaction range
        /// </summary>
        [XmlAttribute("InteractionRadius")]
        public double MaxInteractionRange { get; set; }

        /// <summary>
        ///     Get or set the key of the center unit cell position
        /// </summary>
        [XmlAttribute("WyckoffPosition")]
        public string UnitCellPositionKey { get; set; }

        /// <summary>
        ///     Get or set the list of interaction filters of the environment
        /// </summary>
        [XmlArray("InteractionFilters")]
        [XmlArrayItem("Filter")]
        public List<XmlInteractionFilter> InteractionFilters { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetPreparedModelObject()
        {
            var obj = new UnstableEnvironment
            {
                MaxInteractionRange = MaxInteractionRange,
                UnitCellPosition = new UnitCellPosition {Key = UnitCellPositionKey},
                InteractionFilters = InteractionFilters.Select(x => x.AsAsymmetric()).ToList()
            };
            return obj;
        }
    }
}