using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.Validators
{
    /// <summary>
    ///     Validator for new state exchange pairs that checks for compatibility with existing data and general object
    ///     constraints
    /// </summary>
    public class StateExchangePairValidator : DataValidator<IStateExchangePair, MocassinTransitionSettings, ITransitionDataPort>
    {
        /// <inheritdoc />
        public StateExchangePairValidator(IModelProject modelProject, MocassinTransitionSettings settings,
            IDataReader<ITransitionDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IStateExchangePair obj)
        {
            var report = new ValidationReport();
            AddGenericObjectDuplicateValidation(obj, DataReader.Access.GetStateExchangePairs(), report);
            //AddStateExchangeValidation(obj, report);
            return report;
        }

        /// <summary>
        ///     Validates that the state pair represents a valid change (e.g. is not a self exchange where the particle does not
        ///     change) and adds the results to the validation report
        /// </summary>
        /// <param name="statePair"></param>
        /// <param name="report"></param>
        protected void AddStateExchangeValidation(IStateExchangePair statePair, ValidationReport report)
        {
            if (statePair.DonorParticle != statePair.AcceptorParticle)
                return;

            const string detail = "Donor and acceptor states of the provided state pair are identical and cannot describe a valid state change";
            report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail));
        }
    }
}