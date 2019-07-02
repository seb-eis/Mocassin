using System;
using Mocassin.Framework.Provider;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Represents a custom simulation of undefined type that carries all reference data to generate an encoded simulation
    ///     data set
    /// </summary>
    public interface ISimulation : IModelObject
    {
        /// <summary>
        ///     The user defined custom random number generator seed for lattice creation
        /// </summary>
        string CustomRngSeed { get; }

        /// <summary>
        ///     Get the temperature value of the simulation in [K]
        /// </summary>
        double Temperature { get; }

        /// <summary>
        ///     Get the target monte carlo steps per particle of the simulation
        /// </summary>
        int TargetMcsp { get; }

        /// <summary>
        ///     The number of calls to data out (results, checkpoints,...) during the simulation
        /// </summary>
        int WriteOutCount { get; }

        /// <summary>
        ///     Get the number of jobs that should be created for this simulation
        /// </summary>
        int JobCount { get; }

        /// <summary>
        ///     Defines the save run time for a simulation. After the time span has passed a simulation will automatically
        ///     terminate to avoid forced shutdown during data out operations
        /// </summary>
        TimeSpan SaveRunTimeLimit { get; }

        /// <summary>
        ///     Get the minimal success rate a simulation has to reach. Simulations that fall below this value will be
        ///     automatically terminated
        /// </summary>
        double LowerSuccessRateLimit { get; set; }
    }
}