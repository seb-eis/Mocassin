using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Bitmasks;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <inheritdoc cref="IParticleSet" />
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
        [UseTrackedReferences]
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
            return new ParticleSet {Particles = new List<IParticle>(), Index = 0, Key = "ParticleSet.Void"};
        }

        /// <inheritdoc />
        public bool EqualsInModelProperties(IParticleSet other)
        {
            return GetEncoded().Equals(other.GetEncoded());
        }

		/// <inheritdoc />
		public override string ObjectName => "Particle Set";

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
        public long AsLong()
        {
            return Particles.Where(x => !x.IsEmpty).Aggregate(0L, (current, particle) => current | 1L << particle.Index);
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

        /// <summary>
        ///     Takes a sequence of particles objects and creates a new sorted particle set that contains only the unique ones
        ///     using the provided comparer or a default particle comparer
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IParticleSet ToSortedSet(IEnumerable<IParticle> particles, IComparer<IParticle> comparer = null)
        {
            comparer = comparer ?? Comparer<IParticle>.Create((a, b) => a.Index.CompareTo(b.Index));
            var setList = new SetList<IParticle>(comparer);
            setList.AddMany(particles);
            return new ParticleSet {Particles = setList.ToList()};
        }
    }
}