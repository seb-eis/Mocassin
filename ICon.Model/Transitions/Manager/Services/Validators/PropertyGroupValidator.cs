using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Particles;

namespace ICon.Model.Transitions.Validators
{
    /// <summary>
    /// Validator for new state exchange groups that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class PropertyGroupValidator : DataValidator<IStateExchangeGroup, BasicTransitionSettings, ITransitionDataPort>
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
        public override IValidationReport Validate(IStateExchangeGroup obj)
        {
            var report = new ValidationReport();
            if (!AddHasContentValidation(obj, report))
            {
                return report;
            }

            AddGenericObjectDuplicateValidation(obj, DataReader.Access.GetStateExchangeGroups(), report);
            return report;
        }

        /// <summary>
        /// Validates that the state exchange group contains at least one element of each required content element and adds the reults to the validation report (Returns false on failure)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected bool AddHasContentValidation(IStateExchangeGroup group, ValidationReport report)
        {
            if (group.StatePairCount == 0)
            {
                var detail = "The provided property group does not contain any state pairs and does not describe a valid set of state changes";
                report.AddWarning(ModelMessages.CreateMissingOrEmptyContentWarning(this, detail));
                return false;
            }
            return true;
        }
    }
}
