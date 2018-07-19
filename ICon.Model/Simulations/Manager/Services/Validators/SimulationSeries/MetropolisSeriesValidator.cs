﻿using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Simulation series validator for the specific case of metropolis simulation series. Extends the base series validator functionality
    /// </summary>
    public class MetropolisSeriesValidator : SimulationSeriesValidator, IDataValidator<IMetropolisSimulationSeries>
    {
        /// <summary>
        /// Create new metropolis series validator from project services, simulation settigs and simulation model data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public MetropolisSeriesValidator(IProjectServices projectServices, BasicSimulationSettings settings, IDataReader<ISimulationDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validates the basic and metropolis specific properties of a metropolis simulation series and creates a validation report containing the results
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IValidationReport Validate(IMetropolisSimulationSeries obj)
        {
            var report = (ValidationReport)base.Validate(obj);
            AddValueSeriesValidations(obj, report);
            return report;
        }

        /// <summary>
        /// VAlidates the metropolis specific value series information of a simulation series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected void AddValueSeriesValidations(IMetropolisSimulationSeries series, ValidationReport report)
        {
            AddSingleSeriesValidation(series.BreakToleranceSeries, Settings.BreakTolerance, report);
            AddSingleSeriesValidation(series.BreakSampleLengthSeries, Settings.BreakSampleLength, report);
            AddSingleSeriesValidation(series.BreakSampleIntervalSeries, Settings.BreakSampleInterval, report);
            AddSingleSeriesValidation(series.ResultSampleLengthSeries, Settings.ResultSampleLength, report);
        }
    }
}
