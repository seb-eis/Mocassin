﻿using System.Text.RegularExpressions;
using System.Linq;

using ICon.Framework.Extensions;
using ICon.Framework.Operations;
using ICon.Framework.Constraints;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.Validators
{
    /// <summary>
    /// Validator for new sbtract transitions that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class AbstractTransitionValidator : DataValidator<IAbstractTransition, BasicTransitionSettings, ITransitionDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public AbstractTransitionValidator(IProjectServices projectServices, BasicTransitionSettings settings, IDataReader<ITransitionDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate the passed object in terms of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IAbstractTransition obj)
        {
            var report = new ValidationReport();

            AddHasContentValidation(obj, report);
            AddGenericObjectDuplicateValidation(obj, DataReader.Access.GetAbstractTransitions(), report);
            AddContentRestrictionsValidation(obj, report);
            AddConnectorPatternValidation(obj, report);

            return report;
        }

        /// <summary>
        /// Validates that the abstract transition has a set of property groups and connectors and adds the reults to the validation report (Retruns false if not)
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        protected void AddHasContentValidation(IAbstractTransition transition, ValidationReport report)
        {
            if (transition.ConnectorCount == 0)
            {
                var detail = "The set of property groups does not contain any content and cannot describe a valid transition";
                report.AddWarning(ModelMessages.CreateMissingOrEmptyContentWarning(this, detail));
            }
            if (transition.ConnectorCount == 0)
            {
                var detail = "The set of position connectors does not contain any content and cannot describe a valid transition";
                report.AddWarning(ModelMessages.CreateMissingOrEmptyContentWarning(this, detail));
            }
        }

        /// <summary>
        /// Validates the content restrictions for an abstract transition as defined by the transition settings object and adds the results to the validation report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddContentRestrictionsValidation(IAbstractTransition transition, ValidationReport report)
        {
            var lengthContraint = new ValueConstraint<int>(true, Settings.MinTransitionLength, Settings.MaxTransitionLength, true);
            if (!lengthContraint.IsValid(transition.StateCount))
            {
                var detail = $"The transition has ({transition.StateCount}) base positions. Allowed position counts are {lengthContraint.ToString()}";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail));
            }
            if (transition.StateCount != transition.ConnectorCount + 1)
            {
                var detail0 = $"The transition ({transition.StateCount}) base positions but ({transition.ConnectorCount}) connector steps";
                var detail1 = $"The expected number of connector steps is ({transition.StateCount - 1})";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0, detail1));
            }
            if (!new Regex(Settings.AbstractTransitionNameRegex).IsMatch(transition.Name))
            {
                var detail = $"The abstract transition name ({transition.Name}) violates the contraining regular expression ({Settings.AbstractTransitionNameRegex})";
                report.AddWarning(ModelMessages.CreateNamingViolationWarning(this, detail));
            }
        }

        /// <summary>
        /// Validates the transition connectors of an abstract transition in terms of a potentially valid physical transition and adds the results to the validation report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name=""></param>
        protected void AddConnectorPatternValidation(IAbstractTransition transition, ValidationReport report)
        {
            var validPatterns = GetValidConnectorPatterns();
            foreach (var pattern in validPatterns)
            {
                if (pattern.IsValid(transition.GetConnectorSequence()))
                {
                    return;
                }
            }
            var detail0 = "The provided connector pattern does not result in a supported physical transition";
            var detail1 = validPatterns.Select(value => $"Valid type ('{value.PatternType}') pattern regex is ('{value.PatternRegex}')");
            report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail1.Concat(detail0.AsSingleton()).ToArray()));
        }

        /// <summary>
        /// Get the set of valid connector patterns that form physical meaningfull transitions
        /// </summary>
        /// <returns></returns>
        protected ConnectorPattern[] GetValidConnectorPatterns()
        {
            return new ConnectorPattern[]
            {
                ConnectorPattern.GetMetropolisPattern(),
                ConnectorPattern.GetBasicKineticPattern(),
                ConnectorPattern.GetBasicVehiclePattern(),
                ConnectorPattern.GetSplittedVehiclePattern()
            };
        }
    }
}
