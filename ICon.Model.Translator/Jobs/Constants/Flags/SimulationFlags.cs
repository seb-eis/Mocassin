using System;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Basic simulation job flags that are shared between the different simulation types
    /// </summary>
    [Flags]
    public enum SimulationJobFlags
    {
    }

    /// <summary>
    ///     The general simulation status flags that are shared between the simulation types
    /// </summary>
    [Flags]
    public enum SimulationStatusFlags
    {
    }

    /// <summary>
    ///     The kinetic monte carlo specific simulation job flags
    /// </summary>
    [Flags]
    public enum SimulationKmcJobFlags
    {
    }

    /// <summary>
    ///     The metropolis monte carlo specific simulation job flags
    /// </summary>
    [Flags]
    public enum SimulationMmcJobFlags
    {
    }
}