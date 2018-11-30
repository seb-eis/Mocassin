using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     The energy model specification that defines the actual used energy values for the simulation
    /// </summary>
    [XmlRoot]
    public class MmlEnergySpecification
    {
        /// <summary>
        ///     Get or set the hash value of the target context
        /// </summary>
        [XmlAttribute("Hash")]
        public int TargetContextHash { get; set; }

        /// <summary>
        ///     Get or set the list of group energy specifications that the model should use
        /// </summary>
        [XmlArray("GroupSpecifications")]
        public HashSet<MmlInteraction<MmlGroupEnergy>> GroupInteractionSpecifications { get; set; }

        /// <summary>
        ///     Get or set the list of pair energy specifications that the model should use
        /// </summary>
        [XmlArray("PairSpecifications")]
        public HashSet<MmlInteraction<MmlPairEnergy>> PairInteractionSpecifications { get; set; }
    }
}