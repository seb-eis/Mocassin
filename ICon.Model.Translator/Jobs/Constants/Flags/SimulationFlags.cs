using System;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Basic simulation job flags that are shared between the different simulation types. These flags are used by the internal system
    /// </summary>
    [Flags]
    public enum SimulationExecutionFlags
    {
        /// <summary>
        ///     Marks nothing
        /// </summary>
        None = 0,

        /// <summary>
        ///     Marks simulation as a KMC job
        /// </summary>
        KmcSimulation = 1,

        /// <summary>
        ///     Marks a simulation as an MMC job
        /// </summary>
        MmcSimulation = 1 << 1,

        /// <summary>
        ///     Marks a simulation to use a pre-run routine
        /// </summary>
        UsePrerun = 1 << 2,

        /// <summary>
        ///     Marks a simulation to skip the binary state save process
        /// </summary>
        NoSaving = 1 << 3,

        /// <summary>
        ///     Marks a simulation to correct the dual-definition of degrees of freedom (Only relevant to KMC time-stepping
        ///     calculation)
        /// </summary>
        /// <remarks>
        ///     In a completely unoptimized simulation every degree of freedom exists twice in the system which causes
        ///     invalid time calculation
        /// </remarks>
        UseDualDofCorrection = 1 << 4,

        /// <summary>
        ///     Marks a simulation to skip the time expensive jump logging step into a histogram (KMC only)
        /// </summary>
        NoJumpLogging = 1 << 5,

        /// <summary>
        ///     Marks a simulation to use the fast exponential approximation by N. Schraudolph
        /// </summary>
        UseFastExp = 1 << 6
    }

    /// <summary>
    ///     Custom simulation job flags that provides customizable parts of <see cref="SimulationExecutionFlags" /> as overwrites
    /// </summary>
    [Flags]
    public enum SimulationExecutionOverwriteFlags
    {
        /// <summary>
        ///     Marks the simulation to not generate state dumps as checkpoints
        /// </summary>
        NoSaving = SimulationExecutionFlags.NoSaving,

        /// <summary>
        ///     Marks a simulation to skip the time expensive jump logging step into a histogram (KMC only)
        /// </summary>
        NoJumpLogging = SimulationExecutionFlags.NoJumpLogging,

        /// <summary>
        ///     Marks a simulation to use the fast exponential approximation by N. Schraudolph
        /// </summary>
        UseFastExp = SimulationExecutionFlags.UseFastExp
    }

    /// <summary>
    ///     The general simulation status flags that are shared between the simulation types
    /// </summary>
    [Flags]
    public enum SimulationStateFlags
    {
    }

    /// <summary>
    ///     The kinetic monte carlo specific simulation job flags defined in the job header
    /// </summary>
    [Flags]
    public enum SimulationKmcJobFlags
    {
    }

    /// <summary>
    ///     The metropolis monte carlo specific simulation job flags defined in the job header
    /// </summary>
    [Flags]
    public enum SimulationMmcJobFlags
    {
    }
}