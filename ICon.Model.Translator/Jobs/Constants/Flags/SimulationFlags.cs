using System;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Basic simulation job flags that are shared between the different simulation types
    /// </summary>
    [Flags]
    public enum SimulationJobInfoFlags
    {
        KmcSimulation = 1,
        MmcSimulation = 1 << 1,
        UsePrerun = 1 << 2,
        SkipSaving = 1 << 3
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