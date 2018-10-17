using System;
using System.Collections.Generic;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Abstract base validator for all non sepcified simulations bases.
    /// </summary>
    public abstract class SimulationBaseValidator : DataValidator<ISimulation, MocassinSimulationSettings, ISimulationDataPort>
    {
        /// <summary>
        /// Creates new simulation base validator with the provided project services, simulation settings and simulation data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        protected SimulationBaseValidator(IModelProject modelProject, MocassinSimulationSettings settings, IDataReader<ISimulationDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {

        }

        /// <summary>
        /// Validates a simulation base in terms of settings compatbility and possible internal data conflicts and retruns a validation report for it
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(ISimulation obj)
        {
            var report = new ValidationReport();

            AddStringValidations(obj, report);
            AddCounterValidations(obj, report);
            AddPhysicalValidations(obj, report);
            AddTerminationLimitValidations(obj, report);
            AddBackgroundProviderValidation(obj, report);
            AddLatticeLinkValidation(obj, report);

            return report;
        }

        /// <summary>
        /// Validates the pattern limited string properties of the simulation base and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddStringValidations(ISimulation simulation, ValidationReport report)
        {          
            if (!Settings.Naming.ParseValue(simulation.Name, out var warnings))
            {
                report.AddWarnings(warnings);
            }

            if (!Settings.Seeding.ParseValue(simulation.CustomRngSeed, out warnings))
            {
                report.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates all counter properties on the simulation base and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddCounterValidations(ISimulation simulation, ValidationReport report)
        {
            if (Settings.JobCount.ParseValue(simulation.JobCount, out var warnings) != 0)
            {
                report.AddWarnings(warnings);
            }

            if (Settings.MonteCarloSteps.ParseValue(simulation.TargetMcsp, out warnings) != 0)
            {
                report.AddWarnings(warnings);
            }

            if (Settings.WriteCallCount.ParseValue(simulation.WriteOutCount, out warnings) != 0)
            {
                report.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates the base flags of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddBaseFlagValidations(ISimulation simulation, ValidationReport report)
        {
            if (simulation.BaseFlags.HasFlag(SimulationBaseFlags.UseCheckpointSystem))
            {
                var detail0 = "Disabling the checkpoint system will prevent a simulation form saving its progress and resume after termination";
                var detail1 = "Option 1: Disable if a huge number of very short simulations is performed to avoid later garbage cleanup";
                var detail2 = "Option 2: Disable for testing purposes where resuming is not required";
                report.AddWarning(ModelMessageSource.CreateNotRecommendedWarning(this, detail0, detail1, detail2));
            }
            if (simulation.BaseFlags.HasFlag(SimulationBaseFlags.FullDebugStateDump))
            {
                var detail0 = "Full state dumping is a memory intensive debug feature that is not recommended for regular usage";
                var detail1 = "Option 1: Write a custom simulation extension library that handles the required output data and formatting";
                report.AddWarning(ModelMessageSource.CreateNotRecommendedWarning(this, detail0, detail1));
            }
        }

        /// <summary>
        /// Validates all physical properties of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddPhysicalValidations(ISimulation simulation, ValidationReport report)
        {
            if (Settings.Temperature.ParseValue(simulation.Temperature, out var warnings) != 0)
            {
                report.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates all preliminary termination limits of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddTerminationLimitValidations(ISimulation simulation, ValidationReport report)
        {
            if (Settings.TerminationSuccessRate.ParseValue(simulation.LowerSuccessRateLimit, out var warnings) != 0)
            {
                report.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates the energy background provider info of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddBackgroundProviderValidation(ISimulation simulation, ValidationReport report)
        {
            if (simulation.EnergyBackgroundProviderInfo.IsUndefined)
            {
                return;
            }
            if (!simulation.EnergyBackgroundProviderInfo.IsValidProviderFor(typeof(object), typeof(IEnergyBackground), out var exception))
            {
                var detail0 = $"The defined assembly load information {(simulation.EnergyBackgroundProviderInfo)} for energy background provision is invalid";
                var detail1 = $"Exception message:\n {exception.Message}";
                report.AddWarning(ModelMessageSource.CreateUserInducedExceptionWarning(this, detail0, detail1));
            }
        }


        /// <summary>
        /// Validates the lattice linking of the simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected virtual void AddLatticeLinkValidation(ISimulation simulation, ValidationReport report)
        {

        }
    }
}
