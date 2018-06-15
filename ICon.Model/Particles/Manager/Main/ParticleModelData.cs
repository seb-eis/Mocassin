using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Basic particle manager reference data that represents the base data required for load/save actions and caluclation of all dependent data
    /// </summary>
    [Serializable]
    [DataContract(Name ="ParticleReferenceData")]
    public class ParticleModelData : ModelData<IParticleDataPort>
    {
        /// <summary>
        /// The list of currently known particles that contains at least the 'Void-Particle' at index 0
        /// </summary>
        [DataMember(Name ="Particles")]
        [IndexedModelData(typeof(IParticle))]
        public List<Particle> Particles { get; set; }

        /// <summary>
        /// The list of currently known particle sets
        /// </summary>
        [DataMember(Name ="ParticleSets")]
        [IndexedModelData(typeof(IParticleSet))]
        public List<ParticleSet> ParticleSets { get; set; }

        /// <summary>
        /// Creates a new particle ref data object with a 'Void-Particle' and 'Void-Set' both at index 0
        /// </summary>
        /// <returns></returns>
        public static ParticleModelData CreateNew()
        {
            return CreateDefault<ParticleModelData>();
        }

        /// <summary>
        /// Get an interface data port wrapper for read only access to this data object contents
        /// </summary>
        /// <returns></returns>
        public override IParticleDataPort AsReadOnly()
        {
            return new ParticleDataManager(this);
        }

        /// <summary>
        /// Resets the data object to default conditions with only 'Void-Particle' and 'Void-Set' at index 0
        /// </summary>
        public override void ResetToDefault()
        {
            Particles = new List<Particle>() { Particle.CreateEmpty() };
            ParticleSets = new List<ParticleSet>() { ParticleSet.CreateEmpty() };
        }
    }
}
