using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Basic settings that contain the values for the nature constants
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasicConstantsSettings
    {
        /// <summary>
        /// The boltzmann constant in SI units
        /// </summary>
        [DataMember(Name ="BoltzmannSI")]
        public Double BoltzmannConstSI { get; set; }

        /// <summary>
        /// Universal gas constant in SI units
        /// </summary>
        [DataMember(Name ="GasConstSI")]
        public Double GasConstSI { get; set; }

        /// <summary>
        /// The electric permitivity constant in SI units
        /// </summary>
        [DataMember(Name ="ElectricPermSI")]
        public Double ElectricPermittivitySI { get; set; }

        /// <summary>
        /// The elemental charge constant in SI units
        /// </summary>
        [DataMember(Name ="ElemChargeSI")]
        public Double ElementalChargeSI { get; set; }
    }
}
