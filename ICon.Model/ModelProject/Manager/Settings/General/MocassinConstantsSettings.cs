using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Basic settings that contain the values for the nature constants
    /// </summary>
    [DataContract, XmlRoot("MocassinConstantsSettings")]
    public class MocassinConstantsSettings
    {
        /// <summary>
        ///     The boltzmann constant in SI units
        /// </summary>
        [DataMember, XmlAttribute("BoltzmannConstantSI")]
        public double BoltzmannConstantSi { get; set; } = 1.380649e-23;

        /// <summary>
        ///     Universal gas constant in SI units
        /// </summary>
        [DataMember, XmlAttribute("UniversalGasConstantSI")]
        public double UniversalGasConstantSi { get; set; } = 8.31446261815324;

        /// <summary>
        ///     The electric permittivity constant in Si units
        /// </summary>
        [DataMember, XmlAttribute("VacuumPermittivitySI")]
        public double VacuumPermittivitySi { get; set; } = 8.854187812813e-12;

        /// <summary>
        ///     The elemental charge constant in SI units
        /// </summary>
        [DataMember, XmlAttribute("ElementalChargeSI")]
        public double ElementalChargeSi { get; set; } = 1.602176634e-19;
    }
}