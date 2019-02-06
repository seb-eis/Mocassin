using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.EnergyData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Energies.IStableEnvironmentInfo" /> model parameter creation
    /// </summary>
    [XmlRoot("StableEnvironmentInfo")]
    public class XmlStableEnvironmentInfo : XmlModelParameter
    {
        /// <summary>
        ///     Get or set the interaction cutoff range in [Ang]
        /// </summary>
        [XmlAttribute("InteractionRadius")]
        public double MaxInteractionRange { get; set; }

        /// <summary>
        ///     Get or set the list of interaction filters for stable environments
        /// </summary>
        [XmlArray("InteractionFilters")]
        [XmlArrayItem("Filter")]
        public List<XmlInteractionFilter> InteractionFilters { get; set; }

        /// <inheritdoc />
        protected override ModelParameter GetPreparedModelObject()
        {
            var obj = new StableEnvironmentInfo
            {
                MaxInteractionRange = MaxInteractionRange,
                InteractionFilters = InteractionFilters.Select(x => x.AsSymmetric()).ToList()
            };
            return obj;
        }
    }
}