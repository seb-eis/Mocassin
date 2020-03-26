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
    [XmlRoot]
    public class ParticleModelData : ModelManagerData
    {
        private ObservableCollection<ParticleData> particles;
        private ObservableCollection<ParticleSetData> particleSets;

        /// <summary>
        ///     The list of defined particles
        /// </summary>
        [XmlArray]
        public ObservableCollection<ParticleData> Particles
        {
            get => particles;
            set => SetProperty(ref particles, value);
        }

        /// <summary>
        ///     The list of defined particle sets
        /// </summary>
        [XmlArray]
        public ObservableCollection<ParticleSetData> ParticleSets
        {
            get => particleSets;
            set => SetProperty(ref particleSets, value);
        }

        /// <summary>
        ///     Creates new <see cref="ParticleModelData" /> with empty component lists
        /// </summary>
        public ParticleModelData()
        {
            Particles = new ObservableCollection<ParticleData>();
            ParticleSets = new ObservableCollection<ParticleSetData>();
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