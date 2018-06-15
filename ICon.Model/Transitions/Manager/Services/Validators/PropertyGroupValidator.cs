using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Particles;

namespace ICon.Model.Transitions.Validators
{
    /// <summary>
    /// Validator for new property group that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class PropertyGroupValidator : DataValidator<IPropertyGroup, BasicTransitionSettings, ITransitionDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public PropertyGroupValidator(IProjectServices projectServices, BasicTransitionSettings settings, IDataReader<ITransitionDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate the passed object in terms of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IPropertyGroup obj)
        {
            var report = new ValidationReport();
            if (!AddHasContentValidation(obj, report))
            {
                return report;
            }

            AddGenericObjectDuplicateValidation(obj, DataReader.Access.GetPropertyGroups(), report);
            AddChargeTransferValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validates that the property group contains at least one element of each required content element and adds the reults to the validation report (Returns false on failure)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected bool AddHasContentValidation(IPropertyGroup group, ValidationReport report)
        {
            if (group.StatePairCount == 0)
            {
                var detail = "The provided property group does not contain any state pairs and does not describe a valid set of state changes";
                report.AddWarning(ModelMessages.CreateMissingOrEmptyContentWarning(this, detail));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates that all state pairs of a property group share the same charge transfer value (acceptor state - donor state) and add the results to the validation report
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void AddChargeTransferValidation(IPropertyGroup group, ValidationReport report)
        {
            var chargeTransfers = new List<double>(10);

            foreach (var statePair in group.GetPropertyStatePairs())
            {
                chargeTransfers.Add(statePair.AcceptorParticle.Charge - statePair.DonorParticle.Charge);
            }
            if (!chargeTransfers.All(value => ProjectServices.CommonNumerics.RangeComparer.Compare(chargeTransfers[0], value) == 0))
            {
                var detail = "The contained property state pairs of the provided group do not share a common charge exchange value";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail));
            }
        }
    }
}
