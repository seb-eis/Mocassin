using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.ParticleModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Particles.IParticle" /> model object creation
    /// </summary>
    [XmlRoot]
    public class ParticleData : ModelDataObject
    {
        /// <summary>
        ///     Get the <see cref="ParticleData" /> equivalent to the model <see cref="Particle" /> that represents the void state
        /// </summary>
        public static readonly ParticleData VoidParticle = new ParticleData {Key = Particle.CreateVoid().Key, Name = "Void"};

        private double charge;
        private bool isVacancy;
        private string symbol;

        /// <summary>
        ///     Get or set the charge value of the particle as a string
        /// </summary>
        [XmlAttribute]
        public double Charge
        {
            get => charge;
            set => SetProperty(ref charge, value);
        }

        /// <summary>
        ///     Get or set the symbol of the particle
        /// </summary>
        [XmlAttribute]
        public string Symbol
        {
            get => symbol;
            set => SetProperty(ref symbol, value);
        }

        /// <summary>
        ///     Get or set the boolean flag that enables the vacancy behavior of the particle
        /// </summary>
        [XmlAttribute]
        public bool IsVacancy
        {
            get => isVacancy;
            set => SetProperty(ref isVacancy, value);
        }

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
        ///     Creates a new <see cref="ParticleData" /> by pulling all required data from the passed <see cref="IParticle" />
        ///     model object interface
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        public static ParticleData Create(IParticle particle)
        {
            var obj = new ParticleData
            {
                Charge = particle.Charge,
                IsVacancy = particle.IsVacancy,
                Key = particle.Key,
                Name = particle.Name,
                Symbol = particle.Symbol
            };
            return obj;
        }
    }
}