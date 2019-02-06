using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.TransitionData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IStateExchangePair" /> model object creation
    /// </summary>
    [XmlRoot("StateChange")]
    public class XmlStateExchangePair : XmlModelObject
    {
        /// <summary>
        ///     Get or set the donor state particle key
        /// </summary>
        [XmlAttribute("DonorState")]
        public string DonorParticleKey { get; set; }

        /// <summary>
        ///     Get or set the acceptor state particle key
        /// </summary>
        [XmlAttribute("AcceptorState")]
        public string AcceptorParticleKey { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetPreparedModelObject()
        {
            var obj = new StateExchangePair
            {
                DonorParticle = new Particle {Key = DonorParticleKey},
                AcceptorParticle = new Particle {Key = AcceptorParticleKey}
            };
            return obj;
        }
    }
}