using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Abstract base validator for all non specified simulations bases.
    /// </summary>
    public abstract class SimulationBaseValidator : DataValidator<ISimulation, MocassinSimulationSettings, ISimulationDataPort>
    {
        /// <inheritdoc />
        protected SimulationBaseValidator(IModelProject modelProject, MocassinSimulationSettings settings,
            IDataReader<ISimulationDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(ISimulation obj)
        {
            var report = new ValidationReport();

            AddStringValidations(obj, report);
            AddCounterValidations(obj, report);
            AddPhysicalValidations(obj, report);
            AddTerminationLimitValidations(obj, report);
            AddLatticeLinkValidation(obj, report);

            return report;
        }

        /// <summary>
        ///     Validates the pattern limited string properties of the simulation base and adds the results to the validation
        ///     report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddStringValidations(ISimulation simulation, ValidationReport report)
        {
            if (!Settings.Naming.ParseValue(simulation.Name, out var warnings))
                report.AddWarnings(warnings);

            if (!Settings.Seeding.ParseValue(simulation.CustomRngSeed, out warnings))
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates all counter properties on the simulation base and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddCounterValidations(ISimulation simulation, ValidationReport report)
        {
            if (Settings.JobCount.ParseValue(simulation.JobCount, out var warnings) != 0) 
                report.AddWarnings(warnings);

            if (Settings.MonteCarloStepsPerParticle.ParseValue(simulation.TargetMcsp, out warnings) != 0)
                report.AddWarnings(warnings);

            if (Settings.WriteCallCount.ParseValue(simulation.WriteOutCount, out warnings) != 0) 
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates the base flags of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddBaseFlagValidations(ISimulation simulation, ValidationReport report)
        {
            if (simulation.BaseFlags.HasFlag(SimulationBaseFlags.UseCheckpointSystem))
            {
                const string detail0 = "Disabling the checkpoint system will prevent a simulation form saving its progress and resume after termination";
                const string detail1 = "Option 1: Disable if a huge number of very short simulations is performed to avoid later garbage cleanup";
                const string detail2 = "Option 2: Disable for testing purposes where resuming is not required";
                report.AddWarning(ModelMessageSource.CreateNotRecommendedWarning(this, detail0, detail1, detail2));
            }

            if (simulation.BaseFlags.HasFlag(SimulationBaseFlags.FullDebugStateDump))
            {
                const string detail0 = "Full state dumping is a memory intensive debug feature that is not recommended for regular usage";
                const string detail1 = "Option 1: Write a custom simulation extension library that handles the required output data and formatting";
                report.AddWarning(ModelMessageSource.CreateNotRecommendedWarning(this, detail0, detail1));
            }
        }

        /// <summary>
        ///     Validates all physical properties of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddPhysicalValidations(ISimulation simulation, ValidationReport report)
        {
            if (Settings.Temperature.ParseValue(simulation.Temperature, out var warnings) != 0)
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates all preliminary termination limits of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddTerminationLimitValidations(ISimulation simulation, ValidationReport report)
        {
            if (Settings.TerminationSuccessRate.ParseValue(simulation.LowerSuccessRateLimit, out var warnings) != 0)
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates the lattice linking of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddLatticeLinkValidation(ISimulation simulation, ValidationReport report)
        {
        }
    }
}