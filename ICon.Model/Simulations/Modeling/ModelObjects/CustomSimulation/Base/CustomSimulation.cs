using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// The simulation setting flag that defines basic settings that can be used with both metropolis and kinetic simulations
    /// </summary>
    public enum SimulationBaseFlags
    {
        UseDynamicBreak = 0, UseCheckpointSystem = 0b1, AutoDetectStuckSimulation = 0b10, CopyStdoutToFile = 0b100,
        FullDebugStateDump = 0b1000
    }

    /// <summary>
    /// Abstract base class for single user defined simulations that carry all reference information to generate an encode simulation dataset
    /// </summary>
    [DataContract]
    public abstract class CustomSimulation : ModelObject, ICustomSimulation
    {
        /// <summary>
        /// The user defined identifier to mark the simulation
        /// </summary>
        [DataMember]
        public string UserIndetifier { get; set; }

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
        /// The MCSP the simulation completes between calls to the data out functions
        /// </summary>
        [DataMember]
        public int McspPerDataOutCall { get; set; }

        /// <summary>
        /// The simulation settings flag that controles basic simulation behaviour
        /// </summary>
        [DataMember]
        public SimulationBaseFlags BaseFlags { get; set; }

        /// <summary>
        /// Defines the time span after which the simulation will force termination to avoid beeing shut off during data write operations
        /// </summary>
        [DataMember]
        public TimeSpan ForcedTerminationTime { get; set; }

        /// <summary>
        /// Defines the number of equivalent jobs produced for this simulation (Each job has its RNG induced values recreated)
        /// </summary>
        [DataMember]
        public int TranslationCount { get; set; }

        /// <summary>
        /// Populate this object from a model object interface and retruns it as a generic model object. Returns null if the population failed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<ICustomSimulation>(obj) is ICustomSimulation simulation)
            {
                Temperature = simulation.Temperature;
                TargetMcsp = simulation.TargetMcsp;
                McspPerDataOutCall = simulation.McspPerDataOutCall;
                BaseFlags = simulation.BaseFlags;
                ForcedTerminationTime = simulation.ForcedTerminationTime;
                UserIndetifier = simulation.UserIndetifier;
                CustomRngSeed = simulation.CustomRngSeed;
                TranslationCount = simulation.TranslationCount;
                return this;
            }
            return null;
        }
    }
}
