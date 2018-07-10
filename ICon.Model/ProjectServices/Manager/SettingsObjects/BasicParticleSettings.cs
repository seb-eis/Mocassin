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
        /// The value restriction setting for particle charges
        /// </summary>
        [DataMember]
        public ValueSetting<double> ParticleCharge { get; set; }

        /// <summary>
        /// The value restriction setting for the particle count
        /// </summary>
        [DataMember]
        public ValueSetting<int> ParticleCount { get; set; }

        /// <summary>
        /// The value restriction setting for the particle set count
        /// </summary>
        [DataMember]
        public ValueSetting<int> ParticleSetCount { get; set; }

        /// <summary>
        /// The regular expression for the particle symbol
        /// </summary>
        [DataMember]
        public string SymbolStringPattern { get; set; }

        /// <summary>
        /// The regular expression for the particle name
        /// </summary>
        [DataMember]
        public string NameStringPattern { get; set; }
    }
}
