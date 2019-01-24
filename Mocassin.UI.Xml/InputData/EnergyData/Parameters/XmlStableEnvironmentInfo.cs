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
        /// Get or set the list of ignored particle pairs
        /// </summary>
        [XmlArray("IgnoredInteractions")]
        [XmlArrayItem("ParticlePair")]
        public List<XmlParticlePair> IgnoredParticlePairs { get; set; }

        /// <inheritdoc />
        protected override ModelParameter GetPreparedModelObject()
        {
            var obj = new StableEnvironmentInfo
            {
                MaxInteractionRange = MaxInteractionRange,
                IgnoredPairInteractions = IgnoredParticlePairs.Select(x => x.AsSymmetric()).ToList()
            };
            return obj;
        }
    }
}