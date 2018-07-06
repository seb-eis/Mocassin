using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Settings object for the simulation manager
    /// </summary>
    [DataContract(Name ="SimulationSettings")]
    public class BasicSimulationSettings
    {
        /// <summary>
        /// The value restriction setting for simulation temperature
        /// </summary>
        [DataMember]
        public ValueSetting<double> Temperature { get; set; }

        /// <summary>
        /// The value restriction setting for simulation doping (Settings this beyond [0,1] may crash application)
        /// </summary>
        [DataMember]
        public ValueSetting<double> Doping { get; set; }

        /// <summary>
        /// The value restriction setting for simulation electirc field (KMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<double> ElectricField { get; set; }

        /// <summary>
        /// The value restriction setting for simulation normalization (KMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<double> Normalization { get; set; }

        /// <summary>
        /// The value restriction setting for simulation monte carlo teps
        /// </summary>
        [DataMember]
        public ValueSetting<int> MonteCarloSteps { get; set; }

        /// <summary>
        /// The value restriction setting for simulation supercell sizes in a single direction
        /// </summary>
        [DataMember]
        public ValueSetting<int> UnitCellsPerDirection { get; set; }

        /// <summary>
        /// The value restriction setting for the simulation supercell position count
        /// </summary>
        [DataMember]
        public ValueSetting<int> SupercellPositionCount { get; set; }

        /// <summary>
        /// The value restriction setting for simulation break tolerance (MMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<double> BreakTolerance { get; set; }

        /// <summary>
        /// The value restriction setting for simulation break sample length (MMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<int> BreakSampleLength { get; set; }

        /// <summary>
        /// The value restriction setting for simulation break sampling interval (MMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<int> BreakSampleInterval { get; set; }

        /// <summary>
        /// The value restriction setting for simulation result sample length (MMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<int> ResultSampleLength { get; set; }

        /// <summary>
        /// The value restriction setting for simulation permutation count of a single simulation series
        /// </summary>
        [DataMember]
        public ValueSetting<int> SeriesPermutationCount { get; set; }

        /// <summary>
        /// The value restriction for simulation forced termination by runtime time span in [h]
        /// </summary>
        [DataMember]
        public ValueSetting<int> ForceTerminationTime { get; set; }

        /// <summary>
        /// The value restriction setting for simulation forced termination by success rate
        /// </summary>
        [DataMember]
        public ValueSetting<double> ForceTerminationSuccesRate { get; set; }

        /// <summary>
        /// The value restriction setting for simulation energy set count (File loaded energy sets)
        /// </summary>
        [DataMember]
        public ValueSetting<int> EnergySetCount { get; set; }

        /// <summary>
        /// The regex pattern for the restriction of simulation related string definitions
        /// </summary>
        [DataMember]
        public string NameStringPattern { get; set; }

        /// <summary>
        /// The regex pattern for the restriction of simulation related string definitions
        /// </summary>
        [DataMember]
        public string SeedStringPattern { get; set; }
    }
}
