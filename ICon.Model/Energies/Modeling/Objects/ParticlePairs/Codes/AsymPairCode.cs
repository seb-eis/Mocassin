using System;
using System.Runtime.Serialization;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents a polar pair code of two particle indices that are used to identify a pair interaction. Order of indices is relevant in this case (Polar behavior)
    /// </summary>
    [Serializable]
    [DataContract(Name = "PolarPairCode")]
    public struct AsymPairCode : IEquatable<AsymPairCode>
    {
        /// <summary>
        /// The first particle index
        /// </summary>
        [DataMember(Name = "Index0")]
        public int ParticleIndex0 { get; set; }

        /// <summary>
        /// The second particle index
        /// </summary>
        [DataMember(Name = "Index1")]
        public int ParticleIndex1 { get; set; }

        /// <summary>
        /// Create new pair code from two particle indices
        /// </summary>
        /// <param name="particleIndex0"></param>
        /// <param name="particleIndex1"></param>
        public AsymPairCode(int particleIndex0, int particleIndex1) : this()
        {
            ParticleIndex0 = particleIndex0;
            ParticleIndex1 = particleIndex1;
        }

        /// <summary>
        /// Compares if the particle index values are the same (Returns true also if the index order is reversed)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AsymPairCode other)
        {
            return ParticleIndex1 == other.ParticleIndex1 && ParticleIndex0 == other.ParticleIndex0;
        }
    }
}
