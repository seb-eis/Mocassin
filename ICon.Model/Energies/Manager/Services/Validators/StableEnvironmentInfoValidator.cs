using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Operations;
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
            AddIgnoredPairCodesValidation(obj, report);
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
            var expectedPositionCount = ApproximateMaxInteractionsInInfluenceSphere(envInfo);
            if (Settings.PositionsPerStable.ParseValue(expectedPositionCount, out var warnings) != 0) report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates the ignored pair codes of the stable environments and adds the results to the validation report
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddIgnoredPairCodesValidation(IStableEnvironmentInfo envInfo, ValidationReport report)
        {
            var uniqueIgnoredPairs = new HashSet<SymmetricParticlePair>(envInfo.GetIgnoredPairs());

            if (envInfo.GetIgnoredPairs().Count() == uniqueIgnoredPairs.Count)
                return;

            const string detail = "The ignored pair code set contains duplicates";
            report.AddWarning(ModelMessageSource.CreateRedundantContentWarning(this, detail));
        }

        /// <summary>
        ///     Approximates how many interactions a radial search will produce in the worst case though the pair interaction
        ///     density of the unit cell
        /// </summary>
        /// <param name="envInfo"></param>
        /// <returns></returns>
        protected long ApproximateMaxInteractionsInInfluenceSphere(IStableEnvironmentInfo envInfo)
        {
            long interactionPerUnitCell = ModelProject.GetManager<IStructureManager>().QueryPort
                                              .Query(port => port.GetLinearizedExtendedPositionList())
                                              .Count(position => position.Status == PositionStatus.Stable) - 1;

            var unitCellVolume = ModelProject.GetManager<IStructureManager>().QueryPort
                .Query(port => port.GetVectorEncoder())
                .GetCellVolume();

            return (long) Math.Ceiling(interactionPerUnitCell * MocassinMath.GetSphereVolume(envInfo.MaxInteractionRange) / unitCellVolume);
        }
    }
}