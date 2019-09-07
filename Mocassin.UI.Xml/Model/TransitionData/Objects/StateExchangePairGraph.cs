using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.TransitionModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IStateExchangePair" /> model object creation
    /// </summary>
    [XmlRoot("StateChange")]
    public class StateExchangePairGraph : ModelObjectGraph
    {
        private string donorParticleKey;
        private string acceptorParticleKey;

        /// <summary>
        ///     Get or set the donor state particle key
        /// </summary>
        [XmlAttribute("DonorState")]
        public string DonorParticleKey
        {
            get => donorParticleKey;
            set => SetProperty(ref donorParticleKey, value);
        }

        /// <summary>
        ///     Get or set the acceptor state particle key
        /// </summary>
        [XmlAttribute("AcceptorState")]
        public string AcceptorParticleKey
        {
            get => acceptorParticleKey;
            set => SetProperty(ref acceptorParticleKey, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
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