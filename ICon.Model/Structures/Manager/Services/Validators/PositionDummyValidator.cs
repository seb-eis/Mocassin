using Mocassin.Framework.Operations;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Constraints;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Validators
{
    /// <summary>
    ///     Validator for new unit cell position model objects that checks for compatibility with existing data and general
    ///     object constraints
    /// </summary>
    public class PositionDummyValidator : DataValidator<IPositionDummy, MocassinStructureSettings, IStructureDataPort>
    {
        /// <inheritdoc />
        public PositionDummyValidator(IModelProject modelProject, MocassinStructureSettings settings,
            IDataReader<IStructureDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IPositionDummy obj)
        {
            var report = new ValidationReport();
            AddConstraintValidation(obj, report);
            AddObjectUniquenessValidation(obj, report);
            return report;
        }

        /// <summary>
        ///     Validates that a new cell position is within the general restraints of the system and adds the results to the
        ///     validation report
        /// </summary>
        /// <param name="position"></param>
        /// <param name="report"></param>
        private void AddConstraintValidation(IPositionDummy position, ValidationReport report)
        {
            var constraint = new NumericConstraint(true, 0.0, 1.0, false,
                NumericComparer.CreateRanged(ModelProject.CommonNumeric.ComparisonRange));

            if (constraint.IsValid(position.Vector.A) && constraint.IsValid(position.Vector.B) &&
                constraint.IsValid(position.Vector.C)) 
                return;

            var detail = $"The dummy violates the unit cell boundaries {constraint}";
            report.AddWarning(ModelMessageSource.CreateRestrictionViolationWarning(this, detail));
        }

        /// <summary>
        ///     Validates that the passed cell position vector cannot be found within the extended wyckoff sets of any of the
        ///     already defined positions that are not deprecated
        ///     and adds the results to the validation report
        /// </summary>
        /// <param name="position"></param>
        /// <param name="report"></param>
        private void AddObjectUniquenessValidation(IPositionDummy position, ValidationReport report)
        {
            foreach (var item in DataReader.Access.GetPositionDummies())
            {
                if (item.IsDeprecated) 
                    continue;

                var extended = ModelProject.SpaceGroupService.GetAllWyckoffPositions(item.Vector);
                if (extended.GetCppLowerBound(position.Vector) == extended.Count) 
                    continue;

                const string detail = "Provided dummy position is already present or part of the wyckoff set of another existing dummy";
                report.AddWarning(ModelMessageSource.CreateModelDuplicateWarning(this, detail));
                break;
            }
        }
    }
}