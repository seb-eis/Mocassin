using Mocassin.Framework.Constraints;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Represents a specialized simulation series for metropolis monte carlo simulations
    /// </summary>
    public interface IMetropolisSimulationSeries : ISimulationSeries
    {
        /// <summary>
        ///     Get the metropolis base simulation this series is based upon
        /// </summary>
        new IMetropolisSimulation BaseSimulation { get; }

        /// <summary>
        ///     Get the series for the break tolerance values
        /// </summary>
        IValueSeries BreakToleranceSeries { get; }

        /// <summary>
        ///     Get the value series for the break sample length
        /// </summary>
        IValueSeries BreakSampleLengthSeries { get; }

        /// <summary>
        ///     Get the value series for the break sample interval
        /// </summary>
        IValueSeries BreakSampleIntervalSeries { get; }

        /// <summary>
        ///     Get the value series for the results sample length
        /// </summary>
        IValueSeries ResultSampleLengthSeries { get; }
    }
}