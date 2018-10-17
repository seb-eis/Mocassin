using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Validator for the metropolis simulation model objects that extendes the base simulation validator with metropolis specific methods
    /// </summary>
    public class MetropolisSimulationValidator : SimulationBaseValidator, IDataValidator<IMetropolisSimulation>
    {
        /// <summary>
        /// Create new metropolis simulation validator from project services, simulation settings and simulation model data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public MetropolisSimulationValidator(IModelProject modelProject, MocassinSimulationSettings settings, IDataReader<ISimulationDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <summary>
        /// Validates a metropolis simulation in terms of its base simulation and specific settings an returns a validation report containing the results
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IValidationReport Validate(IMetropolisSimulation obj)
        {
            var report = (ValidationReport)base.Validate(obj);

            AddBreakCriteriaValidations(obj, report);
            AddTransitionsValidation(obj, report);

            return report;
        }

        /// <summary>
        /// Validates the metropolis specific break criteria of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected void AddBreakCriteriaValidations(IMetropolisSimulation simulation, ValidationReport report)
        {
            if (Settings.BreakTolerance.ParseValue(simulation.RelativeBreakTolerance, out var warnings) != 0)
            {
                report.AddWarnings(warnings);
            }

            if (Settings.BreakSampleLength.ParseValue(simulation.BreakSampleLength, out warnings) != 0)
            {
                report.AddWarnings(warnings);
            }

            if (Settings.BreakSampleInterval.ParseValue(simulation.BreakSampleIntervalMcs, out warnings) != 0)
            {
                report.AddWarnings(warnings);
            }

            if (Settings.ResultSampleLength.ParseValue(simulation.ResultSampleMcs, out warnings) != 0)
            {
                report.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates the transitions of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected void AddTransitionsValidation(IMetropolisSimulation simulation, ValidationReport report)
        {
            if (Settings.TransitionCount.ParseValue(simulation.Transitions.Count, out var warnings) != 0)
            {
                report.AddWarnings(warnings);
            }
        }
    }
}
