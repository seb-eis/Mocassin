using System.Linq;
using System.Text.RegularExpressions;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.Validators
{
    /// <summary>
    ///     Validator for new abstract transitions that checks for compatibility with existing data and general object
    ///     constraints
    /// </summary>
    public class AbstractTransitionValidator : DataValidator<IAbstractTransition, MocassinTransitionSettings, ITransitionDataPort>
    {
        /// <inheritdoc />
        public AbstractTransitionValidator(IModelProject modelProject, MocassinTransitionSettings settings,
            IDataReader<ITransitionDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IAbstractTransition obj)
        {
            var report = new ValidationReport();

            AddHasContentValidation(obj, report);
            if (!report.IsGood)
                return report;

            AddGenericObjectDuplicateValidation(obj, DataReader.Access.GetAbstractTransitions(), report);
            AddContentRestrictionsValidation(obj, report);
            AddConnectorPatternValidation(obj, report);
            AddChargeConsistencyValidation(obj, report);

            return report;
        }

        /// <summary>
        ///     Validates that the abstract transition has a set of property groups and connectors and adds the reults to the
        ///     validation report (Returns false if not)
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        protected void AddHasContentValidation(IAbstractTransition transition, ValidationReport report)
        {
            if (transition.ConnectorCount == 0)
            {
                var detail = "The set of property groups does not contain any content and cannot describe a valid transition";
                report.AddWarning(ModelMessageSource.CreateMissingOrEmptyContentWarning(this, detail));
            }

            if (transition.ConnectorCount == 0)
            {
                var detail = "The set of position connectors does not contain any content and cannot describe a valid transition";
                report.AddWarning(ModelMessageSource.CreateMissingOrEmptyContentWarning(this, detail));
            }
        }

        /// <summary>
        ///     Validates the content restrictions for an abstract transition as defined by the transition settings object and adds
        ///     the results to the validation report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddContentRestrictionsValidation(IAbstractTransition transition, ValidationReport report)
        {
            if (Settings.TransitionCount.ParseValue(transition.StateCount, out var warnings) != 0) report.AddWarnings(warnings);
            if (transition.StateCount != transition.ConnectorCount + 1)
            {
                var detail0 = $"The transition ({transition.StateCount}) base positions but ({transition.ConnectorCount}) connector steps";
                var detail1 = $"The expected number of connector steps is ({transition.StateCount - 1})";
                report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0, detail1));
            }

            if (new Regex(Settings.TransitionStringPattern).IsMatch(transition.Name)) 
                return;

            var detail = $"The abstract transition name ({transition.Name}) violates the constraint regex ({Settings.TransitionStringPattern})";
            report.AddWarning(ModelMessageSource.CreateNamingViolationWarning(this, detail));
        }

        /// <summary>
        ///     Validates the transition connectors of an abstract transition in terms of a potentially valid physical transition
        ///     and adds the results to the validation report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddConnectorPatternValidation(IAbstractTransition transition, ValidationReport report)
        {
            var validPatterns = GetValidConnectorPatterns();
            if (validPatterns.Any(pattern => pattern.IsValid(transition.GetConnectorSequence())))
            {
                return;
            }

            var patternType = ConnectorPattern.DeterminePatternType(transition.GetConnectorSequence());
            if (patternType != ConnectorPatternType.NormalVehicle && patternType != ConnectorPatternType.SplitTransitionVehicle &&
                transition.IsAssociation)
            {
                const string detail0 = "Enabled association behavior flag has not effect on non vehicle connector patterns";
                report.AddWarning(ModelMessageSource.CreateRedundantContentWarning(this, detail0));
            }

            if (patternType == ConnectorPatternType.SplitTransitionVehicle)
                AddNormalVehicleRestrictionValidation(transition, report);

            const string detail1 = "The provided connector pattern does not result in a supported physical transition";
            var detail2 = validPatterns.Select(value => $"Valid type ('{value.PatternType}') pattern regex is ('{value.PatternRegex}')");
            report.AddWarning(ModelMessageSource.CreateRestrictionViolationWarning(this, detail2.Concat(detail1.AsSingleton()).ToArray()));
        }

        /// <summary>
        /// Validates that the passed abstract transition does not carry ambiguous transition definition in case of regular vehicle patterns
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddNormalVehicleRestrictionValidation(IAbstractTransition transition, ValidationReport report)
        {
            var unstableStateGroups = transition.GetStateExchangeGroups().Where(x => x.IsUnstablePositionGroup).ToList();
            if (unstableStateGroups.Count > 1 || unstableStateGroups[0].StatePairCount == 1)
                return;

            const string detail0 = "Multiple transition exchange pairs on single transition vehicles is not supported";
            const string detail1 = "Reason: Automatic determination of transition state is not possible";
            report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0, detail1));
        }

        /// <summary>
        ///     Validates that the charge exchange between each consecutive dynamically linked is physically valid
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddChargeConsistencyValidation(IAbstractTransition transition, ValidationReport report)
        {
            var analyzer = new TransitionAnalyzer();
            var swapChain = analyzer.GetChargeTransportChain(transition, ModelProject.CommonNumeric.RangeComparer);

            if (!swapChain.Any(double.IsNaN) || transition.IsMetropolis)
                return;

            const string detail0 = "The transition charge transport chain is ill/ambiguously defined";
            const string detail1 = "Problem : Focal point charge transfer modulus is ambiguous/undefined and cannot be encoded";
            report.AddWarning(ModelMessageSource.CreateRestrictionViolationWarning(this, detail0, detail1));
        }

        /// <summary>
        ///     Get the set of valid connector patterns that form physical meaningful transitions
        /// </summary>
        /// <returns></returns>
        protected ConnectorPattern[] GetValidConnectorPatterns()
        {
            return new[]
            {
                ConnectorPattern.GetMetropolisPattern(),
                ConnectorPattern.GetBasicKineticPattern(),
                ConnectorPattern.GetBasicVehiclePattern(),
                ConnectorPattern.GetSplitVehiclePattern()
            };
        }
    }
}