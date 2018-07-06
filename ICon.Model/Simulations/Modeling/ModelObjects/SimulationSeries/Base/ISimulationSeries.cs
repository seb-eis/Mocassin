using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Constraints;
using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a series description of monte carlo simulations of the same type with varrying parameters
    /// </summary>
    public interface ISimulationSeries : IModelObject
    {
        /// <summary>
        /// The base simulation for this series that defines the default values and affiliated transitions
        /// </summary>
        ICustomSimulation BaseSimulation { get; }

        /// <summary>
        /// User defined identification string for this series
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Get the flag that defines if the user defined base lattice should be used for lattice creation (Prohibits size series if true)
        /// </summary>
        bool UseCustomBaseLattice { get; }

        /// <summary>
        /// Get the series of temperature values
        /// </summary>
        IValueSeries TemperatureSeries { get; } 

        /// <summary>
        /// Get the series of monte carlo steps per particle
        /// </summary>
        IValueSeries McspSeries { get; }

        /// <summary>
        /// Get the series of size values for the A direction
        /// </summary>
        IValueSeries LatticeSizeSeriesA { get; }

        /// <summary>
        /// Get the series of size values for the B direction
        /// </summary>
        IValueSeries LatticeSizeSeriesB { get; }

        /// <summary>
        /// Get the series of size values for the C direction
        /// </summary>
        IValueSeries LatticeSizeSeriesC { get; }

        /// <summary>
        /// Get a read only list of file paths to serialized energy infos
        /// </summary>
        IReadOnlyList<string> EnergyFilepathSeries { get; }

        /// <summary>
        /// Get a read only list of the doping series informations
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IDopingSeries> DopingSeriesList { get; }
    }
}
