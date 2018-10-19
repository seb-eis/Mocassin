using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Validators
{
    /// <summary>
    ///     Validator for new unit cell parameter model parameter that checks for compatibility with existing data and general
    ///     object constraints
    /// </summary>
    public class UnitCellParameterValidator : DataValidator<ICellParameters, MocassinStructureSettings, IStructureDataPort>
    {
        /// <inheritdoc />
        public UnitCellParameterValidator(IModelProject modelProject, MocassinStructureSettings settings,
            IDataReader<IStructureDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(ICellParameters obj)
        {
            var report = new ValidationReport();
            AddGenericContentEqualityValidation(DataReader.Access.GetCellParameters(), obj, report);
            AddCrystalSystemCompatibilityValidation(obj, report);
            return report;
        }

        /// <summary>
        ///     Validate that the new cell parameters match the restrictions of the crystal system and adds the results to the
        ///     validation report
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="report"></param>
        private void AddCrystalSystemCompatibilityValidation(ICellParameters parameters, ValidationReport report)
        {
            var crystalSystem = ModelProject.CrystalSystemService.CrystalSystem;
            if (!crystalSystem.ValidateAngleConditions(parameters.Alpha, parameters.Beta, parameters.Gamma))
            {
                var message = new WarningMessage(this, "Cell angle validation failure");
                message.Details.Add($"The {crystalSystem.SystemName} angle constraints are violated");
                message.Details.Add($"Constraint for angle alpha : {crystalSystem.AlphaConstraint}");
                message.Details.Add($"Constraint for angle beta : {crystalSystem.BetaConstraint}");
                message.Details.Add($"Constraint for angle gamma : {crystalSystem.GammaConstraint}");
            }

            if (!crystalSystem.ValidateParameterConditions(parameters.ParamA, parameters.ParamB, parameters.ParamC))
            {
                var message = new WarningMessage(this, "Cell parameter validation failure");
                message.Details.Add($"The {crystalSystem.SystemName} parameter constraints are violated");
                message.Details.Add($"General parameter constraint : {crystalSystem.ParameterConstraint}");
            }
        }
    }
}