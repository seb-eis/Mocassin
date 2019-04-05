using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="Mocassin.Model.Transitions.IStateExchangePair" />
    [DataContract]
    public class StateExchangePair : ModelObject, IStateExchangePair
    {
        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public IParticle DonorParticle { get; set; }

        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public IParticle AcceptorParticle { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        public bool IsUnstablePositionPair => AcceptorParticle.Index == Particle.VoidIndex;

        /// <inheritdoc />
        [IgnoreDataMember]
        public bool IsVacancyPair => (DonorParticle?.IsVacancy ?? false) ^ (AcceptorParticle?.IsVacancy ?? false);

	    /// <inheritdoc />
	    public override string ObjectName => "State Exchange Pair";

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IStateExchangePair>(obj) is IStateExchangePair statePair))
                return null;

            DonorParticle = statePair.DonorParticle;
            AcceptorParticle = statePair.AcceptorParticle;
            return this;
        }

        /// <inheritdoc />
        public bool Equals(IStateExchangePair other)
        {
            if (other == null)
                return false;

            if (DonorParticle == other.DonorParticle && AcceptorParticle == other.AcceptorParticle)
                return true;

            return DonorParticle == other.AcceptorParticle && AcceptorParticle == other.DonorParticle;
        }

        /// <inheritdoc />
        public (int Donor, int Acceptor) AsIndexTuple()
        {
            return (DonorParticle.Index, AcceptorParticle.Index);
        }

        /// <summary>
        ///     Sorts by donor index than acceptor index. Does not check for inverse equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IStateExchangePair other)
        {
            var donorComp = DonorParticle.Index.CompareTo(other.DonorParticle);
            return donorComp == 0
                ? AcceptorParticle.Index.CompareTo(other.AcceptorParticle)
                : donorComp;
        }
    }
}