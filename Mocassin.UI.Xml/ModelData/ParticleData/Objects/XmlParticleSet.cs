using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.ParticleData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Particles.IParticleSet"/> model object creation
    /// </summary>
    [XmlRoot("ParticleSet")]
    public class XmlParticleSet : XmlModelObject
    {
        /// <summary>
        ///     List of particles contained in the set
        /// </summary>
        [XmlElement("Particle")]
        public List<XmlParticle> Particles { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetPreparedModelObject()
        {
            var obj = new ParticleSet
            {
                Particles = Particles.Select(x => x.GetInputObject()).Cast<IParticle>().ToList()
            };
            return obj;
        }
    }
}