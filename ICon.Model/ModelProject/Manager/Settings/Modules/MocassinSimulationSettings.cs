using System.Runtime.Serialization;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Settings object for the simulation manager
    /// </summary>
    [DataContract]
    [ModuleSettings(typeof(ISimulationManager))]
    public class MocassinSimulationSettings : MocassinModuleSettings
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

        /// <inheritdoc />
        public override void InitAsDefault()
        {
            BreakSampleInterval = new ValueSetting<int>("Break Sample Interval", 1, 1, 100, 1000);
            BreakSampleLength = new ValueSetting<int>("Break Sample Length", 1, 100, 10000, 100000);
            ResultSampleLength = new ValueSetting<int>("Result Sample Length", 1, 1000, 10000, 1000000);
            BreakTolerance = new ValueSetting<double>("Break Tolerance", 0, 0, 0.1, 1);
            Doping = new ValueSetting<double>("Doping Concentration", 0, 0, 1, 1);
            ElectricField = new ValueSetting<double>("Electric Field", 0, 1e4, 1e9, 1e10);
            MonteCarloSteps = new ValueSetting<int>("Steps per Particle", 1, 1, 1000, 1000000);
            JobCount = new ValueSetting<int>("Jobs per Simulation", 1, 5, 100, 1000);
            WriteCallCount = new ValueSetting<int>("Write Calls per Simulation", 0, 5, 100, 1000);
            Temperature = new ValueSetting<double>("Temperature", 0.1, 100, 5000, 10000);
            Normalization = new ValueSetting<double>("Normalization", 0, 0, 0.1, 1.0);
            SeriesPermutation = new ValueSetting<long>("Series Simulation Count", 0, 0, 1000, 5000);
            SingleValuePermutation = new ValueSetting<int>("Value Series Count", 0, 0, 100, 5000);
            SupercellPositionCount = new ValueSetting<int>("Super-cell Position Count", 1, 100, 100000, 500000);
            UnitCellsPerDirection = new ValueSetting<int>("Cells per Direction", 1, 1, 100, 256);
            ForceTerminationTime = new ValueSetting<int>("Forced Termination Time", 0, 1, 48, 120);
            TerminationSuccessRate = new ValueSetting<double>("Lower Termination Success Rate", 1, 10, int.MaxValue, int.MaxValue);
            EnergySetCount = new ValueSetting<int>("Energy Set Count", 0, 0, 10, 100);
            TransitionCount = new ValueSetting<int>("Transitions per Simulation", 0, 0, 10, 100);
            Naming = new StringSetting("Name String", "^[a-zA-Z0-9\\+\\-\\(\\)]{2,100}$", false);
            Seeding = new StringSetting("Seed String", "^[a-zA-Z0-9\\+\\-\\(\\)]{0,100}$", true);
        }
    }
}