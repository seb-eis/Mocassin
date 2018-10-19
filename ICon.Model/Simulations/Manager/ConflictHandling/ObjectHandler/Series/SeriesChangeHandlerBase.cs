using System;
using System.Collections.Generic;
using Mocassin.Framework.Constraints;
using Mocassin.Framework.Operations;
using Mocassin.Framework.Provider;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    ///     Base class for all simulation series change handlers that handles the base series definition induced conflicts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SeriesChangeHandlerBase<T> : ObjectConflictHandler<T, SimulationModelData> where T : SimulationSeriesBase
    {
        /// <inheritdoc />
        protected SeriesChangeHandlerBase(IDataAccessor<SimulationModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(T obj)
        {
            var report = new ConflictReport();
            HandleUndefinedProperties(obj, report);
            return report;
        }

        /// <summary>
        ///     Correct all null properties of series base information to a 'single value series' that matches the base simulation
        ///     value and
        ///     replaces null collections by empty collections
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected void HandleUndefinedProperties(SimulationSeriesBase series, ConflictReport report)
        {
            series.EnergyBackgroundLoadInfos = series.EnergyBackgroundLoadInfos ?? new List<ExternalLoadInfo>();

            HandlePotentialNullSeries("Temperature", report,
                () => series.TemperatureSeries,
                a => series.TemperatureSeries = a,
                series.BaseSimulation.Temperature);

            HandlePotentialNullSeries("Target Mcsp", report,
                () => series.McspSeries,
                a => series.McspSeries = a,
                series.BaseSimulation.TargetMcsp);
        }

        /// <summary>
        ///     Handles a single null value series through access delegates. If the value series is null it is replaced by a single
        ///     value series
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="report"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="value"></param>
        protected void HandlePotentialNullSeries(string displayName, ConflictReport report, Func<IValueSeries> getter,
            Action<IValueSeries> setter, double value)
        {
            if (getter() != null) 
                return;

            setter(ValueSeries.MakeSingle(value));
            var detail = $"Undefined series for [{displayName}] was replaced by single value of base simulation ({value})";
            report.AddWarning(ModelMessageSource.CreateConflictHandlingWarning(this, detail));
        }
    }
}