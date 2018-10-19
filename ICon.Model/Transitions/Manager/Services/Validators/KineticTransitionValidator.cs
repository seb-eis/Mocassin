using System;
using System.Linq;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.Validators
{
    /// <summary>
    ///     Validator for new kinetic transitions that checks for compatibility with existing data and general object
    ///     constraints
    /// </summary>
    public class KineticTransitionValidator : DataValidator<IKineticTransition, MocassinTransitionSettings, ITransitionDataPort>
    {
        /// <inheritdoc />
        public KineticTransitionValidator(IModelProject modelProject, MocassinTransitionSettings settings,
            IDataReader<ITransitionDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IKineticTransition obj)
        {
            var report = new ValidationReport();
            AddHasContentValidation(obj, report);
            AddAbstractTransitionValidation(obj, report);
            AddTransitionGeometryValidation(obj, report);
            AddChargeConsistencyValidation(obj, report);
            return report;
        }

        /// <summary>
        ///     Validates that a kinetic transition is not empty in terms of required model content sequences and adds the results
        ///     to the validation report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        protected void AddHasContentValidation(IKineticTransition transition, ValidationReport report)
        {
            if (transition.GeometryStepCount != 0) 
                return;

            const string detail = "The provided kinetic transition contains no geometry information and cannot describe a valid transition";
            report.AddWarning(ModelMessageSource.CreateMissingOrEmptyContentWarning(this, detail));
        }

        /// <summary>
        ///     Validates the transition geometry of a kinetic transition in terms of match with the abstract transition and basic
        ///     shape of the geometry and adds the results to the validation report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddTransitionGeometryValidation(IKineticTransition transition, ValidationReport report)
        {
            var groupCount = transition.AbstractTransition.StateCount;
            if (transition.GeometryStepCount != groupCount)
            {
                const string detail0 = "The abstract transition and the selected geometry set for the kinetic transition do not match in transition size";
                var detail1 =
                    $"Abstract transition defines ({groupCount}) positions while the geometry set defines ({transition.GeometryStepCount}) positions";
                report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0, detail1));
            }

            if (!new TransitionAnalyzer().ContainsRingTransition(transition.GetGeometrySequence(), ModelProject.SpaceGroupService.Comparer)
            ) return;

            const string detail2 = "The transition geometry contains a ring transition where one position is contained multiple times";
            report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail2));
        }

        /// <summary>
        ///     Validates that the transition abstract is a kinetic and not a metropolis type and adds the results to the report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddAbstractTransitionValidation(IKineticTransition transition, ValidationReport report)
        {
            var patternType = ConnectorPattern.DeterminePatternType(transition.AbstractTransition.GetConnectorSequence());
            switch (patternType)
            {
                case ConnectorPatternType.Metropolis:
                    var detail0 = $"Kinetic transitions cannot use the {patternType} pattern type as is does not support a transition state";
                    report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0));
                    break;

                case ConnectorPatternType.Undefined:
                    throw new InvalidOperationException("Unsupported transition pattern type previously passed validation");
            }
        }

        /// <summary>
        ///     Validates that the charge exchange between each consecutive dynamically linked
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddChargeConsistencyValidation(IKineticTransition transition, ValidationReport report)
        {
            var swapChain =
                new TransitionAnalyzer().GetChargeTransportChain(transition.AbstractTransition, ModelProject.CommonNumeric.RangeComparer);
            if (!swapChain.Any(double.IsNaN)) 
                return;

            const string detail0 = "The transition charge transport chain is ill defined. Please reconsider your abstract transition definition!";
            const string detail1 = "Problem 1 : Property based conductivity calculation will yield nonsense";
            const string detail2 = "Problem 2 : Separation of complex property and ion movement will yield nonsense";
            report.AddWarning(ModelMessageSource.CreateFeatureBreakingInputWarning(this, detail0, detail1, detail2));
        }
    }
}