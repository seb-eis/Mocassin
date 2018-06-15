using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Settings data object for the element managing modul
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasicParticleSettings
    {
        /// <summary>
        /// The maximum number of allowed particles (Depens on used bitmask, default is 63)
        /// </summary>
        [DataMember(Name ="Limit")]
        public Int32 ParticleLimit { get; set; }

        /// <summary>
        /// The maximum absolut charge value that can be applied to a particle
        /// </summary>
        [DataMember(Name ="ChargeLimit")]
        public Double ChargeLimit { get; set; }

        /// <summary>
        /// The tolerance used to compare charge values for equality during validation
        /// </summary>
        [DataMember(Name ="ChargeTolerance")]
        public Double ChargeTolerance { get; set; }

        /// <summary>
        /// The maximum number of allowed particle masks
        /// </summary>
        [DataMember(Name ="MaxOccupants")]
        public Int32 ParticleSetLimit { get; set; }

        /// <summary>
        /// The regular expression for the particle symbol
        /// </summary>
        [DataMember(Name ="Symbol")]
        public String SymbolRegex { get; set; }

        /// <summary>
        /// The regular expression for the particle name
        /// </summary>
        [DataMember(Name ="Name")]
        public String NameRegex { get; set; }
    }
}
