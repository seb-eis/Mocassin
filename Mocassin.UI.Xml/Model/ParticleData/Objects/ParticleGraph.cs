﻿using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.Base;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.ParticleModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Particles.IParticle" /> model object creation
    /// </summary>
    [XmlRoot("Particle")]
    public class ParticleGraph : ModelObjectGraph
    {
        /// <summary>
        ///     Get the <see cref="ParticleGraph" /> equivalent to the model <see cref="Particle" /> that represents the void state
        /// </summary>
        public static readonly ParticleGraph VoidParticle = new ParticleGraph {Key = Particle.CreateEmpty().Key, Name = "Void"};

        private double charge;
        private string symbol;
        private bool isVacancy;

        /// <summary>
        ///     Get or set the charge value of the particle as a string
        /// </summary>
        [XmlAttribute("Charge")]
        [JsonProperty("Charge")]
        public double Charge
        {
            get => charge;
            set => SetProperty(ref charge, value);
        }

        /// <summary>
        ///     Get or set the symbol of the particle
        /// </summary>
        [XmlAttribute("Symbol")]
        [JsonProperty("Symbol")]
        public string Symbol
        {
            get => symbol;
            set => SetProperty(ref symbol, value);
        }

        /// <summary>
        ///     Get or set the boolean flag that enables the vacancy behavior of the particle
        /// </summary>
        [XmlAttribute("IsVacancy")]
        [JsonProperty("IsVacancy")]
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
                Key = particle.Key,
                Name = particle.Name,
                Symbol = particle.Symbol
            };
            return obj;
        }
    }
}