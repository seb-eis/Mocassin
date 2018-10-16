using ICon.Mathematics.Constraints;
using ICon.Mathematics.Comparers;
using ICon.Framework.Operations;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.Validators
{
    /// <summary>
    /// Validator for new unit cell position model objects that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class UnitCellPositionValidator : DataValidator<IUnitCellPosition, BasicStructureSettings, IStructureDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public UnitCellPositionValidator(IProjectServices projectServices, BasicStructureSettings settings, IDataReader<IStructureDataPort> dataReader)
            : base(projectServices, settings, dataReader)
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
            var constraint = new NumericConstraint(true, 0.0, 1.0, false, NumericComparer.CreateRanged(ProjectServices.CommonNumeric.CompRange));
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
            var comparer = new VectorComparer3D<Fractional3D>(ProjectServices.GeometryNumeric.RangeComparer);
            foreach (var item in DataReader.Access.GetUnitCellPositions())
            {
                if (!item.IsDeprecated)
                {
                    var extended = ProjectServices.SpaceGroupService.GetAllWyckoffPositions(item.Vector);
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
