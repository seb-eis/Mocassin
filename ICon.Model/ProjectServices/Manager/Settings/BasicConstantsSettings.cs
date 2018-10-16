using System;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    ///     Basic settings that contain the values for the nature constants
    /// </summary>
    [DataContract]
    public class BasicConstantsSettings
    {
        /// <summary>
        ///     The boltzmann constant in SI units
        /// </summary>
        [DataMember]
        public double BoltzmannConstantSi { get; set; }

        /// <summary>
        ///     Universal gas constant in SI units
        /// </summary>
        [DataMember]
        public double UniversalGasConstantSi { get; set; }

        /// <summary>
        ///     The electric permittivity constant in Si units
        /// </summary>
        [DataMember]
        public double VacuumPermittivitySi { get; set; }

        /// <summary>
        ///     The elemental charge constant in SI units
        /// </summary>
        [DataMember]
        public double ElementalChargeSi { get; set; }
    }
}