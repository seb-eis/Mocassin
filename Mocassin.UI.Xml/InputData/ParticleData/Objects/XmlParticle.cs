using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.ParticleData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Particles.IParticle" /> model object creation
    /// </summary>
    [XmlRoot("Particle")]
    public class XmlParticle : XmlModelObject
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
        protected override ModelObject GetPreparedModelObject()
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
    }
}