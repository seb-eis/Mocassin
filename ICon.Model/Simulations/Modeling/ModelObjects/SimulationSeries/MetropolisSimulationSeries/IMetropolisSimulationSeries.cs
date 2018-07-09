using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Constraints;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a specialized simulation series for metropolis monte carlo simulations
    /// </summary>
    interface IMetropolisSimulationSeries : ISimulationSeriesBase
    {
        /// <summary>
        /// Get the metropolis base simulation this series is based upon
        /// </summary>
        new IMetropolisSimulation BaseSimulation { get; }

        /// <summary>
        /// Get the series for the break tolerance values
        /// </summary>
        IValueSeries BreakToleranceSeries { get; }

        /// <summary>
        /// Get the value series for the break sample length
        /// </summary>
        IValueSeries BreakSampleLengthSeries { get; }

        /// <summary>
        /// Get the value series for the break sample interval
        /// </summary>
        IValueSeries BreakSampleIntervalSeries { get; }

        /// <summary>
        /// Get the value series for the results sample length
        /// </summary>
        IValueSeries ResultSampleLengthSeries { get; }
    }
}
