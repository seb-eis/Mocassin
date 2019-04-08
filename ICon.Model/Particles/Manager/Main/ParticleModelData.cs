using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Basic particle manager reference data that represents the base data required for load/save actions and caluclation
    ///     of all dependent data
    /// </summary>
    [DataContract(Name = "ParticleReferenceData")]
    public class ParticleModelData : ModelData<IParticleDataPort>
    {
        /// <summary>
        ///     The list of currently known particles that contains at least the 'Void-Particle' at index 0
        /// </summary>
        [DataMember(Name = "Particles")]
        [IndexedModelData(typeof(IParticle))]
        public List<Particle> Particles { get; set; }

        /// <summary>
        ///     The list of currently known particle sets
        /// </summary>
        [DataMember(Name = "ParticleSets")]
        [IndexedModelData(typeof(IParticleSet))]
        public List<ParticleSet> ParticleSets { get; set; }

        /// <summary>
        ///     Creates a new particle ref data object with a 'Void-Particle' and 'Void-Set' both at index 0
        /// </summary>
        /// <returns></returns>
        public static ParticleModelData CreateNew()
        {
            return CreateDefault<ParticleModelData>();
        }

        /// <inheritdoc />
        public override IParticleDataPort AsReadOnly()
        {
            return new ParticleDataManager(this);
        }

        /// <inheritdoc />
        public override void ResetToDefault()
        {
            ResetAllIndexedData();
            Particles.Add(Particle.CreateEmpty());
            ParticleSets.Add(ParticleSet.CreateEmpty());
        }
    }
}