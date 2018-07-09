using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ICon.Framework.Provider;

using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// The simulation setting flag that defines basic settings that can be used with both metropolis and kinetic simulations
    /// </summary>
    public enum SimulationBaseFlags
    {
        UseDynamicBreak = 0b1, UseCheckpointSystem = 0b10, AutoDetectStuckSimulation = 0b100, CopyStdoutToFile = 0b1000,
        FullDebugStateDump = 0b10000
    }

    /// <summary>
    /// Abstract base class for single user defined simulations that carry all reference information to generate an encode simulation dataset
    /// </summary>
    [DataContract]
    public abstract class SimulationBase : ModelObject, ISimulationBase
    {
        /// <summary>
        /// The user defined identifier to mark the simulation
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The user defined custom random number generator seed for lattice creation
        /// </summary>
        [DataMember]
        public string CustomRngSeed { get; set; }

        /// <summary>
        /// The simulation temperature setting in [K]
        /// </summary>
        [DataMember]
        public double Temperature { get; set; }

        /// <summary>
        /// The target monte carlo steps per particle of the simulation
        /// </summary>
        [DataMember]
        public int TargetMcsp { get; set; }

        /// <summary>
        /// The number of write calls (Cechkpoints, data out, ...) during the simulation
        /// </summary>
        [DataMember]
        public int WriteOutCount { get; set; }

        /// <summary>
        /// The simulation settings flag that controles basic simulation behaviour
        /// </summary>
        [DataMember]
        public SimulationBaseFlags BaseFlags { get; set; }

        /// <summary>
        /// Defines the save run time for a simulation. After the time span has passed a simulation will automatically terminate to avoid forced shutdown during data out operations
        /// </summary>
        [DataMember]
        public TimeSpan SaveRunTimeLimit { get; set; }

        /// <summary>
        /// Defines the minimal success rate a simulation has to reach. Simulations that fall below this value will be automatically terminated
        /// </summary>
        [DataMember]
        public double LowerSuccessRateLimit { get; set; }

        /// <summary>
        /// Defines the number of equivalent jobs produced for this simulation (Each job has its RNG induced values recreated)
        /// </summary>
        [DataMember]
        public int JobCount { get; set; }

        /// <summary>
        /// Load information for an external background provider source that is used to create an energy background for the simulation
        /// </summary>
        [DataMember]
        public ExternalLoadInfo EnergyBackgroundProviderInfo { get; set; }

        /// <summary>
        /// REad only interface access to the energy background provider load information
        /// </summary>
        IExternalLoadInfo ISimulationBase.EnergyBackgroundProviderInfo => EnergyBackgroundProviderInfo;

        /// <summary>
        /// Populate this object from a model object interface and retruns it as a generic model object. Returns null if the population failed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<ISimulationBase>(obj) is ISimulationBase simulation)
            {
                Temperature = simulation.Temperature;
                TargetMcsp = simulation.TargetMcsp;
                WriteOutCount = simulation.WriteOutCount;
                BaseFlags = simulation.BaseFlags;
                SaveRunTimeLimit = simulation.SaveRunTimeLimit;
                Name = simulation.Name;
                CustomRngSeed = simulation.CustomRngSeed;
                JobCount = simulation.JobCount;
                EnergyBackgroundProviderInfo = new ExternalLoadInfo(simulation.EnergyBackgroundProviderInfo);
                return this;
            }
            return null;
        }
    }
}
