using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    /// Object change handler that handles internal simulation data conflicts on changed metropolis series objects
    /// </summary>
    public class MetropolisSeriesChangeHandler : SeriesChangeHandlerBase<MetropolisSimulationSeries>
    {
        /// <summary>
        /// Create new metropolis simulation series change handler with the provided data accessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="modelProject"></param>
        public MetropolisSeriesChangeHandler(IDataAccessor<SimulationModelData> dataAccess, IModelProject modelProject)
            : base(dataAccess, modelProject)
        {
        }

        /// <summary>
        /// Hanldes the internal data conflicts in the simulation induced by the passed model object as well as resolvable conflicts within the object itself
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(MetropolisSimulationSeries series)
        {
            var report = base.HandleConflicts(series);
            HandleUndefinedProperties(series, report);
            return report;
        }

        /// <summary>
        /// Correct all null value series properties of the metropolis information to a 'single value series' that matches the base simulation value and
        /// replaces null collections by empty collections
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected void HandleUndefinedProperties(MetropolisSimulationSeries series, ConflictReport report)
        {
            HandlePotentialNullSeries("Break Tolerance", report,
                () => series.BreakSampleIntervalSeries,
                a => series.BreakSampleLengthSeries = a,
                series.BaseSimulation.BreakSampleLength);

            HandlePotentialNullSeries("Break Sample Length", report,
                () => series.BreakSampleLengthSeries,
                a => series.BreakSampleLengthSeries = a,
                series.BaseSimulation.BreakSampleLength);

            HandlePotentialNullSeries("Break Sample Interval", report,
                () => series.BreakSampleIntervalSeries,
                a => series.BreakSampleIntervalSeries = a,
                series.BaseSimulation.BreakSampleIntervalMcs);

            HandlePotentialNullSeries("Results Sample Length", report,
                () => series.ResultSampleLengthSeries,
                a => series.ResultSampleLengthSeries = a,
                series.BaseSimulation.ResultSampleMcs);
        }
    }
}
