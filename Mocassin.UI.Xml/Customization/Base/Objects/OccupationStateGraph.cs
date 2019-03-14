using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Particles.OccupationState" /> data
    ///     objects
    /// </summary>
    [XmlRoot("OccupationState")]
    public class OccupationStateGraph : ProjectObjectGraph
    {
        /// <summary>
        ///     Get or set the list of particles that describe the occupation
        /// </summary>
        [XmlElement("Particle")]
        public List<ModelObjectReferenceGraph<Particle>> Particles { get; set; }

        /// <summary>
        ///     Get the object as an <see cref="IOccupationState" /> interface that is valid in the context of the passed
        ///     <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public IOccupationState ToInternal(IModelProject modelProject)
        {
            var particles = Particles.Select(x => modelProject.DataTracker.FindObjectByKey<IParticle>(x.Key)).ToList();
            var obj = new OccupationState
            {
                Particles = particles
            };

            return obj;
        }

        /// <summary>
        ///     Creates a new <see cref="OccupationStateGraph" /> from the passed <see cref="IOccupationState" /> interface
        /// </summary>
        /// <param name="occupationState"></param>
        /// <returns></returns>
        public static OccupationStateGraph Create(IOccupationState occupationState)
        {
            return Create(occupationState.AsEnumerable());
        }

        /// <summary>
        ///     Creates a new <see cref="OccupationStateGraph" /> from the passed sequence of <see cref="IParticle" /> model object
        ///     interfaces
        /// </summary>
        /// <param name="occupationParticles"></param>
        /// <returns></returns>
        public static OccupationStateGraph Create(IEnumerable<IParticle> occupationParticles)
        {
            var obj = new OccupationStateGraph
            {
                Particles = occupationParticles.Select(x => new ModelObjectReferenceGraph<Particle> {Key = x.Key}).ToList()
            };
            return obj;
        }
    }
}