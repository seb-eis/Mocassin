using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.TransitionModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IStateExchangePair" /> model object creation
    /// </summary>
    [XmlRoot]
    public class StateExchangePairData : ModelDataObject
    {
        private ModelObjectReference<Particle> acceptorParticle = new ModelObjectReference<Particle>();
        private ModelObjectReference<Particle> donorParticle = new ModelObjectReference<Particle>();

        /// <summary>
        ///     Get or set the donor state particle key
        /// </summary>
        [XmlElement]
        public ModelObjectReference<Particle> DonorParticle
        {
            get => donorParticle;
            set => SetProperty(ref donorParticle, value);
        }

        /// <summary>
        ///     Get or set the acceptor state particle key
        /// </summary>
        [XmlElement]
        public ModelObjectReference<Particle> AcceptorParticle
        {
            get => acceptorParticle;
            set => SetProperty(ref acceptorParticle, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new StateExchangePair
            {
                DonorParticle = new Particle {Key = DonorParticle.Key},
                AcceptorParticle = new Particle {Key = AcceptorParticle.Key}
            };
            return obj;
        }
    }
}