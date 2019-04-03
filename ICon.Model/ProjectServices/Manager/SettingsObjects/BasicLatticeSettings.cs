using System;
using System.Runtime.Serialization;

namespace Moccasin.Model.ProjectServices
{
    /// <summary>
    /// Settings data object for the lattice managing modul
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasicLatticeSettings
    {
        /// <summary>
        /// The maximum number of particles within the lattice
        /// </summary>
        [DataMember]
        public int MaxNumberOfParticles { get; set; }

        /// <summary>
        /// The tolerance with which the counter doping is applied
        /// </summary>
        [DataMember]
        public double DopingCompensationTolerance { get; set; }
    }
}
