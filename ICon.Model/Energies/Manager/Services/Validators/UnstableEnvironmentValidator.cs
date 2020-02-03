using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies.Validators
{
    /// <summary>
    ///     Data validator for unstable environment objects that checks for potential conflicts with constraints and existing
    ///     data
    /// </summary>
    public class UnstableEnvironmentValidator : DataValidator<IUnstableEnvironment, MocassinEnergySettings, IEnergyDataPort>
    {
        /// <inheritdoc />
        public UnstableEnvironmentValidator(IModelProject modelProject, MocassinEnergySettings settings,
            IDataReader<IEnergyDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IUnstableEnvironment envInfo)
        {
            var report = new ValidationReport();

            AddObjectUniquenessValidation(envInfo, report);
            AddContentRestrictionValidation(envInfo, report);
            AddInteractionFilterValidation(envInfo, report);

            return report;
        }

        /// <summary>
        ///     Validates that the defined environment ist unique and does no already exist (Currently: Uses fact that only one
        ///     environment per UCP is allowed)
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddObjectUniquenessValidation(IUnstableEnvironment envInfo, ValidationReport report)
        {
            var objects = DataReader.Access.GetUnstableEnvironments().Where(value => !value.IsDeprecated);
            if (!objects.Select(value => value.CellReferencePosition).Contains(envInfo.CellReferencePosition))
                return;

            var detail0 = $"The unit cell position with index ({envInfo.CellReferencePosition.Index}) already has a defined environment";
            const string detail1 = "The generation and selection of multiple environments per position is currently not supported";
            report.AddWarning(ModelMessageSource.CreateModelDuplicateWarning(this, detail0, detail1));
        }

        /// <summary>
        ///     Validates that the object does not violated set content restrictions and adds the results to the validation report
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddContentRestrictionValidation(IUnstableEnvironment envInfo, ValidationReport report)
        {
            var approxInteractionCount = ApproximatePairInteractionCount(envInfo);

            if (envInfo.CellReferencePosition.Stability != PositionStability.Unstable)
            {
                const string detail0 = "Cannot define an unstable environment for a stable position";
                const string detail1 = "Stable environments are defined through one entity to avoid generation of 'energy holes' breaking the MMC routine";
                report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0, detail1));
            }

            if (Settings.PositionsPerUnstable.ParseValue(approxInteractionCount, out var warnings) >= 0) report.AddWarnings(warnings);
        }

                /// <summary>
        ///     Validates the interaction filters of the stable environments and adds the results to the validation report
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddInteractionFilterValidation(IUnstableEnvironment envInfo, ValidationReport report)
        {
            AddInteractionFilterRangeValidation(envInfo, report);
            AddInteractionFilterUniquenessValidation(envInfo, report);
        }

        /// <summary>
        /// Validates that the interaction filter definitions of the environment are unique
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddInteractionFilterUniquenessValidation(IUnstableEnvironment envInfo, ValidationReport report)
        {
            var duplicateIndices = envInfo.GetInteractionFilters()
                .ToList().RemoveDuplicatesAndGetRemovedIndices((a, b) => a.IsEqualFilter(b));

            var details = duplicateIndices
                .Select(index => $"Filter ({index}) is a duplicate of previous filter definition")
                .ToList();

            if (details.Count == 0)
                return;

            report.AddWarning(ModelMessageSource.CreateRedundantContentWarning(this, details.ToArray()));
        }

        /// <summary>
        /// Validates that the interaction filter definitions of the environment have valid range definitions
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddInteractionFilterRangeValidation(IUnstableEnvironment envInfo, ValidationReport report)
        {
            var index = 0;
            var constraint = new NumericConstraint(true, 0.0, envInfo.MaxInteractionRange, true, NumericComparer.Default());
            var details = new List<string>();
            foreach (var filter in envInfo.GetInteractionFilters())
            {
                if (!constraint.IsValid(filter.StartRadius) || !constraint.IsValid(filter.EndRadius))
                {
                    details.Add($"Range ({filter.StartRadius} to {filter.EndRadius}) of filter ({index}) is out of constraint {constraint}");
                }
                if (filter.StartRadius > filter.EndRadius)
                {
                    details.Add($"Start radius of filter ({index}) is below its end radius!");
                }

                index++;
            }

            if (details.Count == 0)
                return;

            report.AddWarning(ModelMessageSource.CreateRestrictionViolationWarning(this, details.ToArray()));
        }

        /// <summary>
        ///     Approximates how many interactions a radial search will produce in the worst case though the pair interaction
        ///     density of the unit cell when no filter has any effect
        /// </summary>
        /// <param name="envInfo"></param>
        /// <returns></returns>
        protected long ApproximatePairInteractionCount(IUnstableEnvironment envInfo)
        {
            long interactionPerUnitCell = ModelProject.GetManager<IStructureManager>().QueryPort
                .Query(port => port.GetExtendedIndexToPositionList())
                .Count(entry => entry.Stability == PositionStability.Stable);

            var unitCellVolume = ModelProject.GetManager<IStructureManager>().QueryPort
                .Query(port => port.GetVectorEncoder())
                .GetCellVolume();

            return (long) Math.Ceiling(interactionPerUnitCell * MocassinMath.GetSphereVolume(envInfo.MaxInteractionRange) / unitCellVolume);
        }
    }
}