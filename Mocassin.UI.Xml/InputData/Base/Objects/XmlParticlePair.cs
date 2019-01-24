using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.Particles;

namespace Mocassin.UI.Xml.BaseData
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Energies.ParticlePair" /> data objects
    /// </summary>
    [XmlRoot("ParticlePair")]
    public class XmlParticlePair
    {
        /// <summary>
        ///     Get or set the key of the center particle
        /// </summary>
        [XmlAttribute("Center")]
        public string CenterParticleKey { get; set; }

        /// <summary>
        ///     Get or set the key of the partner particle
        /// </summary>
        [XmlAttribute("Partner")]
        public string PartnerParticleKey { get; set; }

        /// <summary>
        ///     Returns the helper object content as a <see cref="Mocassin.Model.Energies.SymmetricParticlePair" /> object
        /// </summary>
        /// <returns></returns>
        public SymmetricParticlePair AsSymmetric()
        {
            return new SymmetricParticlePair
            {
                Particle0 = new Particle {Key = CenterParticleKey},
                Particle1 = new Particle {Key = PartnerParticleKey}
            };
        }

        /// <summary>
        ///     Returns the helper object content as a <see cref="Mocassin.Model.Energies.AsymmetricParticlePair" /> object
        /// </summary>
        /// <returns></returns>
        public AsymmetricParticlePair AsAsymmetric()
        {
            return new AsymmetricParticlePair
            {
                Particle0 = new Particle {Key = CenterParticleKey},
                Particle1 = new Particle {Key = PartnerParticleKey}
            };
        }
    }
}