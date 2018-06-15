using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Collections;
using ICon.Mathematics.Extensions;
using ICon.Model.ProjectServices;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Model.Particles;
using ICon.Framework.Operations;

namespace ICon.Model.Energies.Validators
{
    /// <summary>
    /// Data validator for stable environment info parameters that checks for potential conflicts with constraints and existing data
    /// </summary>
    public class StableEnvironmentInfoValidator : DataValidator<IStableEnvironmentInfo, BasicEnergySettings, IEnergyDataPort>
    {
        /// <summary>
        /// Creates new validator with project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public StableEnvironmentInfoValidator(IProjectServices projectServices, BasicEnergySettings settings, IDataReader<IEnergyDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {

        }

        /// <summary>
        /// Validates a stable group info object in terms of compatibility with existing data and basic constraints
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IStableEnvironmentInfo obj)
        {
            var report = new ValidationReport();
            AddGenericContentEqualityValidation(obj, DataReader.Access.GetStableEnvironmentInfo(), report);
            AddInteractionRangeValidation(obj, report);
            AddIgnoredPairCodesValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validates that the interaction range does not lead to position count violations and adds the results to the validation report
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddInteractionRangeValidation(IStableEnvironmentInfo envInfo, ValidationReport report)
        {
            var expectedPositionCount = AppoximateMaxInteractionsInInfluenceSphere(envInfo);
            if (expectedPositionCount > Settings.EnvironmentPositionWarningLimit)
            {
                var detail0 = $"The expected maximum position environment count ({expectedPositionCount}) exceeds the warning limit ({Settings.EnvironmentPositionWarningLimit})";
                var detail1 = $"The simulation cycle rate may be low";
                report.AddWarning(ModelMessages.CreateWarningLimitReachedWarning(this, detail0, detail1));
            }
            if (expectedPositionCount > Settings.MaxStableEnvironmentPositionCount)
            {
                var details = $"The expected maximum position environment count {expectedPositionCount} exceeds the limit {Settings.MaxStableEnvironmentPositionCount}";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, details));
            }
        }

        /// <summary>
        /// Validates the ignored pair codes of the stable environments and adds the results to the validation report
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddIgnoredPairCodesValidation(IStableEnvironmentInfo envInfo, ValidationReport report)
        {
            var uniqueIgnoredPairs = new HashSet<SymParticlePair>(envInfo.GetIgnoredPairs());

            if (envInfo.GetIgnoredPairs().Count() != uniqueIgnoredPairs.Count())
            {
                var detail = $"The ignored pair code set contains duplicates";
                report.AddWarning(ModelMessages.CreateRedundantContentWarning(this, detail));
            }
        }

        /// <summary>
        /// Approximates how many interactions a radial search will produce in the worst case though the pair interaction density of the unit cell
        /// </summary>
        /// <param name="envInfo"></param>
        /// <returns></returns>
        protected long AppoximateMaxInteractionsInInfluenceSphere(IStableEnvironmentInfo envInfo)
        {
            long interactionPerUnitCell = ProjectServices.GetManager<IStructureManager>().QueryPort
                .Query(port => port.GetLinearizedExtendedPositionList())
                .Where(position => position.Status == PositionStatus.Stable)
                .Count() - 1;

            double unitCellVolume = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetVectorEncoder()).GetCellVolume();
            return (long)Math.Ceiling(interactionPerUnitCell * ExtMath.GetSphereVolume(envInfo.MaxInteractionRange) / unitCellVolume);
        }
    }
}
