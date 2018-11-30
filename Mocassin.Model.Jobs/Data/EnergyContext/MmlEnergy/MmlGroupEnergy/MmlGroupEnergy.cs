using System.Xml.Serialization;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     A group interaction setting that assigns a single energy value to a state code and center particle and carries a
    ///     state description for identification by the user
    /// </summary>
    [XmlRoot]
    public class MmlGroupEnergy : MmlEnergy
    {
        /// <summary>
        ///     The center particle id that the setting belongs to
        /// </summary>
        [XmlAttribute("CenterId")]
        public int CenterParticleIndex { get; set; }

        /// <summary>
        ///     The state code that encodes the state occupation for lookup in the model context
        /// </summary>
        [XmlAttribute("Code")]
        public long StateCode { get; set; }
    }
}