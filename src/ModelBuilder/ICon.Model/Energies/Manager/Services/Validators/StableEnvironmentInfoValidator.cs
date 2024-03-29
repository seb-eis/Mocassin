﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies.Validators
{
    /// <summary>
    ///     Data validator for stable environment info parameters that checks for potential conflicts with constraints and
    ///     existing data
    /// </summary>
    public class StableEnvironmentInfoValidator : DataValidator<IStableEnvironmentInfo, MocassinEnergySettings, IEnergyDataPort>
    {
        /// <inheritdoc />
        public StableEnvironmentInfoValidator(IModelProject modelProject, MocassinEnergySettings settings,
            IDataReader<IEnergyDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IStableEnvironmentInfo obj)
        {
            var report = new ValidationReport();
            AddGenericContentEqualityValidation(obj, DataReader.Access.GetStableEnvironmentInfo(), report);
            AddInteractionRangeValidation(obj, report);
            AddInteractionFilterValidation(obj, report);
            AddDefectBackgroundValidation(obj, report);
            return report;
        }

        /// <summary>
        ///     Validates that the interaction range does not lead to position count violations and adds the results to the
        ///     validation report
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddInteractionRangeValidation(IStableEnvironmentInfo envInfo, ValidationReport report)
        {
            var expectedPositionCount = ApproximateWorstCaseInteractionCount(envInfo);
            if (Settings.PositionsPerStable.ParseValue(expectedPositionCount, out var warnings) != 0) report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates the interaction filters of the stable environments and adds the results to the validation report
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddInteractionFilterValidation(IStableEnvironmentInfo envInfo, ValidationReport report)
        {
            AddInteractionFilterRangeValidation(envInfo, report);
            AddInteractionFilterUniquenessValidation(envInfo, report);
        }

        /// <summary>
        ///     Validates that the interaction filter definitions of the environment are unique
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddInteractionFilterUniquenessValidation(IStableEnvironmentInfo envInfo, ValidationReport report)
        {
            var duplicateIndices = envInfo.GetInteractionFilters()
                                          .ToList().RemoveDuplicatesAndGetRemovedIndices((a, b) => a.IsEqualFilter(b));

            var details = duplicateIndices
                          .Select(index => $"Filter ({index}) is a duplicate of previous filter definition.")
                          .ToList();

            if (details.Count == 0)
                return;

            report.AddWarning(ModelMessageSource.CreateRedundantContentWarning(this, details.ToArray()));
        }

        /// <summary>
        ///     Validates that the interaction filter definitions of the environment have valid range definitions
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddInteractionFilterRangeValidation(IStableEnvironmentInfo envInfo, ValidationReport report)
        {
            var index = 0;
            var constraint = new NumericConstraint(true, 0.0, envInfo.MaxInteractionRange, true, NumericComparer.Default());
            var details = new List<string>();
            foreach (var filter in envInfo.GetInteractionFilters())
            {
                if (!constraint.IsValid(filter.StartRadius) || !constraint.IsValid(filter.EndRadius))
                    details.Add($"Range ({filter.StartRadius} to {filter.EndRadius}) of filter ({index}) is out of constraint {constraint}");
                if (filter.StartRadius > filter.EndRadius) details.Add($"Start radius of filter ({index}) is below its end radius!");

                index++;
            }

            if (details.Count == 0)
                return;

            report.AddWarning(ModelMessageSource.CreateRestrictionViolationWarning(this, details.ToArray()));
        }

        /// <summary>
        ///     Approximates how many interactions a radial search will produce in the worst case through the pair interaction
        ///     density of the unit cell
        /// </summary>
        /// <param name="envInfo"></param>
        /// <returns></returns>
        protected long ApproximateWorstCaseInteractionCount(IStableEnvironmentInfo envInfo)
        {
            long interactionPerUnitCell = ModelProject.Manager<IStructureManager>().DataAccess
                                                      .Query(port => port.GetLinearizedExtendedPositionList())
                                                      .Count(position => position.Stability == PositionStability.Stable) - 1;

            interactionPerUnitCell = Math.Max(interactionPerUnitCell, 1);

            var unitCellVolume = ModelProject.Manager<IStructureManager>().DataAccess
                                             .Query(port => port.GetVectorEncoder())
                                             .GetCellVolume();

            return (long) Math.Ceiling(interactionPerUnitCell * MocassinMath.GetSphereVolume(envInfo.MaxInteractionRange) / unitCellVolume);
        }

        /// <summary>
        ///     Validates the collection of <see cref="DefectEnergy" /> energies  in the <see cref="IStableEnvironmentInfo" /> and
        ///     adds the results to the report
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddDefectBackgroundValidation(IStableEnvironmentInfo envInfo, ValidationReport report)
        {
            var details = new List<string>();
            foreach (var defectEnergy in envInfo.GetDefectEnergies())
            {
                if (envInfo.GetDefectEnergies().SkipWhile(x => x != defectEnergy).Count(x => x.Equals(defectEnergy)) != 1)
                    details.Add($"Defect [{defectEnergy.Particle}] @ [{defectEnergy.CellSite}] has multiple definitions.");

                if (defectEnergy.CellSite.IsValidAndStable() && defectEnergy.Particle.IsVoid)
                    details.Add($"Defect [{defectEnergy.Particle}] (void) @ [{defectEnergy.CellSite}] (stable) has no effect.");

                if (double.IsInfinity(defectEnergy.Energy) || double.IsNaN(defectEnergy.Energy))
                    details.Add($"Defect [{defectEnergy.Particle}] @ [{defectEnergy.CellSite}] has an infinity/NaN energy value.");
            }

            if (details.Count != 0)
                report.AddWarning(ModelMessageSource.CreateNotRecommendedWarning(this, details.ToArray()));
        }
    }
}