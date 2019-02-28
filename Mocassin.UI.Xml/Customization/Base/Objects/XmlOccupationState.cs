using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Particles.OccupationState" /> data
    ///     objects
    /// </summary>
    [XmlRoot("OccupationState")]
    public class XmlOccupationState
    {
        /// <summary>
        ///     Get or set the list of particles that describe the occupation
        /// </summary>
        [XmlElement("Particle")]
        public List<XmlParticle> Particles { get; set; }

        /// <summary>
        ///     Get the object as an <see cref="IOccupationState" /> interface that is valid in the context of the passed
        ///     <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public IOccupationState ToInternal(IModelProject modelProject)
        {
            var particles = Particles.Select(x => modelProject.DataTracker.FindObjectByKey<IParticle>(x.GetKey())).ToList();
            var obj = new OccupationState
            {
                Particles = particles
            };

            return obj;
        }

        /// <summary>
        ///     Creates a new <see cref="XmlOccupationState" /> from the passed <see cref="IOccupationState" /> interface
        /// </summary>
        /// <param name="occupationState"></param>
        /// <returns></returns>
        public static XmlOccupationState Create(IOccupationState occupationState)
        {
            return Create(occupationState.AsEnumerable());
        }

        /// <summary>
        /// Creates a new <see cref="XmlOccupationState"/> from the passed sequence of <see cref="IParticle"/> model object interfaces
        /// </summary>
        /// <param name="occupationParticles"></param>
        /// <returns></returns>
        public static XmlOccupationState Create(IEnumerable<IParticle> occupationParticles)
        {
            var obj = new XmlOccupationState
            {
                Particles = occupationParticles.Select(XmlParticle.Create).ToList()
            };
            return obj;
        }
    }
}