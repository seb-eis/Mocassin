using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Operations;
using ICon.Mathematics.Extensions;
using ICon.Model.Structures;
using ICon.Model.ProjectServices;
using ICon.Model.Basic;

namespace ICon.Model.Energies.Validators
{
    /// <summary>
    /// Data validator for unstable environment objects that checks for potential conflicts with constraints and existing data
    /// </summary>
    public class UnstableEnvironmentValidator : DataValidator<IUnstableEnvironment, BasicEnergySettings, IEnergyDataPort>
    {
        /// <summary>
        /// Creates new validator with project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public UnstableEnvironmentValidator(IProjectServices projectServices, BasicEnergySettings settings, IDataReader<IEnergyDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {

        }

        /// <summary>
        /// Validates a stable group info object in terms of compatibility with existing data and basic constraints
        /// </summary>
        /// <param name="envInfo"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IUnstableEnvironment envInfo)
        {
            var report = new ValidationReport();

            AddObjectUniquenessValidation(envInfo, report);
            AddContentRestrictionValidation(envInfo, report);
            AddGroupInteractionValidation(envInfo, report);

            return report;
        }

        /// <summary>
        /// Validates that the defined environment ist unique and does no already exist (Currently: Uses fact that only one environment per UCP is allowed)
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddObjectUniquenessValidation(IUnstableEnvironment envInfo, ValidationReport report)
        {
            var objects = DataReader.Access.GetUnstableEnvironments().Where(value => !value.IsDeprecated);
            if (objects.Select(value => value.UnitCellPosition).Contains(envInfo.UnitCellPosition))
            {
                var detail0 = $"The unit cell position with index ({envInfo.UnitCellPosition.Index}) already has a defined environment";
                var detail1 = $"The generation and selection of multiple environments per position is currently not supported";
                report.AddWarning(ModelMessages.CreateModelDuplicateWarning(this, detail0, detail1));
            }
        }

        /// <summary>
        /// Validates that the object does not violated set content restrictions and adds the results to the validation report
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddContentRestrictionValidation(IUnstableEnvironment envInfo, ValidationReport report)
        {
            long approxInteractionCount = ApproximatePairInteractionCount(envInfo);

            if (envInfo.UnitCellPosition.Status != PositionStatus.Unstable)
            {
                var detail0 = "Cannot define an unstable environment for a stable position";
                var detail1 = "Stable environments are defined through one entity to avoid generation of 'energy holes' breaking the MMC routine";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0, detail1));
            }

            if (Settings.PositionsPerUnstable.ParseValue(approxInteractionCount, out var warnings) >= 0)
            {
                report.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates that the group interactions specfified with the environment match the environment
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void AddGroupInteractionValidation(IUnstableEnvironment envInfo, ValidationReport report)
        {
            foreach (var interaction in envInfo.GetGroupInteractions().Where(value => value.CenterUnitCellPosition != envInfo.UnitCellPosition))
            {
                var detail0 = $"The group interaction with index ({interaction.Index}) cannot be applied to this environment";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
            }
        }

        /// <summary>
        /// Approximates how many interactions a radial search will produce in the worst case though the pair interaction density of the unit cell
        /// and the ignored position information of the environment
        /// </summary>
        /// <param name="envInfo"></param>
        /// <returns></returns>
        protected long ApproximatePairInteractionCount(IUnstableEnvironment envInfo)
        {
            long interactionPerUnitCell = ProjectServices.GetManager<IStructureManager>().QueryPort
                .Query(port => port.GetExtendedIndexToPositionDictionary())
                .Where(entry => entry.Value.Status == PositionStatus.Stable && !envInfo.GetIgnoredPositions().Contains(entry.Value))
                .Count();

            double unitCellVolume = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetVectorEncoder()).GetCellVolume();
            return (long)Math.Ceiling(interactionPerUnitCell * ExtMath.GetSphereVolume(envInfo.MaxInteractionRange) / unitCellVolume);
        }
    }
}
