using System.Text.RegularExpressions;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.Validators
{
    /// <summary>
    /// Validator for new state exchange pairs that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class PropertyStatePairValidator : DataValidator<IStateExchangePair, MocassinTransitionSettings, ITransitionDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public PropertyStatePairValidator(IModelProject modelProject, MocassinTransitionSettings settings, IDataReader<ITransitionDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate the passed object in terms of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IStateExchangePair obj)
        {
            var report = new ValidationReport();
            AddGenericObjectDuplicateValidation(obj, DataReader.Access.GetStateExchangePairs(), report);
            AddStateExchangeValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validates that the state pair represents a valid change (e.g. is not a self exchange where the paricle does not change) and adds the reults to the validation report
        /// </summary>
        /// <param name="statePair"></param>
        /// <param name="report"></param>
        protected void AddStateExchangeValidation(IStateExchangePair statePair, ValidationReport report)
        {
            if (statePair.DonorParticle == statePair.AcceptorParticle)
            {
                var detail = $"Donor and acceptor states of the provided state pair are identical and cannot describe a valid state change";
                report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail));
            }
        }
    }
}
