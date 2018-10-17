using System;
using System.Runtime.Serialization;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a polar pair code of two particle indices that are used to identify a pair interaction. Order of indices
    ///     is relevant in this case (Polar behavior)
    /// </summary>
    [DataContract(Name = "AsymmetricPairCode")]
    public struct AsymmetricPairCode : IEquatable<AsymmetricPairCode>
    {
        /// <summary>
        ///     The first particle index
        /// </summary>
        [DataMember(Name = "Index0")]
        public int ParticleIndex0 { get; set; }

        /// <summary>
        ///     The second particle index
        /// </summary>
        [DataMember(Name = "Index1")]
        public int ParticleIndex1 { get; set; }

        /// <summary>
        ///     Create new pair code from two particle indices
        /// </summary>
        /// <param name="particleIndex0"></param>
        /// <param name="particleIndex1"></param>
        public AsymmetricPairCode(int particleIndex0, int particleIndex1)
            : this()
        {
            ParticleIndex0 = particleIndex0;
            ParticleIndex1 = particleIndex1;
        }

        /// <inheritdoc />
        public bool Equals(AsymmetricPairCode other)
        {
            return ParticleIndex1 == other.ParticleIndex1 && ParticleIndex0 == other.ParticleIndex0;
        }
    }
}