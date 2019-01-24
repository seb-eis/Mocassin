﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.ParticleData
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Particles.IParticleManager" />
    ///     system
    /// </summary>
    [XmlRoot("ParticleModel")]
    public class XmlParticleData : XmlProjectManagerData
    {
        /// <summary>
        ///     The list of defines particles
        /// </summary>
        [XmlArray("Particles")]
        [XmlArrayItem("Particle")]
        public List<XmlParticle> Particles { get; set; }

        /// <summary>
        ///     The list of defined particle sets
        /// </summary>
        [XmlArray("ParticleSets")]
        [XmlArrayItem("ParticleSet")]
        public List<XmlParticleSet> ParticleSets { get; set; }

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