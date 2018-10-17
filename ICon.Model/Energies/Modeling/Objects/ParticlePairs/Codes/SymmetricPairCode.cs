using System;
using System.Runtime.Serialization;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a pair code of two particle indices that are used to identify a pair interaction. Order of indices is
    ///     not relevant for equality (Symmetric behavior)
    /// </summary>
    [DataContract(Name = "SymmetricPairCode")]
    public struct SymmetricPairCode : IEquatable<SymmetricPairCode>, IEquatable<PairCode>
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
        public SymmetricPairCode(int particleIndex0, int particleIndex1)
            : this()
        {
            ParticleIndex0 = particleIndex0;
            ParticleIndex1 = particleIndex1;
        }

        /// <inheritdoc />
        public bool Equals(SymmetricPairCode other)
        {
            if (ParticleIndex0 != other.ParticleIndex0)
                return ParticleIndex1 == other.ParticleIndex0 && ParticleIndex0 == other.ParticleIndex1;
            return ParticleIndex1 == other.ParticleIndex1;
        }

        /// <inheritdoc />
        public bool Equals(PairCode other)
        {
            return Equals(new SymmetricPairCode(other.Index0, other.Index1));
        }

        /// <summary>
        ///     Gets a string representation of the symmetric pair code
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"'Symmetric Pair Code' ({ParticleIndex0}, {ParticleIndex1})";
        }
    }
}