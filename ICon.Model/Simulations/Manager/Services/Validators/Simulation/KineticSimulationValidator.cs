using System;
using System.Collections.Generic;
using System.Linq;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Validator for the kinetic simulation model objects that extendes the base simulation validator with kinetic specific methods
    /// </summary>
    public class KineticSimulationValidator : SimulationBaseValidator, IDataValidator<IKineticSimulation>
    {
        /// <summary>
        /// Create new kinetic simulation validator from project services, simulation settings and simulation model data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public KineticSimulationValidator(IProjectServices projectServices, BasicSimulationSettings settings, IDataReader<ISimulationDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public IValidationReport Validate(IKineticSimulation obj)
        {
            var report = (ValidationReport)base.Validate(obj);
            AddPhysicalParameterValidations(obj, report);
            AddKineticFlagValidation(obj, report);
            AddTransitionsValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validates the physical parameters of the kinetic simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected void AddPhysicalParameterValidations(IKineticSimulation simulation, ValidationReport report)
        {
            if (Settings.Normalization.ParseValue(simulation.NormalizationProbability, out var warnings) != 0)
            {
                report.AddWarnings(warnings);
            }

            if (Settings.ElectricField.ParseValue(simulation.ElectricFieldMagnitude, out warnings) != 0)
            {
                report.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates the specific flags of the kinetic simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected void AddKineticFlagValidation(IKineticSimulation simulation, ValidationReport report)
        {
            if (!simulation.KineticFlags.HasFlag(KineticSimulationFlags.UseDynamicTrackers | KineticSimulationFlags.UseStaticTrackers)) 
                return;
            var detail0 = "Both advanced tracking options are enabled. This significantly increases memory and storage space requirements";
            var detail1 = "Option 1: Use static tracking for position bound flow information (e.g. flow integrals)";
            var detail2 = "Option 2: Use dynamic tracking for particle bound movement information (e.g. mean square displacement)";
            var detail3 = "Option 3: Use exchange group tracking (always enabled). Yields the global mean displacements of the exchange groups and particles";
            report.AddWarning(ModelMessageSource.CreateNotRecommendedWarning(this, detail0, detail1, detail2, detail3));
        }

        /// <summary>
        /// Validates the transitions affiliated with the kinetic simulation and adds the results to the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        protected void AddTransitionsValidation(IKineticSimulation simulation, ValidationReport report)
        {
            if (Settings.TransitionCount.ParseValue(simulation.Transitions.Count, out var warnings) != 0)
            {
                report.AddWarnings(warnings);
            }
        }
    }
}
