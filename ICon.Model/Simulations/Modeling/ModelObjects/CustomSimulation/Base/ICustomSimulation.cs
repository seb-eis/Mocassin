using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a custom simulation of undefined type that carries all reference data to generate an encoded simulation dataset
    /// </summary>
    public interface ICustomSimulation : IModelObject
    {
        /// <summary>
        /// Get the user defined identifier for this simulation
        /// </summary>
        string UserIndetifier { get; }

        /// <summary>
        /// The user defined custom random number generator seed for lattice creation
        /// </summary>
        string CustomRngSeed { get; }

        /// <summary>
        /// Get the temperature value of the simulation in [K]
        /// </summary>
        double Temperature { get; }

        /// <summary>
        /// Get the target monte carlo steps per particle of the simulation
        /// </summary>
        int TargetMcsp { get; }

        /// <summary>
        /// The number of MCSP between out calls (results, checkpoints,...) of the simulation
        /// </summary>
        int McspPerDataOutCall { get; }

        /// <summary>
        /// Get the number of jobs that should be created for this simulation
        /// </summary>
        int TranslationCount { get; }

        /// <summary>
        /// The simulation settings falg that describes basic simulation settings
        /// </summary>
        SimulationBaseFlags BaseFlags { get; }

        /// <summary>
        /// Defines the time span after which the simulation will force terminate to avoid shutdown during data write operations
        /// </summary>
        TimeSpan ForcedTerminationTime { get; }
    }
}
