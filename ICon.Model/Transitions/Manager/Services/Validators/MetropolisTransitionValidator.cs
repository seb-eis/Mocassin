using System.Text.RegularExpressions;

using ICon.Framework.Operations;
using ICon.Framework.Messaging;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.Validators
{
    /// <summary>
    /// Validator for new metropolis transitions that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class MetropolisTransitionValidator : DataValidator<IMetropolisTransition, BasicTransitionSettings, ITransitionDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public MetropolisTransitionValidator(IProjectServices projectServices, BasicTransitionSettings settings, IDataReader<ITransitionDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate the passed object in terms of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IMetropolisTransition obj)
        {
            var report = new ValidationReport();
            AddGenericObjectDuplicateValidation(obj, DataReader.Access.GetMetropolisTransitions(), report);
            return report;
        }
    }
}
