using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.ParticleModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Particles.IParticleManager" />
    ///     system
    /// </summary>
    [XmlRoot("ParticleModel")]
    public class ParticleModelGraph : ModelManagerGraph
    {
        /// <summary>
        ///     The list of defines particles
        /// </summary>
        [XmlArray("Particles")]
        [XmlArrayItem("Particle")]
        public List<ParticleGraph> Particles { get; set; }

        /// <summary>
        ///     The list of defined particle sets
        /// </summary>
        [XmlArray("ParticleSets")]
        [XmlArrayItem("ParticleSet")]
        public List<ParticleSetGraph> ParticleSets { get; set; }

        /// <summary>
        ///     Creates new <see cref="ParticleModelGraph" /> with empty component lists
        /// </summary>
        public ParticleModelGraph()
        {
            Particles = new List<ParticleGraph>();
            ParticleSets = new List<ParticleSetGraph>();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield break;
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return Particles
                .Select(x => x.GetInputObject())
                .Concat(ParticleSets.Select(x => x.GetInputObject()));
        }
    }
}