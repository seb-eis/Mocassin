using System.Linq;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.Validators
{
    /// <summary>
    ///     Validator for new state exchange groups that checks for compatibility with existing data and general object
    ///     constraints
    /// </summary>
    public class StateExchangeGroupValidator : DataValidator<IStateExchangeGroup, MocassinTransitionSettings, ITransitionDataPort>
    {
        /// <inheritdoc />
        public StateExchangeGroupValidator(IModelProject modelProject, MocassinTransitionSettings settings,
            IDataReader<ITransitionDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IStateExchangeGroup obj)
        {
            var report = new ValidationReport();
            if (!AddHasContentValidation(obj, report))
                return report;

            AddGenericObjectDuplicateValidation(obj, DataReader.Access.GetStateExchangeGroups(), report);
            AddContainsHybridExchangeDefinitions(obj, report);
            return report;
        }

        /// <summary>
        ///     Validates that the state exchange group contains at least one element of each required content element and adds the
        ///     results to the validation report (Returns false on failure)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected bool AddHasContentValidation(IStateExchangeGroup group, ValidationReport report)
        {
            if (group.StatePairCount != 0)
                return true;

            const string detail = "The provided state exchange group does not contain any state pairs and does not describe a valid set of state changes";
            report.AddWarning(ModelMessageSource.CreateMissingOrEmptyContentWarning(this, detail));
            return false;
        }

        /// <summary>
        ///     Validates that the state exchange group does not contain hybrid exchange definitions, that is, the exchange pairs
        ///     do not mix charge transport and physical movement and adds the results to the validation report
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        protected void AddContainsHybridExchangeDefinitions(IStateExchangeGroup group, ValidationReport report)
        {
            var condition0 = group.GetStateExchangePairs().All(x => x.DonorParticle.Symbol == x.AcceptorParticle.Symbol && !x.DonorParticle.Equals(x.AcceptorParticle));
            var condition1 = group.GetStateExchangePairs().All(x => x.DonorParticle.Symbol != x.AcceptorParticle.Symbol || x.DonorParticle.Equals(x.AcceptorParticle));
            if (condition0 ^ condition1) return;

            const string detail0 = "The provided state group mixes physical exchanges and charge state changes which is not supported.";
            const string detail1 = "Please separate the two types into distinct exchange groups.";
            report.AddWarning(ModelMessageSource.CreateRestrictionViolationWarning(this, detail0, detail1));
        }
    }
}