using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Bitmask;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <inheritdoc cref="IParticleSet" />
    public class ParticleSet : ModelObject, IParticleSet
    {
        /// <inheritdoc />
        public int ParticleCount => Particles.Count;

        /// <summary>
        ///     The list of particles belonging to the particle set
        /// </summary>
        [UseTrackedData]
        public List<IParticle> Particles { get; set; }

        /// <inheritdoc />
        public override string ObjectName => "Particle Set";

        /// <inheritdoc />
        public IEnumerable<IParticle> GetParticles() => (Particles ?? new List<IParticle>()).AsEnumerable();


        /// <inheritdoc />
        public Bitmask64 AsBitmask()
        {
            ulong mask = 0;
            foreach (var particle in Particles)
            {
                if (particle.Index == 0) continue;
                if (!particle.IsDeprecated) mask += 1UL << particle.Index;
            }

            return new Bitmask64(mask);
        }

        /// <inheritdoc />
        public bool EqualsInModelProperties(IParticleSet other) => AsBitmask().Equals(other.AsBitmask());


        /// <inheritdoc />
        public bool IsEmpty() => ParticleCount == 0;

        /// <inheritdoc />
        public long AsLong()
        {
            return Particles.Where(x => !x.IsVoid).Aggregate(0L, (current, particle) => current | (1L << particle.Index));
        }

        /// <inheritdoc />
        public IEnumerator<IParticle> GetEnumerator() => Particles.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        ///     Creates new empty particle set that does not allow any occupants (Always has the index 0)
        /// </summary>
        /// <returns></returns>
        public static ParticleSet CreateEmpty() => new ParticleSet {Particles = new List<IParticle>(), Index = 0, Key = "ParticleSet.Void"};

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IParticleSet>(obj) is { } particleSet)) return null;

            Particles = particleSet.GetParticles().ToList();
            return this;
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
            comparer ??= Comparer<IParticle>.Create((a, b) => a.Index.CompareTo(b.Index));
            var setList = new SetList<IParticle>(comparer);
            setList.AddRange(particles);
            return new ParticleSet {Particles = setList.ToList()};
        }
    }
}