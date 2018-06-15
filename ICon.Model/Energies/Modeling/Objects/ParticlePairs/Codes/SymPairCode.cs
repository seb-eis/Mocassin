using System;
using System.Runtime.Serialization;
using ICon.Model.Particles;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents a pair code of two particle indices that are used to identify a pair interaction. Order of indices is not relevant for equality (Unpolar behavior)
    /// </summary>
    [Serializable]
    [DataContract(Name ="PolarPairCode")]
    public struct SymPairCode : IEquatable<SymPairCode>, IEquatable<PairCode>
    {
        /// <summary>
        /// The first particle index
        /// </summary>
        [DataMember(Name ="Index0")]
        public int ParticleIndex0 { get; set; }

        /// <summary>
        /// The second particle index
        /// </summary>
        [DataMember(Name ="Index1")]
        public int ParticleIndex1 { get; set; }

        /// <summary>
        /// Create new pair code from two particle indices
        /// </summary>
        /// <param name="particleIndex0"></param>
        /// <param name="particleIndex1"></param>
        public SymPairCode(int particleIndex0, int particleIndex1) : this()
        {
            ParticleIndex0 = particleIndex0;
            ParticleIndex1 = particleIndex1;
        }

        /// <summary>
        /// Compares if the particle index values are the same (Returns true also if the index order is reversed)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SymPairCode other)
        {
            if (ParticleIndex0 != other.ParticleIndex0)
            {
                return ParticleIndex1 == other.ParticleIndex0 && ParticleIndex0 == other.ParticleIndex1;
            }
            return ParticleIndex1 == other.ParticleIndex1;
        }

        /// <summary>
        /// Compares to the general pair code object
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PairCode other)
        {
            return Equals(new SymPairCode(other.Index0, other.Index1));
        }

        /// <summary>
        /// Gets a string representation of the unpolar pair code
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"'Unpolar Pair Code' ({ParticleIndex0}, {ParticleIndex1})";
        }
    }
}
