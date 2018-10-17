using System;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies.Validators
{
    /// <summary>
    ///     Data validator for group interaction definitions in terms of conflicts with existing data or restriction violations
    /// </summary>
    public class GroupInteractionValidator : DataValidator<IGroupInteraction, MocassinEnergySettings, IEnergyDataPort>
    {
        /// <inheritdoc />
        public GroupInteractionValidator(IModelProject modelProject, MocassinEnergySettings settings,
            IDataReader<IEnergyDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IGroupInteraction group)
        {
            var report = new ValidationReport();

            if (!AddDefinedPositionValidation(group, report))
                return report;

            AddContentRestrictionValidation(group, report);
            AddGeometricDuplicateValidation(group, report);
            AddEnvironmentConsistencyValidation(group, report);

            return report;
        }

        /// <summary>
        ///     Validates that the group is defined for a supported position type and an environment definition exists for the
        ///     position
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected bool AddDefinedPositionValidation(IGroupInteraction group, ValidationReport report)
        {
            if (group.CenterUnitCellPosition.Status != PositionStatus.Unstable &&
                group.CenterUnitCellPosition.Status != PositionStatus.Stable)
            {
                var detail0 = $"The position status type ({group.CenterUnitCellPosition.Status.ToString()}) is not supported for grouping";
                var detail1 = $"Supported types are ({PositionStatus.Stable.ToString()}) and ({PositionStatus.Unstable.ToString()})";
                report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0, detail1));
            }

            if (GetGroupAffiliatedEnvironment(group) != null)
                return report.IsGood;

            const string detail2 = "The affiliated environment has to be defined before a group interaction can be defined";
            report.AddWarning(ModelMessageSource.CreateMissingOrEmptyContentWarning(this, detail2));

            return report.IsGood;
        }

        /// <summary>
        ///     Validates a group interaction in terms of conflicts with the defined content restrictions and adds the result to
        ///     the report
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void AddContentRestrictionValidation(IGroupInteraction group, ValidationReport report)
        {
            if (Settings.AtomsPerGroup.ParseValue(group.GroupSize, out var warnings) != 0)
                report.AddWarnings(warnings);

            if (Settings.PermutationsPerGroup.ParseValue(GetGroupPermutationCount(group), out warnings) != 0)
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates if a symmetry equivalent group already exists in the current data and adds the result to the report
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void AddGeometricDuplicateValidation(IGroupInteraction group, ValidationReport report)
        {
            var comparer = new VectorComparer3D<Fractional3D>(ModelProject.GeometryNumeric.RangeComparer);
            var currentData = DataReader.Access.GetGroupInteractions()
                .Where(value => !value.IsDeprecated && value.CenterUnitCellPosition == group.CenterUnitCellPosition)
                .Select(value => (value, value.GetBaseGeometry().ToArray()));

            foreach (var (otherGroup, geometry) in currentData)
            {
                Array.Sort(geometry, comparer);
                if (geometry.LexicographicCompare(group.GetBaseGeometry(), comparer) != 0)
                    continue;

                var detail0 = $"Group interaction has identical geometry to interaction at index ({otherGroup.Index})";
                const string detail1 =
                    "Double definition of the equivalent group is not supported as it does not provided any useful modeling extension";
                report.AddWarning(ModelMessageSource.CreateModelDuplicateWarning(this, detail0, detail1));
                return;
            }
        }

        /// <summary>
        ///     Validates that the group geometry is consistent with the start positions environment description and adds the
        ///     result to the report
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void AddEnvironmentConsistencyValidation(IGroupInteraction group, ValidationReport report)
        {
            var groupRange = GetMaxGroupRange(group);
            var envRange = GetGroupEnvironmentRange(group);

            if (ModelProject.GeometryNumeric.RangeComparer.Compare(groupRange, envRange) > 0)
            {
                var detail0 =
                    $"The group max vector range is ({groupRange}) while the affiliated environment interaction range is ({envRange})";
                report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail0));
            }

            if (!GroupContainsNonEnvironmentPositions(group))
                return;

            const string detail1 = "The group contains positions that form ignored pair interactions with the center position";
            const string detail2 = "Groups can only contain positions that are also defined as pair interactions with the center";
            report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail1, detail2));
        }

        /// <summary>
        ///     Calculates the total number of permutations (not symmetry filtered, including center position) that the group
        ///     describes
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected long GetGroupPermutationCount(IGroupInteraction group)
        {
            var unitCellProvider = ModelProject.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var permutationSource = new GeometryGroupAnalyzer(unitCellProvider, ModelProject.SpaceGroupService)
                .GetGroupStatePermutationSource(group);
            return permutationSource.PermutationCount * group.CenterUnitCellPosition.OccupationSet.ParticleCount;
        }

        /// <summary>
        ///     Finds the longest vector of the group geometry and returns the group maximum radius in internal units
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected double GetMaxGroupRange(IGroupInteraction group)
        {
            var transformer = ModelProject.CrystalSystemService.VectorTransformer;
            var start = group.CenterUnitCellPosition.AsPosition().Vector;

            return group.GetBaseGeometry()
                .Select(vector => transformer.ToCartesian(vector - start).GetLength())
                .Concat(new[] {0.0})
                .Max();
        }

        /// <summary>
        ///     Determines which environment information belongs to the group and determines the interaction range
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected double GetGroupEnvironmentRange(IGroupInteraction group)
        {
            var envDef = GetGroupAffiliatedEnvironment(group);
            double envRange;

            switch (envDef)
            {
                case IStableEnvironmentInfo stable:
                    envRange = stable.MaxInteractionRange;
                    break;

                case IUnstableEnvironment unstable:
                    envRange = unstable.MaxInteractionRange;
                    break;

                default:
                    throw new ArgumentException("Group has unknown or null environment definition");
            }

            return envRange;
        }

        /// <summary>
        ///     Determines if a group contains any forbidden position that are not defined within the set of pair interactions of
        ///     the environment
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected bool GroupContainsNonEnvironmentPositions(IGroupInteraction group)
        {
            var ucProvider = ModelProject.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var analyzer = new GeometryGroupAnalyzer(ucProvider, ModelProject.SpaceGroupService);

            switch (group.CenterUnitCellPosition.Status)
            {
                case PositionStatus.Stable:
                    var ignoredPairs = DataReader.Access.GetStableEnvironmentInfo()
                        .GetIgnoredPairs()
                        .ToList();

                    return analyzer.GetAllGroupPairs(group).Any(value => ignoredPairs.Contains(value));

                case PositionStatus.Unstable:
                    var ignoredPos = DataReader.Access.GetUnstableEnvironment(group.CenterUnitCellPosition.Index)
                        .GetIgnoredPositions()
                        .ToList();

                    return analyzer.GetGroupUnitCellPositions(group).Any(value => ignoredPos.Contains(value));

                case PositionStatus.Undefined:
                    throw new InvalidOperationException("Undefined position reached environment check");

                default:
                    throw new InvalidOperationException("Undefined position reached environment check");
            }
        }

        /// <summary>
        ///     Determines which environment belongs to the group. Returns null if the environment is not yet defined or not
        ///     supported
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected object GetGroupAffiliatedEnvironment(IGroupInteraction group)
        {
            switch (group.CenterUnitCellPosition.Status)
            {
                case PositionStatus.Stable:
                    return DataReader.Access.GetStableEnvironmentInfo();

                case PositionStatus.Unstable:
                    return DataReader.Access
                        .GetUnstableEnvironments()
                        .SingleOrDefault(value => value.UnitCellPosition == group.CenterUnitCellPosition);

                case PositionStatus.Undefined:
                    return null;
                default:
                    return null;
            }
        }
    }
}