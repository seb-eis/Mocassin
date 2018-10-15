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
    [Flags]
    public enum SimulationBaseFlags
    {
        UseDynamicBreak = 0b1, UseCheckpointSystem = 0b10, AutoDetectStuckSimulation = 0b100, CopyStdoutToFile = 0b1000,
        FullDebugStateDump = 0b10000
    }

    /// <inheritdoc cref="ICon.Model.Simulations.ISimulation"/>
    /// <remarks> Abstract base class for simulation implementations </remarks>
    [DataContract]
    public abstract class SimulationBase : ModelObject, ISimulation
    {
        /// <inheritdoc />
        [DataMember]
        public string Name { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string CustomRngSeed { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double Temperature { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int TargetMcsp { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int WriteOutCount { get; set; }

        /// <inheritdoc />
        [DataMember]
        public SimulationBaseFlags BaseFlags { get; set; }

        /// <inheritdoc />
        [DataMember]
        public TimeSpan SaveRunTimeLimit { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double LowerSuccessRateLimit { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int JobCount { get; set; }

        /// <summary>
        /// Load information for an external background provider source that is used to create an energy background for the simulation
        /// </summary>
        [DataMember]
        public ExternalLoadInfo EnergyBackgroundProviderInfo { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        IExternalLoadInfo ISimulation.EnergyBackgroundProviderInfo => EnergyBackgroundProviderInfo;

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ISimulation>(obj) is ISimulation simulation))
                return null;

            Temperature = simulation.Temperature;
            TargetMcsp = simulation.TargetMcsp;
            WriteOutCount = simulation.WriteOutCount;
            BaseFlags = simulation.BaseFlags;
            SaveRunTimeLimit = simulation.SaveRunTimeLimit;
            Name = simulation.Name;
            CustomRngSeed = simulation.CustomRngSeed;
            JobCount = simulation.JobCount;
            LowerSuccessRateLimit = simulation.LowerSuccessRateLimit;
            EnergyBackgroundProviderInfo = new ExternalLoadInfo(simulation.EnergyBackgroundProviderInfo);
            return this;
        }
    }
}
