﻿using System.Text.RegularExpressions;

using ICon.Framework.Operations;
using ICon.Framework.Messaging;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.Validators
{
    /// <summary>
    /// Validator for new kinetic transitions that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class KineticTransitionValidator : DataValidator<IKineticTransition, BasicTransitionSettings, ITransitionDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public KineticTransitionValidator(IProjectServices projectServices, BasicTransitionSettings settings, IDataReader<ITransitionDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate the passed object in terms of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IKineticTransition obj)
        {
            var report = new ValidationReport();
            if (!AddHasContentValidation(obj, report))
            {
                return report;
            }
            AddTransitionGeometryValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validates that a kinetic transition is not empty in terms of required model content sequences and adds the results to the validation report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        protected bool AddHasContentValidation(IKineticTransition transition, ValidationReport report)
        {
            if (transition.GeometryStepCount == 0)
            {
                var detail = "The provided kinetic transition contains no geometry information and cannot describe a valid transition";
                report.AddWarning(ModelMessages.CreateMissingOrEmptyContentWarning(this, detail));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates the transition geometry of a kinetic transition in terms of match with the abstract transition and basic shape of the geoemtry and adds the results to the validation report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void AddTransitionGeometryValidation(IKineticTransition transition, ValidationReport report)
        {
            int groupCount = transition.AbstractTransition.StateCount;
            if (transition.GeometryStepCount != groupCount)
            {
                var detail0 = "The abstract transition and the selected geometry set for the kinetic transition do not match in transition size";
                var detail1 = $"Abstract transition defines ({groupCount}) positions while the geoemtry set defines ({transition.GeometryStepCount}) positions";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0, detail1));
            }
            if (new TransitionAnalyzer().ContainsRingTransition(transition.GetGeometrySequence(), ProjectServices.SpaceGroupService.Comparer))
            {
                var detail = "The transition geometry contains a ring transition where one position is contained multiple times";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail));
            }
        }
    }
}
