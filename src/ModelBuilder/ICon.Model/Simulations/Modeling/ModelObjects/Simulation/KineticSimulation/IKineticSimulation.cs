using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Represents a specialized custom simulation that describes the reference data for a kinetic monte carlo routine
    /// </summary>
    public interface IKineticSimulation : ISimulation
    {
        /// <summary>
        ///     Get the number of pre run MCSP for relaxation and normalization
        /// </summary>
        int PreRunMcsp { get; }

        /// <summary>
        ///     The electric field direction as a read only fractional vector
        /// </summary>
        Fractional3D ElectricFieldVector { get; }

        /// <summary>
        ///     Get the magnitude of the electric field in [V/m]
        /// </summary>
        double ElectricFieldMagnitude { get; }

        /// <summary>
        ///     The normalization energy that is used for in-simulation normalization of all jump attempts
        /// </summary>
        double NormalizationEnergy { get; }

        /// <summary>
        ///     Get the list of kinetic transitions affiliated with the simulation
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IKineticTransition> Transitions { get; }
    }
}