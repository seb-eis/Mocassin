using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

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
            if (!report.IsGood)
                return report;

            AddAbstractTransitionValidation(obj, report);
            AddTransitionGeometryValidation(obj, report);
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
                const string detail0 = "Size mismatch between abstract transition and selected geometry binding";
                var detail1 = $"Abstract defines ({groupCount}) positions, geometry defines ({transition.GeometryStepCount}) positions";
                report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0, detail1));
            }

            if (TransitionGeometryIsPlausible(transition, report)) 
                return;

            const string detail2 = "Transition geometry does not form a meaningful kinetic transition!";
            report.AddWarning(ModelMessageSource.CreateRestrictionViolationWarning(this, detail2));
        }

        /// <summary>
        ///     Checks if the passed kinetic transition has a plausible geometry binding that can form meaningful kinetic
        ///     transition and adds the found problems to the passed validation report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        protected bool TransitionGeometryIsPlausible(IKineticTransition transition, ValidationReport report)
        {
            var details = new List<string>();
            var unitCellProvider = ModelProject.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var cellReferencePositions = transition.GetGeometrySequence().Select(x => unitCellProvider.GetEntryValueAt(x)).ToList();

            AddExchangeGroupGeometryValidation(cellReferencePositions, transition.AbstractTransition.GetStateExchangeGroups().ToList(), report);
            if (!report.IsGood) return false;

            if (!cellReferencePositions[0].IsValidAndStable() || !cellReferencePositions[cellReferencePositions.Count - 1].IsValidAndStable())
            {
                const string detail0 = "Tailing positions of the transition are invalid or unstable!";
                details.Add(detail0);
            }

            if (cellReferencePositions.Any(x => !x.IsValidAndUnstable() && !x.IsValidAndStable()))
            {
                const string detail1 = "Geometry sequence contains deprecated unit cell positions!";
                details.Add(detail1);
            }

            for (var i = 1; i < cellReferencePositions.Count - 1; i++)
            {
                if (!cellReferencePositions[i].IsValidAndUnstable() || !cellReferencePositions[i + 1].IsValidAndUnstable()) 
                    continue;

                var detail2 = $"The geometry position contains consecutive unstable positions at positions ({i}) and ({i+1})";
                break;
            }

            if (cellReferencePositions.All(x => x.Stability != PositionStability.Unstable))
            {
                const string detail3 = "The geometry set does not contain any unstable position for the transition state";
                details.Add(detail3);
            }

            var analyzer = new TransitionAnalyzer();
            if (analyzer.ContainsRingTransition(transition.GetGeometrySequence(), ModelProject.SpaceGroupService.Comparer))
            {
                const string detail4 = "The transition geometry forms or contains a ring!";
                details.Add(detail4);
            }

            if (details.Count == 0)
                return true;

            report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, details.ToArray()));
            return false;
        }

        /// <summary>
        /// Validates that the set of passed exchange groups matches the set of binding unit cell positions and adds the results to the passed validation report
        /// </summary>
        /// <param name="cellReferencePositions"></param>
        /// <param name="stateExchangeGroups"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        protected void AddExchangeGroupGeometryValidation(IList<ICellReferencePosition> cellReferencePositions,
            IList<IStateExchangeGroup> stateExchangeGroups, ValidationReport report)
        {
            if (cellReferencePositions.Count != stateExchangeGroups.Count)
                return;

            var details = new List<string>();
            for (var i = 0; i < cellReferencePositions.Count; i++)
            {
                if (cellReferencePositions[i] == null)
                {
                    var detail1 = $"Position at geometry step ({i}) does not exist in the defined unit cell.";
                    details.Add(detail1);
                    continue;
                }

                if ((cellReferencePositions[i].IsValidAndStable() && !stateExchangeGroups[i].IsUnstablePositionGroup) ||
                    (cellReferencePositions[i].IsValidAndUnstable() && stateExchangeGroups[i].IsUnstablePositionGroup))
                    continue;

                var detail2 = $"Exchange group and position at geometry step ({i}) do not match in stability";
                details.Add(detail2);
            }

            if (details.Count == 0)
                return;

            report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, details.ToArray()));
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
                    var detail0 = $"Abstract defines {patternType} that does not support a transition state";
                    report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0));
                    break;

                case ConnectorPatternType.Undefined:
                    throw new InvalidOperationException("Unsupported transition pattern type previously passed validation");
            }
        }
    }
}