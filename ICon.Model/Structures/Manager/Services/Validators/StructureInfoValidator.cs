using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Validators
{
    /// <summary>
    ///     Validator for new structure info model parameter that checks for compatibility with existing data and general object
    ///     constraints
    /// </summary>
    public class StructureInfoValidator : DataValidator<IStructureInfo, MocassinStructureSettings, IStructureDataPort>
    {
        /// <inheritdoc />
        public StructureInfoValidator(IModelProject modelProject, MocassinStructureSettings settings,
            IDataReader<IStructureDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IStructureInfo obj)
        {
            var report = new ValidationReport();
            AddGenericContentEqualityValidation(DataReader.Access.GetStructureInfo(), obj, report);
            return report;
        }
    }
}