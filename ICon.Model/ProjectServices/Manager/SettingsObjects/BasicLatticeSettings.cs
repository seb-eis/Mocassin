using System;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Settings data object for the energy managing modul
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasicLatticeSettings
    {
        /// <summary>
        /// The maximum number of ULP steps that are allowed in numeric calculations using ULP comparisons
        /// </summary>
        [DataMember]
        public int MaxNumberOfParticles { get; set; }
    }
}
