using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Validator for the kinetic simulation model objects that extends the base simulation validator with kinetic
    ///     specific methods
    /// </summary>
    public class KineticSimulationValidator : SimulationBaseValidator, IDataValidator<IKineticSimulation>
    {
        /// <inheritdoc />
        public KineticSimulationValidator(IModelProject modelProject, MocassinSimulationSettings settings,
            IDataReader<ISimulationDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public IValidationReport Validate(IKineticSimulation obj)
        {
            var report = (ValidationReport) base.Validate(obj);
            AddPhysicalParameterValidations(obj, report);
            AddTransitionsValidation(obj, report);
            return report;
        }

        /// <summary>
        ///     Validates the physical parameters of the kinetic simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected void AddPhysicalParameterValidations(IKineticSimulation simulation, ValidationReport report)
        {
            if (Settings.Normalization.ParseValue(simulation.NormalizationProbability, out var warnings) != 0) 
                report.AddWarnings(warnings);

            if (Settings.ElectricField.ParseValue(simulation.ElectricFieldMagnitude, out warnings) != 0)
                report.AddWarnings(warnings);

            if (Settings.PreRunMcsp.ParseValue(simulation.PreRunMcsp, out warnings) != 0)
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates the transitions affiliated with the kinetic simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected void AddTransitionsValidation(IKineticSimulation simulation, ValidationReport report)
        {
            if (Settings.TransitionCount.ParseValue(simulation.Transitions.Count, out var warnings) != 0)
                report.AddWarnings(warnings);
        }
    }
}