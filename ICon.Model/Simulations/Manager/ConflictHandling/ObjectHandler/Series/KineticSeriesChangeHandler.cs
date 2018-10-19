using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    ///     Object change handler that handles internal simulation data conflicts on changed metropolis series objects
    /// </summary>
    public class KineticSeriesChangeHandler : SeriesChangeHandlerBase<KineticSimulationSeries>
    {
        /// <inheritdoc />
        public KineticSeriesChangeHandler(IDataAccessor<SimulationModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(KineticSimulationSeries series)
        {
            var report = base.HandleConflicts(series);
            HandleUndefinedProperties(series, report);
            return report;
        }

        /// <summary>
        ///     Correct all null value series properties of the metropolis information to a 'single value series' that matches the
        ///     base simulation value and
        ///     replaces null collections by empty collections
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        public void HandleUndefinedProperties(KineticSimulationSeries series, ConflictReport report)
        {
            HandlePotentialNullSeries("Electric Field Magnitude", report,
                () => series.ElectricFieldSeries,
                a => series.ElectricFieldSeries = a,
                series.BaseSimulation.ElectricFieldMagnitude);

            HandlePotentialNullSeries("Normalization Factor", report,
                () => series.NormalizationProbabilitySeries,
                a => series.NormalizationProbabilitySeries = a,
                series.BaseSimulation.NormalizationProbability);
        }
    }
}