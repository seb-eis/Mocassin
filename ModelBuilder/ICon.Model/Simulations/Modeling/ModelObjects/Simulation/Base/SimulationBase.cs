using System;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <inheritdoc cref="ISimulation" />
    /// <remarks> Abstract base class for simulation implementations </remarks>
    public abstract class SimulationBase : ModelObject, ISimulation
    {
        /// <inheritdoc />
        public string CustomRngSeed { get; set; }

        /// <inheritdoc />
        public double Temperature { get; set; }

        /// <inheritdoc />
        public int TargetMcsp { get; set; }

        /// <inheritdoc />
        public int WriteOutCount { get; set; }

        /// <inheritdoc />
        public TimeSpan SaveRunTimeLimit { get; set; }

        /// <inheritdoc />
        public double LowerSuccessRateLimit { get; set; }

        /// <inheritdoc />
        public int JobCount { get; set; }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ISimulation>(obj) is { } simulation))
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