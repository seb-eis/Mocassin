using System.Collections.Generic;
using System.Xml.Serialization;
using Mocassin.UI.Xml.EnergyData;
using Mocassin.UI.Xml.ParticleData;
using Mocassin.UI.Xml.StructureData;
using Mocassin.UI.Xml.TransitionData;

namespace Mocassin.UI.Xml.ProjectData
{
    /// <summary>
    ///     The main root for mocassin project data input as a serialized information
    /// </summary>
    [XmlRoot("MocassinProjectData")]
    public class XmlMocassinProjectData
    {
        /// <summary>
        ///     Get or set the input particle data
        /// </summary>
        [XmlElement("ParticleModel")]
        [ModelInputRoot(0)]
        public XmlParticleData ParticleData { get; set; }

        /// <summary>
        ///     Get or set the input structure data
        /// </summary>
        [XmlElement("StructureModel")]
        [ModelInputRoot(1)]
        public XmlStructureData StructureData { get; set; }

        /// <summary>
        ///     Get or set the input transition data
        /// </summary>
        [XmlElement("TransitionModel")]
        [ModelInputRoot(2)]
        public XmlTransitionData TransitionData { get; set; }

        /// <summary>
        ///     Get or set the input energy data
        /// </summary>
        [XmlElement("EnergyModel")]
        [ModelInputRoot(3)]
        public XmlEnergyData EnergyData { get; set; }
    }
}