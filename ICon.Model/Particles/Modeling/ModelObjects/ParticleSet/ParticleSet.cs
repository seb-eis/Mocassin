using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ICon.Mathematics.Bitmasks;
using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <inheritdoc cref="ICon.Model.Particles.IParticleSet" />
    [DataContract(Name = "ParticleSet")]
    public class ParticleSet : ModelObject, IParticleSet
    {
        /// <inheritdoc />
        [IgnoreDataMember]
        public int ParticleCount => Particles.Count;

        /// <summary>
        ///     The list of particles belonging to the particle set
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IParticle> Particles { get; set; }

        /// <inheritdoc />
        public IEnumerable<IParticle> GetParticles()
        {
            return (Particles ?? new List<IParticle>()).AsEnumerable();
        }


        /// <inheritdoc />
        public Bitmask64 GetEncoded()
        {
            ulong mask = 1;
            foreach (var particle in Particles)
            {
                if (particle.Index == 0) 
                    continue;

                if (!particle.IsDeprecated) 
                    mask = mask + (1UL << particle.Index);
            }

            return new Bitmask64(mask);
        }

        /// <summary>
        ///     Creates new empty particle set that does not allow any occupants (Always has the index 0)
        /// </summary>
        /// <returns></returns>
        public static ParticleSet CreateEmpty()
        {
            return new ParticleSet {Particles = new List<IParticle>(), Index = 0};
        }

        /// <inheritdoc />
        public bool EqualsInModelProperties(IParticleSet other)
        {
            return GetEncoded().Equals(other.GetEncoded());
        }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "'Particle Set'";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IParticleSet>(obj) is IParticleSet particleSet))
                return null;

            if (particleSet.IsEmpty())
                throw new ArgumentException("Interface consume function called on empty particle set interface");

            Particles = particleSet.GetParticles().ToList();
            return this;
        }


        /// <inheritdoc />
        public bool IsEmpty()
        {
            return ParticleCount == 0;
        }

        /// <inheritdoc />
        public IEnumerator<IParticle> GetEnumerator()
        {
            return Particles.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}