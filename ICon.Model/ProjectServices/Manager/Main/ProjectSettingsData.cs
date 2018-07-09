using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Data class for the model settings that stores general settings required throughout the entire model process (e.g. floating point tolerances)
    /// </summary>
    [DataContract]
    public class ProjectSettingsData
    {
        /// <summary>
        /// The numeric settings for common calculations and comparisons
        /// </summary>
        [DataMember]
        public BasicNumericSettings CommonNumericSettings { get; set; }

        /// <summary>
        /// The numeric settings for geometry calculations and comparisons
        /// </summary>
        [DataMember]
        public BasicNumericSettings GeometryNumericSettings { get; set; }

        /// <summary>
        /// The basic concurrency settings for timeout exceptions during parallel access to the model library
        /// </summary>
        [DataMember]
        public BasicConcurrencySettings ConcurrencySettings { get; set; }

        /// <summary>
        /// The basic constant settings that contain nature constants
        /// </summary>
        [DataMember]
        public BasicConstantsSettings ConstantsSettings { get; set; }

        /// <summary>
        /// The basic symmetry settings for space groups and crystal systems
        /// </summary>
        [DataMember]
        public BasicSymmetrySettings SymmetrySettings { get; set; }

        /// <summary>
        /// The settings for particle related input into the affiliated manager
        /// </summary>
        [DataMember]
        public BasicParticleSettings ParticleSettings { get; set; }


        /// <summary>
        /// The settings for structure related inputs into the affiliated manager
        /// </summary>
        [DataMember]
        public BasicStructureSettings StructureSettings { get; set; }

        /// <summary>
        /// The settings for the transition related input
        /// </summary>
        [DataMember]
        public BasicTransitionSettings TransitionSettings { get; set; }

        /// <summary>
        /// The settings for the energy related input
        /// </summary>
        [DataMember]
        public BasicEnergySettings EnergySettings { get; set; }

        /// <summary>
        /// The settings for the lattice related input
        /// </summary>
        [DataMember]
        public BasicLatticeSettings LatticeSettings { get; set; }

        /// <summary>
        /// The settings for the simulation related input
        /// </summary>
        [DataMember]
        public BasicSimulationSettings SimulationSettings { get; set; }

        /// <summary>
        /// Creates a new default project servies data object
        /// </summary>
        /// <returns></returns>
        public static ProjectSettingsData CreateDefault()
        {
            return new ProjectSettingsData
            {
                CommonNumericSettings = new BasicNumericSettings()
                {
                    FactorValue = 1.0e-6,
                    RangeValue = 1.0e-10,
                    UlpValue = 10
                },
                GeometryNumericSettings = new BasicNumericSettings()
                {
                    FactorValue = 1.0e-2,
                    RangeValue = 1.0e-3,
                    UlpValue = 50
                },
                ConcurrencySettings = new BasicConcurrencySettings()
                {
                    AttemptInterval = TimeSpan.FromMilliseconds(100),
                    MaxAttempts = 20
                },
                ConstantsSettings = new BasicConstantsSettings()
                {
                    BoltzmannConstantSi = 1.38064852e-23,
                    UniversalGasConstantSi = 8.3144598,
                    VacuumPermittivitySi = 8.85418781762e-12,
                    ElementalChargeSi = 1.6021766208e-19
                },
                ParticleSettings = new BasicParticleSettings()
                {
                    ParticleCharge = new ValueSetting<double>("Particle Charge", -1000, 1000),
                    ParticleCount = new ValueSetting<int>("Particle Count", 0, 64),
                    ParticleSetCount = new ValueSetting<int>("Particle Set Count", 0, 100),
                    NameStringPattern = "^[a-zA-Z]{1,1}[a-zA-Z0-9\\+\\-\\(\\)]{1,100}$",
                    SymbolStringPattern = "^[A-Z]{1,1}[a-zA-Z0-9\\+\\-\\(\\)]{0,4}$"
                },
                StructureSettings = new BasicStructureSettings()
                {
                    BasePositionCount = new ValueSetting<int>("Base Position Count", 0, 1000),
                    TotalPositionCount = new ValueSetting<int>("Total Position Count", 0, 10000),
                    CellParameter = new ValueSetting<double>("Cell Parameter Length", 0.1, 1000),
                    NameStringPattern = "^[a-zA-Z0-9\\+\\-\\(\\)]{2,100}$"
                },
                SymmetrySettings = new BasicSymmetrySettings()
                {
                    SpaceGroupDbPath = $"{Environment.GetEnvironmentVariable("USERPROFILE")}/source/repos/ICon.Program/ICon.Framework.Symmetry/SpaceGroups/SpaceGroups.db",
                    VectorTolerance = 1.0e-6,
                    ParameterTolerance = 1.0e-6
                },
                TransitionSettings = new BasicTransitionSettings()
                {
                    TransitionCount = new ValueSetting<int>("Transition Count", 0, 100),
                    TransitionLength = new ValueSetting<int>("Transition Length", 2, 8),
                    TransitionStringPattern = "^[a-zA-Z0-9\\+\\-\\(\\)]{2,100}$",
                    FilterUnrecognisedRuleTypes = true
                },
                EnergySettings = new BasicEnergySettings()
                {
                    EnforceGroupConsistency = true,
                    AtomsPerGroup = new ValueSetting<int>("Particles per Group", 2, 8),
                    GroupsPerPosition = new ValueSetting<int>("Groups per Position", 0, 0, 4, 10),
                    PermutationsPerGroup = new ValueSetting<long>("Permutations per Group", 1, 1, 500, 5000),
                    PositionsPerStable = new ValueSetting<long>("Positions per Stable Environment", 0, 1, 500, 5000),
                    PositionsPerUnstable = new ValueSetting<long>("Positions per Unstable Environment", 0, 1, 100, 1000),
                    PairEnergies = new ValueSetting<double>("Pair Energy", -100, 100),
                    GroupEnergies = new ValueSetting<double>("Group Energy", -100, 100)
                },
                SimulationSettings = new BasicSimulationSettings()
                {
                    BreakSampleInterval = new ValueSetting<int>("Break Sample Interval", 1, 1, 100, 1000),
                    BreakSampleLength = new ValueSetting<int>("Break Sample Length", 1, 100, 10000, 100000),
                    ResultSampleLength = new ValueSetting<int>("Result Sample Length", 1, 1000, 10000, 1000000),
                    BreakTolerance = new ValueSetting<double>("Break Tolerance", 0, 0, 0.1, 1),
                    Doping = new ValueSetting<double>("Doping Concentration", 0, 0, 1, 1),
                    ElectricField = new ValueSetting<double>("Electric Field", 0, 1e4, 1e9, 1e10),
                    MonteCarloSteps = new ValueSetting<int>("Steps per Particle", 1, 1, 1000, 1000000),
                    JobCount = new ValueSetting<int>("Jobs per Simulation", 1, 5, 100, 1000),
                    WriteCallCount = new ValueSetting<int>("Write Calls per Simulation", 0, 5, 100, 1000),
                    Temperature = new ValueSetting<double>("Temperature", 0.1, 100, 5000, 10000),
                    Normalization = new ValueSetting<double>("Normalization", 0, 0, 0.1, 1.0),
                    SeriesPermutationCount = new ValueSetting<int>("Series Simulation Count", 0, 0, 1000, 5000),
                    SupercellPositionCount = new ValueSetting<int>("Supercell Position Count", 1, 100, 100000, 500000),
                    UnitCellsPerDirection = new ValueSetting<int>("Cells per Direction", 1, 1, 100, 256),
                    ForceTerminationTime = new ValueSetting<int>("Forced Termination Time", 0, 1, 48, 120),
                    TerminationSuccessRate = new ValueSetting<double>("Lower Termintation Success Rate", 1, 10, int.MaxValue, int.MaxValue),
                    EnergySetCount = new ValueSetting<int>("Energy Set Count", 0, 0, 10, 100),
                    TransitionCount = new ValueSetting<int>("Transitions per Simulation", 0, 0, 10, 100),
                    Naming = new StringSetting("Name String", "^[a-zA-Z0-9\\+\\-\\(\\)]{2,100}$"),
                    Seeding = new StringSetting("Seed String", "^[a-zA-Z0-9\\+\\-\\(\\)]{4,100}$")
                }
        };
        }
    }
}
