using ICon.Mathematics.Constraints;
using ICon.Mathematics.Comparers;
using ICon.Framework.Operations;
using ICon.Framework.Messaging;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.Validators
{
    /// <summary>
    /// Validator for new unit cell position model objects that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class PositionDummyValidator : DataValidator<IPositionDummy, BasicStructureSettings, IStructureDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public PositionDummyValidator(IProjectServices projectServices, BasicStructureSettings settings, IDataReader<IStructureDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new unit cell position object in term of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IPositionDummy obj)
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
        private void AddConstraintValidation(IPositionDummy position, ValidationReport report)
        {
            var constraint = new DoubleConstraint(true, 0.0, 1.0, false, DoubleComparer.CreateRanged(ProjectServices.CommonNumerics.CompRange));
            if (!constraint.IsValid(position.Vector.A) || !constraint.IsValid(position.Vector.B) || !constraint.IsValid(position.Vector.C))
            {
                var detail = $"The dummy violates the unit cell boundaries {constraint.ToString()}";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail));
            }
        }

        /// <summary>
        /// Validates that the passed cell position vector cannot be found within the extended wyckoff sets of any of the already defined positions that are not deprecated
        /// and adds the results to the validation report
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dataReader"></param>
        /// <param name="report"></param>
        private void AddObjectUniquenessValidation(IPositionDummy position, ValidationReport report)
        {
            foreach (var item in DataReader.Access.GetPositionDummies())
            {
                if (!item.IsDeprecated)
                {
                    var extended = ProjectServices.SpaceGroupService.GetAllWyckoffPositions(item.Vector);
                    if (extended.CppFindLowerBound(position.Vector) != extended.Count)
                    {
                        var detail = "Provided dummy position is already present or part of the wyckoff set of another existing dummy";
                        report.AddWarning(ModelMessages.CreateModelDuplicateWarning(this, detail));
                        break;
                    }
                }
            }
        }
    }
}
