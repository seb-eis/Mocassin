using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Particles.OccupationState" /> data
    ///     objects
    /// </summary>
    [XmlRoot]
    public class OccupationStateData : ProjectDataObject, IDuplicable<OccupationStateData>
    {
        private ObservableCollection<ModelObjectReference<Particle>> particles;

        /// <summary>
        ///     Get or set the list of particles that describe the occupation
        /// </summary>
        [XmlArray]
        public ObservableCollection<ModelObjectReference<Particle>> Particles
        {
            get => particles;
            set => SetProperty(ref particles, value);
        }

        /// <inheritdoc />
        public OccupationStateData Duplicate()
        {
            var copy = new OccupationStateData
            {
                Name = Name,
                particles = particles.Select(x => x.Duplicate()).ToObservableCollection()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();

        /// <summary>
        ///     Get the object as an <see cref="IOccupationState" /> interface that is valid in the context of the passed
        ///     <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public IOccupationState ToInternal(IModelProject modelProject)
        {
            var particleList = Particles.Select(x => modelProject.DataTracker.FindObject<IParticle>(x.Key)).ToList();
            var obj = new OccupationState
            {
                Particles = particleList
            };

            return obj;
        }

        /// <summary>
        ///     Creates a new <see cref="OccupationStateData" /> from the passed <see cref="IOccupationState" /> interface and
        ///     <see cref="ParticleModel.ParticleModelData" /> parent collection
        /// </summary>
        /// <param name="occupationState"></param>
        /// <param name="parents"></param>
        /// <returns></returns>
        public static OccupationStateData Create(IOccupationState occupationState, IList<ParticleData> parents) =>
            Create(occupationState.AsEnumerable(), parents);

        /// <summary>
        ///     Creates a new <see cref="OccupationStateData" /> from the passed sequence of <see cref="IParticle" /> model object
        ///     interfaces and <see cref="ParticleData" /> parent instances
        /// </summary>
        /// <param name="occupationParticles"></param>
        /// <param name="parents"></param>
        /// <returns></returns>
        public static OccupationStateData Create(IEnumerable<IParticle> occupationParticles, IList<ParticleData> parents)
        {
            if (occupationParticles == null) throw new ArgumentNullException(nameof(occupationParticles));
            if (parents == null) throw new ArgumentNullException(nameof(parents));

            var obj = new OccupationStateData
            {
                Particles = occupationParticles.Select(
                                                   x => new ModelObjectReference<Particle>
                                                       {Target = parents.Concat(ParticleData.VoidParticle.AsSingleton()).SingleOrDefault(y => y.Key == x.Key)})
                                               .ToObservableCollection()
            };
            return obj;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder(200);
            foreach (var particle in Particles) builder.Append($"[{particle?.Target?.Name ?? "Error@Ref"}]");

            return builder.ToString();
        }

        /// <summary>
        ///     Checks if the occupation describes by the other <see cref="OccupationStateData" /> is equal to this one
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasEqualState(OccupationStateData other) => other != null && (ReferenceEquals(this, other) || Particles.SequenceEqual(other.Particles));
    }
}