using System.Text.RegularExpressions;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.Validators
{
    /// <summary>
    /// Validator for new metropolis transitions that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class MetropolisTransitionValidator : DataValidator<IMetropolisTransition, MocassinTransitionSettings, ITransitionDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public MetropolisTransitionValidator(IModelProject modelProject, MocassinTransitionSettings settings, IDataReader<ITransitionDataPort> dataReader)
            : base(modelProject, settings, dataReader)
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
            AddAbstractTransitionValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validates if the abstract transition is valid in the sense of a metropolis transition and adds the results to the report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddAbstractTransitionValidation(IMetropolisTransition transition, ValidationReport report)
        {
            var metropolisPattern = ConnectorPattern.GetMetropolisPattern();
            if (!metropolisPattern.IsValid(transition.AbstractTransition.GetConnectorSequence()))
            {
                var detail0 = $"The abstract transition does not describe a valid metropolis pattern";
                var detail1 = $"Metropolis transition patterns follow the regular expression {metropolisPattern.PatternRegex.ToString()}";
                report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0, detail1));
            }
        }
    }
}
