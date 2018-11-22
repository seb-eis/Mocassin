﻿using System;
using Mocassin.Framework.Constraints;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Abstract base class for implementations of simulation series validators. This class provides the basic shared
    ///     validation functions
    /// </summary>
    public abstract class SimulationSeriesValidator : DataValidator<ISimulationSeries, MocassinSimulationSettings, ISimulationDataPort>
    {
        /// <inheritdoc />
        protected SimulationSeriesValidator(IModelProject modelProject, MocassinSimulationSettings settings,
            IDataReader<ISimulationDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(ISimulationSeries obj)
        {
            var report = new ValidationReport();

            AddStringPropertyValidations(obj, report);
            AddValueSeriesValidations(obj, report);
            AddDopingSeriesValidation(obj, report);
            AddExternalDataSourceValidations(obj, report);

            return report;
        }

        /// <summary>
        ///     Validates the string property values of the simulation series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected virtual void AddStringPropertyValidations(ISimulationSeries series, ValidationReport report)
        {
            if (!Settings.Naming.ParseValue(series.Name, out var warnings))
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates all value series objects defined in the simulation series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected virtual void AddValueSeriesValidations(ISimulationSeries series, ValidationReport report)
        {
            AddSingleSeriesValidation(series.TemperatureSeries, Settings.Temperature, report);
            AddSingleSeriesValidation(series.McspSeries, Settings.MonteCarloStepsPerParticle, report);
        }

        /// <summary>
        ///     Validate the doping series object of the simulation series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected virtual void AddDopingSeriesValidation(ISimulationSeries series, ValidationReport report)
        {
        }

        /// <summary>
        ///     Validates load-ability of all external data sources of the series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected virtual void AddExternalDataSourceValidations(ISimulationSeries series, ValidationReport report)
        {
            foreach (var loadInfo in series.EnergyBackgroundLoadInfos)
            {
                if (loadInfo.IsValidProviderFor(typeof(object), typeof(IEnergyBackground), out var exception))
                    continue;

                var detail0 = $"The defined assembly load information {loadInfo} for energy background provision is invalid";
                var detail1 = $"Exception message:\n {exception.Message}";
                report.AddWarning(ModelMessageSource.CreateUserInducedExceptionWarning(this, detail0, detail1));
            }
        }

        /// <summary>
        ///     Validates that the simulation series does not violate the maximum permutation count and adds the result to the
        ///     validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected virtual void AddPermutationCountValidation(ISimulationSeries series, ValidationReport report)
        {
            if (Settings.SeriesPermutation.ParseValue(series.GetSimulationCount(), out var warnings) != 0) report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validate if a value series violates its affiliated value setting and adds the results to the validation report
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="series"></param>
        /// <param name="setting"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        protected void AddSingleSeriesValidation<T>(IValueSeries series, ValueSetting<T> setting, ValidationReport report)
            where T : IComparable<T>, IConvertible
        {
            if (series == null) 
                return;

            if (Settings.SingleValuePermutation.ParseValue(series.GetValueCount(), out var warnings) != 0)
                report.AddWarnings(warnings);

            if (setting.ParseValue((T) Convert.ChangeType(series.LowerLimit, typeof(T)), out warnings) != 0)
                report.AddWarnings(warnings);

            if (setting.ParseValue((T) Convert.ChangeType(series.UpperLimit, typeof(T)), out warnings) != 0) 
                report.AddWarnings(warnings);
        }
    }
}