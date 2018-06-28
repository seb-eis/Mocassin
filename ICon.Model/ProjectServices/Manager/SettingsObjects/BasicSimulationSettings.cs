using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Settings object for the simulation manager settings
    /// </summary>
    [DataContract(Name ="SimulationSettings")]
    public class BasicSimulationSettings
    {
        /// <summary>
        /// Defines the maximum number of simulations that a single simulation set can contain
        /// </summary>
        [DataMember]
        public int MaxSimulationsPerSet { get; set; }

        /// <summary>
        /// Defines the lower temperature limit for simulations (Typically above 0)
        /// </summary>
        [DataMember]
        public double MinTemperature { get; set; }

        /// <summary>
        /// Defines the upper temperature limit for simulations
        /// </summary>
        [DataMember]
        public double MaxTemperature { get; set; }

        /// <summary>
        /// Defines the lower limit for value stepping factors
        /// </summary>
        [DataMember]
        public double MinValueSteppingFactor { get; set; }

        /// <summary>
        /// Defines the upper limit for value stepping factors (Values beyong 1 make little sense)
        /// </summary>
        [DataMember]
        public double MaxValueSteppingFactor { get; set; }
    }
}
