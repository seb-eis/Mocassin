using System.Collections.Generic;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.ParticleModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Particles.IParticle" /> model object creation
    /// </summary>
    [XmlRoot("Particle")]
    public class ParticleGraph : ModelObjectGraph
    {
        /// <summary>
        ///     Get or set the charge value of the particle as a string
        /// </summary>
        [XmlAttribute("Charge")]
        public double Charge { get; set; }

        /// <summary>
        ///     Get or set the name of the particle
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        ///     Get or set the symbol of the particle
        /// </summary>
        [XmlAttribute("Symbol")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Get or set the boolean flag that enables the vacancy behavior of the particle
        /// </summary>
        [XmlAttribute("IsVacancy")]
        public bool IsVacancy { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new Particle
            {
                Charge = Charge,
                Name = Name,
                Symbol = Symbol,
                IsVacancy = IsVacancy
            };
            return obj;
        }

        /// <summary>
        ///     Creates a new <see cref="ParticleGraph" /> by pulling all required data from the passed <see cref="IParticle" />
        ///     model object interface
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        public static ParticleGraph Create(IParticle particle)
        {
            var obj = new ParticleGraph
            {
                Charge = particle.Charge,
                IsVacancy = particle.IsVacancy,
                KeyReference = particle.Key,
                Name = particle.Name,
                Symbol = particle.Symbol
            };
            return obj;
        }
    }
}