using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    ///     Settings object for the simulation manager
    /// </summary>
    [DataContract(Name = "SimulationSettings")]
    public class BasicSimulationSettings
    {
        /// <summary>
        ///     The value restriction setting for simulation temperature
        /// </summary>
        [DataMember]
        public ValueSetting<double> Temperature { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation doping (Settings this beyond [0,1] may crash application)
        /// </summary>
        [DataMember]
        public ValueSetting<double> Doping { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation electric field in [V/m] (KMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<double> ElectricField { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation normalization (KMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<double> Normalization { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation monte carlo steps
        /// </summary>
        [DataMember]
        public ValueSetting<int> MonteCarloSteps { get; set; }

        /// <summary>
        ///     The value restriction for the number of calls to the write functions
        /// </summary>
        [DataMember]
        public ValueSetting<int> WriteCallCount { get; set; }

        /// <summary>
        ///     The value restriction for the job count of a single simulation
        /// </summary>
        [DataMember]
        public ValueSetting<int> JobCount { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation super-cell sizes in a single direction
        /// </summary>
        [DataMember]
        public ValueSetting<int> UnitCellsPerDirection { get; set; }

        /// <summary>
        ///     The value restriction setting for the simulation super-cell position count
        /// </summary>
        [DataMember]
        public ValueSetting<int> SupercellPositionCount { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation break tolerance (MMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<double> BreakTolerance { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation break sample length (MMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<int> BreakSampleLength { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation break sampling interval (MMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<int> BreakSampleInterval { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation result sample length (MMC only)
        /// </summary>
        [DataMember]
        public ValueSetting<int> ResultSampleLength { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation permutation count of a single simulation series
        /// </summary>
        [DataMember]
        public ValueSetting<long> SeriesPermutation { get; set; }

        /// <summary>
        ///     The value restriction setting for a single value series permutation count
        /// </summary>
        [DataMember]
        public ValueSetting<int> SingleValuePermutation { get; set; }

        /// <summary>
        ///     The value restriction for simulation forced termination by runtime time span in [h]
        /// </summary>
        [DataMember]
        public ValueSetting<int> ForceTerminationTime { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation forced termination by success rate
        /// </summary>
        [DataMember]
        public ValueSetting<double> TerminationSuccessRate { get; set; }

        /// <summary>
        ///     The value restriction setting for simulation energy set count (File loaded energy sets)
        /// </summary>
        [DataMember]
        public ValueSetting<int> EnergySetCount { get; set; }

        /// <summary>
        ///     The value restriction setting for the number of transitions per simulation
        /// </summary>
        [DataMember]
        public ValueSetting<int> TransitionCount { get; set; }

        /// <summary>
        ///     The regex pattern for the restriction of simulation related string definitions
        /// </summary>
        [DataMember]
        public StringSetting Naming { get; set; }

        /// <summary>
        ///     The regex pattern for the restriction of simulation related string definitions
        /// </summary>
        [DataMember]
        public StringSetting Seeding { get; set; }
    }
}