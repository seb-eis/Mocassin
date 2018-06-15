using System.Text.RegularExpressions;

using ICon.Framework.Operations;
using ICon.Framework.Messaging;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.Validators
{
    /// <summary>
    /// Validator for new space group info model parametr that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class SpaceGroupInfoValidator : DataValidator<ISpaceGroupInfo, BasicStructureSettings, IStructureDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public SpaceGroupInfoValidator(IProjectServices projectServices, BasicStructureSettings settings, IDataReader<IStructureDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new space group info parameter object in terms of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(ISpaceGroupInfo obj)
        {
            var report = new ValidationReport();
            AddGenericContentEqualityValidation(DataReader.Access.GetSpaceGroupInfo(), obj, report);
            return report;
        }
    }
}
