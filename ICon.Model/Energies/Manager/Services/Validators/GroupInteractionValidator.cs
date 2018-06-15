using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Model.ProjectServices;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Framework.Operations;
using ICon.Framework.Extensions;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Energies.Validators
{
    /// <summary>
    /// Data validator for group interaction definitions in terms of conflicts with existing data or restriction violations
    /// </summary>
    public class GroupInteractionValidator : DataValidator<IGroupInteraction, BasicEnergySettings, IEnergyDataPort>
    {
        /// <summary>
        /// Creates new validator with project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public GroupInteractionValidator(IProjectServices projectServices, BasicEnergySettings settings, IDataReader<IEnergyDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {

        }

        /// <summary>
        /// Validates a group info object in terms of compatibility with existing data and basic constraints
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IGroupInteraction group)
        {
            var report = new ValidationReport();

            if (!AddDefinedPositionValidation(group, report))
            {
                return report;
            }
            AddContentRestrictionValidation(group, report);
            AddGeometricDuplicateValidation(group, report);
            AddEnvironmentConsistencyValidation(group, report);

            return report;
        }

        /// <summary>
        /// Validates that the group is defined for a supported position type and an environment definition exists for the position
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected bool AddDefinedPositionValidation(IGroupInteraction group, ValidationReport report)
        {
            if (group.UnitCellPosition.Status != PositionStatus.Unstable && group.UnitCellPosition.Status != PositionStatus.Stable)
            {
                var detail0 = $"The position status type ({group.UnitCellPosition.Status.ToString()}) is not supported for grouping";
                var detail1 = $"Supported types are ({PositionStatus.Stable.ToString()}) and ({PositionStatus.Unstable.ToString()})";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0, detail1));
            }

            if (GetGroupAffiliatedEnvironment(group) == null)
            {
                var detail0 = $"The affiliated environment has to be defined before a group interaction can be defined";
                report.AddWarning(ModelMessages.CreateMissingOrEmptyContentWarning(this, detail0));
            }

            return report.IsGood;
        }

        /// <summary>
        /// Validates a group interaction in terms of conflicts with the defined content restrictions and adds the result to the report
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void AddContentRestrictionValidation(IGroupInteraction group, ValidationReport report)
        {
            if (group.GroupSize > Settings.MaxGroupingSize)
            {
                var detail0 = $"Maximum group position count violated with ({group.GroupSize}) of ({Settings.MaxGroupingSize}) allowed positions";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }

            long permCount = GetGroupPermutationCount(group);
            if (permCount > Settings.MaxGroupPermutationCount)
            {
                var detail0 = $"Maximum permutation count violated with ({permCount}) of ({Settings.MaxGroupPermutationCount}) allowed permutations";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validates if a symmetry equivalent group already exists in the current data and adds the result to the report
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void AddGeometricDuplicateValidation(IGroupInteraction group, ValidationReport report)
        {
            var comparer = new VectorComparer3D<Fractional3D>(ProjectServices.GeometryNumerics.RangeComparer);
            var currentData = DataReader.Access.GetGroupInteractions()
                .Where(value => !value.IsDeprecated && value.UnitCellPosition == group.UnitCellPosition)
                .Select(value => (value, value.GetBaseGeometry().ToArray()));

            foreach (var (otherGroup, geometry) in currentData)
            {
                Array.Sort(geometry, comparer);
                if (geometry.LexicographicCompare(group.GetBaseGeometry(), comparer) == 0)
                {
                    var detail0 = $"Group interaction has identical geometry to interaction at index ({otherGroup.Index})";
                    var detail1 = $"Double definition of the equivalent group is not supported as it does not provided any usefull modelling extension";
                    report.AddWarning(ModelMessages.CreateModelDuplicateWarning(this, detail0, detail1));

                    return;
                }
            }
        }

        /// <summary>
        /// Validates that the group geometry is consistent with the start positions environment description and adds the result to the report
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void AddEnvironmentConsistencyValidation(IGroupInteraction group, ValidationReport report)
        {
            double groupRange = GetMaxGroupRange(group);
            double envRange = GetGroupEnvironmentRange(group);

            if (ProjectServices.GeometryNumerics.RangeComparer.Compare(groupRange, envRange) > 0)
            {
                var detail0 = $"The group max vector range is ({groupRange}) while the affiliated environment interaction range is ({envRange})";
                report.AddWarning(ModelMessages.CreateWarningLimitReachedWarning(this, detail0));
            }
        }

        /// <summary>
        /// Calculates the total number of permutations (not symmetry filtered, including center position) that the group describes
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected long GetGroupPermutationCount(IGroupInteraction group)
        {
            var unitCellProvider = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var permuter = new GeometryGroupAnalyzer(unitCellProvider, ProjectServices.SpaceGroupService).GetGroupStatePermuter(group);
            return permuter.PermutationCount * group.UnitCellPosition.OccupationSet.ParticleCount;
        }

        /// <summary>
        /// Finds the longest vector of the group geometry and returns the group maximum radius in internal units
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected double GetMaxGroupRange(IGroupInteraction group)
        {
            var transformer = ProjectServices.CrystalSystemService.VectorTransformer;
            var start = group.UnitCellPosition.AsPosition().Vector;
            double maxDistance = 0.0;

            foreach (var distance in group.GetBaseGeometry().Select(vector => transformer.CartesianFromFractional(vector - start).GetLength()))
            {
                maxDistance = (maxDistance < distance) ? distance : maxDistance;
            }

            return maxDistance;
        }

        /// <summary>
        /// Determines which environment information belongs to the group and determines the interaction range
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected double GetGroupEnvironmentRange(IGroupInteraction group)
        {
            var envDef = GetGroupAffiliatedEnvironment(group);
            double envRange = 0.0;

            switch (envDef)
            {
                case IStableEnvironmentInfo stable when stable != null:
                    envRange = stable.MaxInteractionRange;
                    break;

                case IUnstableEnvironment unstable when unstable != null:
                    envRange = unstable.MaxInteractionRange;
                    break;

                default:
                    throw new ArgumentException("Group has unknown or null environment definition");
            }
            return envRange;
        }

        /// <summary>
        /// Determines if a group contains
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected bool GroupContainsNonEnvironmentPositions(IGroupInteraction group)
        {
            return false;
        }

        /// <summary>
        /// Determines which environment belongs to the group. Returns null if the environment is not yet defined or not supported
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected object GetGroupAffiliatedEnvironment(IGroupInteraction group)
        {
            switch (group.UnitCellPosition.Status)
            {
                case PositionStatus.Stable:
                    return DataReader.Access.GetStableEnvironmentInfo();

                case PositionStatus.Unstable:
                    return DataReader.Access.GetUnstableEnvironments()
                        .Where(value => value.UnitCellPosition == group.UnitCellPosition)
                        .SingleOrDefault();

                default:
                    return null;
            }
        }
    }
}
