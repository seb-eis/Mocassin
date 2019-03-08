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
    ///     Serializable data object for <see cref="Mocassin.Model.Energies.IUnstableEnvironment" /> model object creation
    /// </summary>
    [XmlRoot("UnstableEnvironment")]
    public class UnstableEnvironmentGraph : ModelObjectGraph
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
        public List<InteractionFilterGraph> InteractionFilters { get; set; }

        /// <summary>
        ///     Creates new <see cref="UnstableEnvironmentGraph"/> with empty component lists
        /// </summary>
        public UnstableEnvironmentGraph()
        {
            InteractionFilters = new List<InteractionFilterGraph>();
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
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