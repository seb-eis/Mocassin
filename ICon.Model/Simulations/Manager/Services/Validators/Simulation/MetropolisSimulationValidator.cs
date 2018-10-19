using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Validator for the metropolis simulation model objects that extends the base simulation validator with metropolis
    ///     specific methods
    /// </summary>
    public class MetropolisSimulationValidator : SimulationBaseValidator, IDataValidator<IMetropolisSimulation>
    {
        /// <inheritdoc />
        public MetropolisSimulationValidator(IModelProject modelProject, MocassinSimulationSettings settings,
            IDataReader<ISimulationDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public IValidationReport Validate(IMetropolisSimulation obj)
        {
            var report = (ValidationReport) base.Validate(obj);

            AddBreakCriteriaValidations(obj, report);
            AddTransitionsValidation(obj, report);

            return report;
        }

        /// <summary>
        ///     Validates the metropolis specific break criteria of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected void AddBreakCriteriaValidations(IMetropolisSimulation simulation, ValidationReport report)
        {
            if (Settings.BreakTolerance.ParseValue(simulation.RelativeBreakTolerance, out var warnings) != 0)
                report.AddWarnings(warnings);

            if (Settings.BreakSampleLength.ParseValue(simulation.BreakSampleLength, out warnings) != 0)
                report.AddWarnings(warnings);

            if (Settings.BreakSampleInterval.ParseValue(simulation.BreakSampleIntervalMcs, out warnings) != 0)
                report.AddWarnings(warnings);

            if (Settings.ResultSampleLength.ParseValue(simulation.ResultSampleMcs, out warnings) != 0) 
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates the transitions of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected void AddTransitionsValidation(IMetropolisSimulation simulation, ValidationReport report)
        {
            if (Settings.TransitionCount.ParseValue(simulation.Transitions.Count, out var warnings) != 0)
                report.AddWarnings(warnings);
        }
    }
}