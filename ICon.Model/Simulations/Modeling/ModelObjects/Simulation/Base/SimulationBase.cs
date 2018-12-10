using System;
using System.Runtime.Serialization;
using Mocassin.Framework.Provider;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     The simulation setting flag that defines basic settings that can be used with both metropolis and kinetic
    ///     simulations
    /// </summary>
    [Flags]
    public enum SimulationBaseFlags
    {
        UseDynamicBreak = 0b1,
        UseCheckpointSystem = 0b10,
        AutoDetectStuckSimulation = 0b100,
        CopyStdoutToFile = 0b1000,
        FullDebugStateDump = 0b10000
    }

    /// <inheritdoc cref="ISimulation" />
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
            return this;
        }
    }
}