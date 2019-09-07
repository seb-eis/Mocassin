﻿using System.Collections.Generic;
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
    [XmlRoot("ParticleSet")]
    public class ParticleSetGraph : ModelObjectGraph
    {
        private List<ModelObjectReferenceGraph<Particle>> particles;

        /// <summary>
        ///     List of particles contained in the set
        /// </summary>
        [XmlElement("Particle")]
        public List<ModelObjectReferenceGraph<Particle>> Particles
        {
            get => particles;
            set => SetProperty(ref particles, value);
        }

        /// <summary>
        ///     creates new <see cref="ParticleSetGraph" /> with empty component lists
        /// </summary>
        public ParticleSetGraph()
        {
            Particles = new List<ModelObjectReferenceGraph<Particle>>();
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