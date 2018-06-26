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
        /// Creates a new default project servies data object
        /// </summary>
        /// <returns></returns>
        public static ProjectSettingsData CreateDefault()
        {
            return new ProjectSettingsData
            {
                CommonNumericSettings = new BasicNumericSettings()
                {
                    CompFactor = 1.0e-6,
                    CompRange = 1.0e-10,
                    CompUlp = 10
                },
                GeometryNumericSettings = new BasicNumericSettings()
                {
                    CompFactor = 1.0e-2,
                    CompRange = 1.0e-3,
                    CompUlp = 50
                },
                ConcurrencySettings = new BasicConcurrencySettings()
                {
                    AttemptInterval = TimeSpan.FromMilliseconds(100),
                    MaxAttempts = 20
                },
                ConstantsSettings = new BasicConstantsSettings()
                {
                    BoltzmannConstSI = 1.38064852e-23,
                    GasConstSI = 8.3144598,
                    ElectricPermittivitySI = 8.85418781762e-12,
                    ElementalChargeSI = 1.6021766208e-19
                },
                ParticleSettings = new BasicParticleSettings()
                {
                    ChargeLimit = 10000,
                    ChargeTolerance = 1.0e-3,
                    ParticleLimit = 64,
                    ParticleSetLimit = 1000,
                    NameRegex = "^[a-zA-Z]{1,1}[a-zA-Z0-9\\+\\-\\(\\)]{1,100}$",
                    SymbolRegex = "^[A-Z]{1,1}[a-zA-Z0-9\\+\\-\\(\\)]{0,4}$"
                },
                StructureSettings = new BasicStructureSettings()
                {
                    BasePositionsLimit = 1000,
                    TotalPositionsLimit = 10000,
                    MaxBaseParameterLength = 1000,
                    NameRegex = "^[a-zA-Z0-9\\+\\-\\(\\)]{2,100}$"
                },
                SymmetrySettings = new BasicSymmetrySettings()
                {
                    DatabaseFilepath = $"{Environment.GetEnvironmentVariable("USERPROFILE")}/source/repos/ICon.Program/ICon.Framework.Symmetry/SpaceGroups/SpaceGroups.db",
                    VectorTolerance = 1.0e-6,
                    ParameterTolerance = 1.0e-6
                },
                TransitionSettings = new BasicTransitionSettings()
                {
                    MaxTransitionCount = 100,
                    MinTransitionLength = 2,
                    MaxTransitionLength = 4,
                    AbstractTransitionNameRegex = "^[a-zA-Z0-9\\+\\-\\(\\)]{2,100}$",
                    AutoHandleRuleMovementType = true
                },
                EnergySettings = new BasicEnergySettings()
                {
                    GroupingEnabled = false,
                    EnforceStableGroupingConsistency = true,
                    MaxGroupPermutationCount = 10000,
                    MaxGroupingCount = 10,
                    MaxGroupingSize = 8,
                    MaxStableEnvironmentPositionCount = 5000,
                    MaxUnstableEnvironmentPositionCount = 500,
                    EnvironmentPositionWarningLimit = 250
                }
        };
        }
    }
}
