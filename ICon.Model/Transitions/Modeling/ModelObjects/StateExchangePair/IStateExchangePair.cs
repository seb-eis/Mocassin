using System;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents a state exchange pair of two particles where one is the acceptor state and the other the donor state
    /// </summary>
    public interface IStateExchangePair : IModelObject, IEquatable<IStateExchangePair>, IComparable<IStateExchangePair>
    {
        /// <summary>
        ///     Flag if the property state pair belongs to a vacancy mechanism (Exactly one of the states is a vacancy)
        /// </summary>
        bool IsVacancyPair { get; }

        /// <summary>
        ///     Flag if the exchange pair is only valid for unstable positions
        /// </summary>
        bool IsUnstablePositionPair { get; }

        /// <summary>
        ///     The index of the particle representing the donor state
        /// </summary>
        IParticle DonorParticle { get; }

        /// <summary>
        ///     The index of the particle representing the acceptor state
        /// </summary>
        IParticle AcceptorParticle { get; }

        /// <summary>
        ///     Get the state pair particle index information as a two value tuple
        /// </summary>
        /// <returns></returns>
        (int Donor, int Acceptor) AsIndexTuple();
    }
}