using System;
using System.Collections.Generic;
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
    [XmlRoot("OccupationState")]
    public class OccupationStateGraph : ProjectObjectGraph
    {
        private List<ModelObjectReferenceGraph<Particle>> particles;

        /// <summary>
        ///     Get or set the list of particles that describe the occupation
        /// </summary>
        [XmlElement("Particle")]
        public List<ModelObjectReferenceGraph<Particle>> Particles
        {
            get => particles;
            set => SetProperty(ref particles, value);
        }

        /// <summary>
        ///     Get the object as an <see cref="IOccupationState" /> interface that is valid in the context of the passed
        ///     <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public IOccupationState ToInternal(IModelProject modelProject)
        {
            var particleList = Particles.Select(x => modelProject.DataTracker.FindObjectByKey<IParticle>(x.Key)).ToList();
            var obj = new OccupationState
            {
                Particles = particleList
            };

            return obj;
        }

        /// <summary>
        ///     Creates a new <see cref="OccupationStateGraph" /> from the passed <see cref="IOccupationState" /> interface and <see cref="ParticleModelGraph"/> parent collection
        /// </summary>
        /// <param name="occupationState"></param>
        /// <param name="parents"></param>
        /// <returns></returns>
        public static OccupationStateGraph Create(IOccupationState occupationState, IList<ParticleGraph> parents)
        {
            return Create(occupationState.AsEnumerable(), parents);
        }

        /// <summary>
        ///     Creates a new <see cref="OccupationStateGraph" /> from the passed sequence of <see cref="IParticle" /> model object
        ///     interfaces and <see cref="ParticleGraph"/> parent instances
        /// </summary>
        /// <param name="occupationParticles"></param>
        /// <param name="parents"></param>
        /// <returns></returns>
        public static OccupationStateGraph Create(IEnumerable<IParticle> occupationParticles, IList<ParticleGraph> parents)
        {
            if (occupationParticles == null) throw new ArgumentNullException(nameof(occupationParticles));
            if (parents == null) throw new ArgumentNullException(nameof(parents));

            var obj = new OccupationStateGraph
            {
                Particles = occupationParticles.Select(
                    x => new ModelObjectReferenceGraph<Particle> {TargetGraph = parents.Concat(ParticleGraph.VoidParticle.AsSingleton()).SingleOrDefault(y => y.Key == x.Key)})
                    .ToList()
            };
            return obj;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder(200);
            foreach (var particle in Particles)
            {
                builder.Append($"[{particle?.TargetGraph?.Name ?? "Error@Ref"}]");
            }

            return builder.ToString();
        }
    }
}