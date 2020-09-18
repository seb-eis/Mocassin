using Mocassin.Framework.Operations;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Constraints;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Validators
{
    /// <summary>
    ///     Validator for new unit cell position model objects that checks for compatibility with existing data and general
    ///     object constraints
    /// </summary>
    public class CellReferencePositionValidator : DataValidator<ICellSite, MocassinStructureSettings, IStructureDataPort>
    {
        /// <inheritdoc />
        public CellReferencePositionValidator(IModelProject modelProject, MocassinStructureSettings settings,
            IDataReader<IStructureDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(ICellSite obj)
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
        private void AddConstraintValidation(ICellSite position, ValidationReport report)
        {
            var constraint = new NumericConstraint(true, 0.0, 1.0, false,
                NumericComparer.CreateRanged(ModelProject.CommonNumeric.ComparisonRange));

            if (constraint.IsValid(position.Vector.A) && constraint.IsValid(position.Vector.B) &&
                constraint.IsValid(position.Vector.C))
                return;

            var detail0 = $"The position violates the unit cell boundaries {constraint}";
            report.AddWarning(ModelMessageSource.CreateRestrictionViolationWarning(this, detail0));
        }

        /// <summary>
        ///     Validates that the passed cell position vector cannot be found within the extended wyckoff sets of any of the
        ///     already defined positions that are not deprecated and adds the results to the validation report
        /// </summary>
        /// <param name="position"></param>
        /// <param name="report"></param>
        private void AddObjectUniquenessValidation(ICellSite position, ValidationReport report)
        {
            foreach (var item in DataReader.Access.GetCellReferencePositions())
            {
                if (item.IsDeprecated)
                    continue;

                var extended = ModelProject.SpaceGroupService.GetUnitCellP1PositionExtension(item.Vector);
                if (!extended.Contains(position.Vector))
                    continue;

                var detail0 = $"Provided unit cell position is already defined by index ({item.Index})";
                report.AddWarning(ModelMessageSource.CreateModelDuplicateWarning(this, detail0));
                break;
            }
        }
    }
}