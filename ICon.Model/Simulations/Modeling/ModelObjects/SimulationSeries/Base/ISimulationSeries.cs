using System.Collections.Generic;
using Mocassin.Framework.Constraints;
using Mocassin.Framework.Provider;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Represents a series description of monte carlo simulations of the same type with varying parameters
    /// </summary>
    public interface ISimulationSeries : IModelObject
    {
        /// <summary>
        ///     The base simulation for this series that defines the default values and affiliated transitions
        /// </summary>
        ISimulation BaseSimulation { get; }

        /// <summary>
        ///     User defined identification string for this series
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Get the series of temperature values
        /// </summary>
        IValueSeries TemperatureSeries { get; }

        /// <summary>
        ///     Get the series of monte carlo steps per particle
        /// </summary>
        IValueSeries McspSeries { get; }

        /// <summary>
        ///     Get a read only list of all defined energy background load information used to provide energetic backgrounds
        /// </summary>
        IReadOnlyList<IExternalLoadInfo> EnergyBackgroundLoadInfos { get; }

        /// <summary>
        ///     Get the number of simulations described by this series
        /// </summary>
        /// <returns></returns>
        long GetSimulationCount();
    }
}