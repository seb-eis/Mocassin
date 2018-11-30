using System.Xml.Serialization;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Abstract base class for all interaction setting implementations that carry an energy value and a description
    ///     for the interaction type
    /// </summary>
    [XmlRoot]
    public abstract class MmlEnergy
    {
        /// <summary>
        ///     Get or set the interaction energy value
        /// </summary>
        [XmlAttribute]
        public double Energy { get; set; }

        /// <summary>
        ///     Get or set the interaction description the energy value is for
        /// </summary>
        [XmlElement]
        public string Description { get; set; }
    }
}