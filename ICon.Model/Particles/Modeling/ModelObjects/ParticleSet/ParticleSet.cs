using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ICon.Mathematics.Extensions;
using ICon.Mathematics.Bitmasks;
using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Basic particle set that uses a 64 bit bitmask to encode allowed and not allowed states of the particles
    /// </summary>
    [DataContract(Name = "ParticleSet")]
    public class ParticleSet : ModelObject, IParticleSet
    {
        /// <summary>
        /// The number of particles in the set
        /// </summary>
        [IgnoreDataMember]
        public int ParticleCount => Particles.Count;

        /// <summary>
        /// The list of particles belonging to the particle set
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public List<IParticle> Particles { get; set; }
        
        /// <summary>
        /// Get the sequence of particles for the particle set
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IParticle> GetParticles()
        {
            return (Particles ?? new List<IParticle>()).AsEnumerable();
        }


        /// <summary>
        /// Encodes the interal set of particles into a bitmask (Contains only particles that are not deprecated, empty particle with index 0 is always allowed)
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public Bitmask64 GetEncoded()
        {
            ulong mask = 1;
            foreach (var particle in Particles)
            {
                if (particle.Index == 0)
                {
                    continue;
                }
                if (!particle.IsDeprecated)
                {
                    mask = mask + (1UL << (particle.Index));
                }
            }
            return new Bitmask64(mask);
        }

        /// <summary>
        /// Creates new empty particle set that does not allow any occupants (Always has the index 0)
        /// </summary>
        /// <returns></returns>
        public static ParticleSet CreateEmpty()
        {
            return new ParticleSet() { Particles = new List<IParticle>(), Index = 0 };
        }

        /// <summary>
        /// Checks if the particle set have the same bitmask representation
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualsInModelProperties(IParticleSet other)
        {
            return GetEncoded().Equals(other.GetEncoded());
        }

        /// <summary>
        /// Get a string that represents the name of the object type Particle Set
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Particle Set'";
        }

        /// <summary>
        /// Creates new particle set from model object interface (Returns null for type mismatch or deprecated model object)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IParticleSet>(obj) is var particleSet)
            {
                if (particleSet.IsEmpty())
                {
                    throw new ArgumentException("Interface consume function called on empty particle set interface");
                }
                Particles = particleSet.GetParticles().ToList();
                return this;
            }
            return null;
        }

        /// <summary>
        /// Checks if the particle count is zero
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return ParticleCount == 0;
        }
    }
}
