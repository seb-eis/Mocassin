using Mocassin.Framework.Operations;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Validators
{
    /// <summary>
    /// Validator for new unit cell position model objects that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class UnitCellPositionValidator : DataValidator<IUnitCellPosition, MocassinStructureSettings, IStructureDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public UnitCellPositionValidator(IModelProject modelProject, MocassinStructureSettings settings, IDataReader<IStructureDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new unit cell position object in term of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IUnitCellPosition obj)
        {
            var report = new ValidationReport();
            AddConstraintValidation(obj, report);
            AddObjectUniquenessValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validates that a new cell position is within the general restraints of the system and adds the results to the validation report
        /// </summary>
        /// <param name="position"></param>
        /// <param name="report"></param>
        private void AddConstraintValidation(IUnitCellPosition position, ValidationReport report)
        {
            var constraint = new NumericConstraint(true, 0.0, 1.0, false, NumericComparer.CreateRanged(ModelProject.CommonNumeric.ComparisonRange));
            if (!constraint.IsValid(position.Vector.A) || !constraint.IsValid(position.Vector.B) || !constraint.IsValid(position.Vector.C))
            {
                var detail0 = $"The position violates the unit cell boundaries {constraint.ToString()}";
                report.AddWarning(ModelMessageSource.CreateRestrictionViolationWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validates that the passed cell position vector cannot be found within the extended wyckoff sets of any of the already defined positions that are not deprecated
        /// and adds the results to the validation report
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dataReader"></param>
        /// <param name="report"></param>
        private void AddObjectUniquenessValidation(IUnitCellPosition position, ValidationReport report)
        {
            var comparer = new VectorComparer3D<Fractional3D>(ModelProject.GeometryNumeric.RangeComparer);
            foreach (var item in DataReader.Access.GetUnitCellPositions())
            {
                if (!item.IsDeprecated)
                {
                    var extended = ModelProject.SpaceGroupService.GetAllWyckoffPositions(item.Vector);
                    if (extended.Contains(position.Vector))
                    {
                        var detail0 = $"Provided unit cell position is already defined by index ({item.Index})";
                        report.AddWarning(ModelMessageSource.CreateModelDuplicateWarning(this, detail0));
                        break;
                    }
                }
            }
        }
    }
}
