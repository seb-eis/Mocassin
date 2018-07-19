using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations.ConflictHandling
{
    /// <summary>
    /// Object change handler that handles internal simulation data conflicts on changed metropolis series objects
    /// </summary>
    public class KineticSeriesChangeHandler : SeriesChangeHandlerBase<KineticSimulationSeries>
    {
        /// <summary>
        /// Create new kinetic simulation series change handler with the provided data accessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public KineticSeriesChangeHandler(IDataAccessor<SimulationModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Handles the internal data conflicts in the simulation induced by the passed model object as well as resolvable conflicts within the object itself
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(KineticSimulationSeries series)
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
        public void HandleUndefinedProperties(KineticSimulationSeries series, ConflictReport report)
        {
            HandlePotentialNullSeries("Electic Field Magnitude", report,
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
