using System.Runtime.InteropServices;

namespace Mocassin.Tools.UAccess.Readers.McsReader.Components
{
    /// <summary>
    ///     Simulation state meta data struct that contains the meta information of a 'C' Simulator state file
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 80, Pack = 8)]
    public readonly struct McsMetaData
    {
        /// <summary>
        ///     Get the simulated time span in [s]
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double SimulatedTime;

        /// <summary>
        ///     Get the jump normalization during simulation
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double JumpNormalization;

        /// <summary>
        ///     Get the raw max jump probability used for normalization calculation
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double MaxRawJumpProbability;

        /// <summary>
        ///     Get the lattice energy in [eV]
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double LatticeEnergy;

        /// <summary>
        ///     Get the total program execution time in [s]
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double TotalExecutionTime;

        /// <summary>
        ///     Get the simulation cycle execution rate in [Hz]
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double SimulationCycleRate;

        /// <summary>
        ///     Get the simulation cycle success rate in [Hz]
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double SimulationSuccessRate;

        /// <summary>
        ///     Get the execution time per block in [s]
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double BlockExecutionTime;

        /// <summary>
        ///     Get the 64 bit state of the PCG32 random number generator as a <see cref="long" /> value
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long Pcg32State;

        /// <summary>
        ///     Get the 64 bit increase of the PCG32 random number generator as a <see cref="long" /> value
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long Pcg32Increase;
    }
}