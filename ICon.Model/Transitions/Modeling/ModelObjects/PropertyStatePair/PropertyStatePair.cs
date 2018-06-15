using System;
using System.Runtime.Serialization;
using ICon.Model.Basic;
using ICon.Model.Particles;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Property state pair that contains the donor and acceptor state of a property exchange
    /// </summary>
    [DataContract(Name ="PropertyStatePair")]
    public class PropertyStatePair : ModelObject, IPropertyStatePair
    {
        /// <summary>
        /// The particle index of the donor state
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public IParticle DonorParticle { get; set; }

        /// <summary>
        /// The particle index of the acceptor state
        /// </summary>
        [DataMember]
        [IndexResolvable]
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
        public override string GetModelObjectName()
        {
            return "'Property State Pair'";
        }

        /// <summary>
        /// Tries to create a new model object of this type from model object interface (Returns null if wrong type or deprecated)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IPropertyStatePair>(obj) is var statePair)
            {
                DonorParticle = statePair.DonorParticle;
                AcceptorParticle = statePair.AcceptorParticle;
                IsVacancyPair = statePair.IsVacancyPair;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Checks if equal to other property state pair (Reversed case also counts as equal)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IPropertyStatePair other)
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
    }
}
