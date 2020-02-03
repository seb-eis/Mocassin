using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.ParticleModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Particles.IParticleSet" /> model object creation
    /// </summary>
    [XmlRoot]
    public class ParticleSetData : ModelDataObject
    {
        private ObservableCollection<ModelObjectReference<Particle>> particles;

        /// <summary>
        ///     List of particles contained in the set
        /// </summary>
        [XmlArray]
        public ObservableCollection<ModelObjectReference<Particle>> Particles
        {
            get => particles;
            set => SetProperty(ref particles, value);
        }

        /// <summary>
        ///     creates new <see cref="ParticleSetData" /> with empty component lists
        /// </summary>
        public ParticleSetData()
        {
            Particles = new ObservableCollection<ModelObjectReference<Particle>>();
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new ParticleSet
            {
                Particles = Particles.Select(x => x.GetInputObject()).Cast<IParticle>().ToList()
            };
            return obj;
        }
    }
}