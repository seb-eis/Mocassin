using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Simulation series validator for the specific case of kinetic simulation series. Extends the base series validator functionality
    /// </summary>
    public class KineticSeriesValidator : SimulationSeriesValidator, IDataValidator<IKineticSimulationSeries>
    {
        /// <summary>
        /// Create new kinetic series validator from project services, simulation settings and simulation model data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public KineticSeriesValidator(IModelProject modelProject, MocassinSimulationSettings settings, IDataReader<ISimulationDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <summary>
        /// Validates the basic and kinetic specific properties of a kinetic simulation series and creates a validation report containing the results
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IValidationReport Validate(IKineticSimulationSeries obj)
        {
            var report = (ValidationReport)base.Validate(obj);
            AddValueSeriesValidations(obj, report);
            return report;
        }

        /// <summary>
        /// Validates all kinetic specific value series of the simulation series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected void AddValueSeriesValidations(IKineticSimulationSeries series, ValidationReport report)
        {
            AddSingleSeriesValidation(series.ElectricFieldSeries, Settings.ElectricField, report);
            AddSingleSeriesValidation(series.NormalizationProbabilitySeries, Settings.Normalization, report);
        }
    }
}
