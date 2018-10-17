using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Validators
{
    /// <summary>
    /// Validator for new unit cell parameter model parameter that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class UnitCellParameterValidator : DataValidator<ICellParameters, MocassinStructureSettings, IStructureDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public UnitCellParameterValidator(IModelProject modelProject, MocassinStructureSettings settings, IDataReader<IStructureDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new unit cell parameter object in terms of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(ICellParameters obj)
        {
            var report = new ValidationReport();
            AddGenericContentEqualityValidation(DataReader.Access.GetCellParameters(), obj, report);
            AddCrystalSystemCompatibiliyValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validate that the new cell parameters match the restrictions of the crystal system and adds the results to the validation report
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="report"></param>
        private void AddCrystalSystemCompatibiliyValidation(ICellParameters parameters, ValidationReport report)
        {
            var crystalSystem = ModelProject.CrystalSystemService.CrystalSystem;
            if (crystalSystem.ValidateAngleConditions(parameters.Alpha, parameters.Beta, parameters.Gamma) == false)
            {
                var message = new WarningMessage(this, "Cell angle validation failure");
                message.Details.Add($"The {crystalSystem.SystemName} angle constraints are violated");
                message.Details.Add($"Constraint for angle alpha : {crystalSystem.AlphaConstraint.ToString()}");
                message.Details.Add($"Constraint for angle beta : {crystalSystem.BetaConstraint.ToString()}");
                message.Details.Add($"Constraint for angle gamma : {crystalSystem.GammaConstraint.ToString()}");
            }
            if (crystalSystem.ValidateParameterConditions(parameters.ParamA, parameters.ParamB, parameters.ParamC) == false)
            {
                var message = new WarningMessage(this, "Cell parameter validation failure");
                message.Details.Add($"The {crystalSystem.SystemName} parameter constraints are violated");
                message.Details.Add($"General parameter constraint : {crystalSystem.ParameterConstraint.ToString()}");
            }
        }
    }
}
