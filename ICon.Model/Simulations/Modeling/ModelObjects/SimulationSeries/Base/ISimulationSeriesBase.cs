using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Constraints;
using ICon.Framework.Provider;
using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a series description of monte carlo simulations of the same type with varrying parameters
    /// </summary>
    public interface ISimulationSeriesBase : IModelObject
    {
        /// <summary>
        /// The base simulation for this series that defines the default values and affiliated transitions
        /// </summary>
        ISimulationBase BaseSimulation { get; }

        /// <summary>
        /// User defined identification string for this series
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the series of temperature values
        /// </summary>
        IValueSeries TemperatureSeries { get; } 

        /// <summary>
        /// Get the series of monte carlo steps per particle
        /// </summary>
        IValueSeries McspSeries { get; }

        /// <summary>
        /// Get a read only list of all defined energy background load informations used to provider energetic background parametrizations
        /// </summary>
        IReadOnlyList<IExternalLoadInfo> EnergyBackgroundLoadInfos { get; }

        /// <summary>
        /// Get the number of simulations described by this series
        /// </summary>
        /// <returns></returns>
        long GetSimulationCount(); 
    }
}
