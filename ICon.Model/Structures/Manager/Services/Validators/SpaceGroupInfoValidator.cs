using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Validators
{
    /// <summary>
    ///     Validator for new space group info model parameter that checks for compatibility with existing data and general
    ///     object constraints
    /// </summary>
    public class SpaceGroupInfoValidator : DataValidator<ISpaceGroupInfo, MocassinStructureSettings, IStructureDataPort>
    {
        /// <inheritdoc />
        public SpaceGroupInfoValidator(IModelProject modelProject, MocassinStructureSettings settings,
            IDataReader<IStructureDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(ISpaceGroupInfo obj)
        {
            var report = new ValidationReport();
            if (obj.GroupIndex == 1) return report;
            AddGenericContentEqualityValidation(DataReader.Access.GetSpaceGroupInfo(), obj, report);
            return report;
        }
    }
}