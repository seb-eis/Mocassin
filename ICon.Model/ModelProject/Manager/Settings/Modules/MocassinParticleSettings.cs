using System.Runtime.Serialization;
using Mocassin.Model.Particles;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Settings data object for the element managing module
    /// </summary>
    [DataContract, ModuleSettings(typeof(IParticleManager))]
    public class MocassinParticleSettings : MocassinModuleSettings
    {
        /// <summary>
        ///     The value restriction setting for particle charges
        /// </summary>
        [DataMember]
        public ValueSetting<double> ParticleCharge { get; set; }

        /// <summary>
        ///     The value restriction setting for the particle count
        /// </summary>
        [DataMember]
        public ValueSetting<int> ParticleCount { get; set; }

        /// <summary>
        ///     The value restriction setting for the particle set count
        /// </summary>
        [DataMember]
        public ValueSetting<int> ParticleSetCount { get; set; }

        /// <summary>
        ///     The regular expression for the particle symbol
        /// </summary>
        [DataMember]
        public StringSetting SymbolNaming { get; set; }

        /// <inheritdoc />
        public override void InitAsDefault()
        {
            ParticleCharge = new ValueSetting<double>("Particle Charge", -1000, 1000);
            ParticleCount = new ValueSetting<int>("Particle Count", 0, 64);
            ParticleSetCount = new ValueSetting<int>("Particle Set Count", 0, 100);
            SymbolNaming = new StringSetting("Particle Symbol", "^[A-Z]{1,1}[a-zA-Z0-9\\+\\-\\(\\)]{0,10}$", false);
        }
    }
}