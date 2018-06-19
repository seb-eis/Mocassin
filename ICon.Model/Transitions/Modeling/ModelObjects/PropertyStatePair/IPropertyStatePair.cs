using System;
using System.Collections.Generic;
using ICon.Model.Basic;
using ICon.Model.Particles;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a property pair of two particles where one is the acceptor state and the other the donor state
    /// </summary>
    public interface IPropertyStatePair : IModelObject, IEquatable<IPropertyStatePair>, IComparable<IPropertyStatePair>
    {
        /// <summary>
        /// Flag if the property state pair belongs to a vacancy mechanism (Exactly one of the states is a vacancy)
        /// </summary>
        bool IsVacancyPair { get; }

        /// <summary>
        /// The index of the particle representing the donor state
        /// </summary>
        IParticle DonorParticle { get; }

        /// <summary>
        /// The index of the particle representing the acceptor state
        /// </summary>
        IParticle AcceptorParticle { get; }

        /// <summary>
        /// Get the state pair particle index information as a two value tuple
        /// </summary>
        /// <returns></returns>
        (int Donor, int Acceptor) AsIndexTuple();
    }
}
