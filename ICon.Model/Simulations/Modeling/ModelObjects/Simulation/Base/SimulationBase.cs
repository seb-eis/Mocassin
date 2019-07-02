using System;
using System.Runtime.Serialization;
using Mocassin.Framework.Provider;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <inheritdoc cref="ISimulation" />
    /// <remarks> Abstract base class for simulation implementations </remarks>
    [DataContract]
    public abstract class SimulationBase : ModelObject, ISimulation
    {
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
            SaveRunTimeLimit = simulation.SaveRunTimeLimit;
            Name = simulation.Name;
            CustomRngSeed = simulation.CustomRngSeed;
            JobCount = simulation.JobCount;
            LowerSuccessRateLimit = simulation.LowerSuccessRateLimit;
            return this;
        }
    }
}