using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Generic base class for implementations of interaction specifications that carry interaction settings for a shared
    ///     energy model context object
    /// </summary>
    [XmlRoot]
    public class MmlInteraction<TEnergy> where TEnergy : MmlEnergy
    {
        /// <summary>
        ///     Get or set the id of the model context object that the specification targets
        /// </summary>
        [XmlAttribute("TargetModel")]
        public int ModelContextId { get; set; }

        /// <summary>
        ///     Get or set the hash value for the target model instance
        /// </summary>
        [XmlAttribute("Hash")]
        public int TargetModelHash { get; set; }

        /// <summary>
        ///     Get or set the basic description of the interaction
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }

        /// <summary>
        ///     Get or set the list of position infos of the interaction specification
        /// </summary>
        [XmlArray("Positions")]
        public List<MmlPosition> Positions { get; set; }

        /// <summary>
        ///     Get or set the list of interaction settings of the specification
        /// </summary>
        [XmlArray("Energies")]
        public List<TEnergy> Energies { get; set; }
    }
}