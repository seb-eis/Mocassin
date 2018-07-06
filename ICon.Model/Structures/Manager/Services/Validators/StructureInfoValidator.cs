using System.Text.RegularExpressions;

using ICon.Framework.Operations;
using ICon.Framework.Messaging;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.Validators
{
    /// <summary>
    /// Validator for new structure info model parametr that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class StructureInfoValidator : DataValidator<IStructureInfo, BasicStructureSettings, IStructureDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public StructureInfoValidator(IProjectServices projectServices, BasicStructureSettings settings, IDataReader<IStructureDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new structure info parameter object in terms of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IStructureInfo obj)
        {
            var report = new ValidationReport();
            AddGenericContentEqualityValidation(DataReader.Access.GetStructureInfo(), obj, report);
            AddNameValidation(obj, report);
            return report;
        }
        /// <summary>
        /// Validates the structure name and adds the results to the validation report
        /// </summary>
        /// <param name="info"></param>
        /// <param name="report"></param>
        private void AddNameValidation(IStructureInfo info, ValidationReport report)
        {
            if (new Regex(Settings.NameStringPattern).IsMatch(info.Name) == false)
            {
                var message = new WarningMessage(this, "Structure name validation failure");
                message.Details.Add($"Naming violates the the naming restriction regex {Settings.NameStringPattern}");
                report.AddWarning(message);
            }
        }
    }
}
