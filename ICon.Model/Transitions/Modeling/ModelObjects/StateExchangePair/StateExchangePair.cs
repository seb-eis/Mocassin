using System;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    /// State exchange pair that contains the donor and acceptor state of a position state change
    /// </summary>
    [DataContract]
    public class StateExchangePair : ModelObject, IStateExchangePair
    {
        /// <summary>
        /// The particle index of the donor state
        /// </summary>
        [DataMember]
        [IndexResolved]
        public IParticle DonorParticle { get; set; }

        /// <summary>
        /// The particle index of the acceptor state
        /// </summary>
        [DataMember]
        [IndexResolved]
        public IParticle AcceptorParticle { get; set; }

        /// <summary>
        /// Flag if the state pair involes exactly one vacancy and is therefore valid in the sense of a vacancy mechansim
        /// </summary>
        [DataMember]
        public bool IsVacancyPair { get; set; }

        /// <summary>
        /// Get the type name of the model object
        /// </summary>
        /// <returns></returns>
        public override string GetObjectName()
        {
            return "'Property State Pair'";
        }

        /// <summary>
        /// Tries to create a new model object of this type from model object interface (Returns null if wrong type or deprecated)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastIfNotDeprecated<IStateExchangePair>(obj) is var statePair)
            {
                DonorParticle = statePair.DonorParticle;
                AcceptorParticle = statePair.AcceptorParticle;
                IsVacancyPair = statePair.IsVacancyPair;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Checks if equal to other state exchange pair (Reversed case also counts as equal)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IStateExchangePair other)
        {
            if (DonorParticle == other.DonorParticle && AcceptorParticle == other.AcceptorParticle)
            {
                return true;
            }
            if (DonorParticle == other.AcceptorParticle && AcceptorParticle == other.DonorParticle)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get donor and acceptor particle indices as a two value tuple
        /// </summary>
        /// <returns></returns>
        public (int Donor, int Acceptor) AsIndexTuple()
        {
            return (DonorParticle.Index, AcceptorParticle.Index);
        }

        /// <summary>
        /// Sorts by donor index than acceptor index. Does not check for inverse equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IStateExchangePair other)
        {
            var donorComp = DonorParticle.Index.CompareTo(other.DonorParticle);
            if (donorComp == 0)
            {
                return AcceptorParticle.Index.CompareTo(other.AcceptorParticle);
            }
            return donorComp;
        }
    }
}
