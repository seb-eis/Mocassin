using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<ParticleGraph> particles;
        private ObservableCollection<ParticleSetGraph> particleSets;

        /// <summary>
        ///     The list of defines particles
        /// </summary>
        [XmlArray("Particles")]
        [XmlArrayItem("Particle")]
        public ObservableCollection<ParticleGraph> Particles
        {
            get => particles;
            set => SetProperty(ref particles, value);
        }

        /// <summary>
        ///     The list of defined particle sets
        /// </summary>
        [XmlArray("ParticleSets")]
        [XmlArrayItem("ParticleSet")]
        public ObservableCollection<ParticleSetGraph> ParticleSets
        {
            get => particleSets;
            set => SetProperty(ref particleSets, value);
        }

        /// <summary>
        ///     Creates new <see cref="ParticleModelGraph" /> with empty component lists
        /// </summary>
        public ParticleModelGraph()
        {
            Particles = new ObservableCollection<ParticleGraph>();
            ParticleSets = new ObservableCollection<ParticleSetGraph>();
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