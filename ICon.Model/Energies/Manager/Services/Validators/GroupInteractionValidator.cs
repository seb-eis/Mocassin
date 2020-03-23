using System;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Mathematics.Comparer;
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

            AddGeometricDuplicateValidation(group, report);
            AddEnvironmentConsistencyValidation(group, report);

            if (!report.IsGood) return report;

            AddContentRestrictionValidation(group, report);

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
            if (group.CenterCellReferencePosition.Stability != PositionStability.Unstable &&
                group.CenterCellReferencePosition.Stability != PositionStability.Stable)
            {
                var detail0 = $"The position status type ({group.CenterCellReferencePosition.Stability.ToString()}) is not supported for grouping";
                var detail1 = $"Supported types are ({PositionStability.Stable.ToString()}) and ({PositionStability.Unstable.ToString()})";
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
                .Where(value => !value.IsDeprecated && value.CenterCellReferencePosition == group.CenterCellReferencePosition)
                .Select(value => (value, value.GetSurroundingGeometry().ToArray()));

            foreach (var (otherGroup, geometry) in currentData)
            {
                Array.Sort(geometry, comparer);
                if (geometry.LexicographicCompare(group.GetSurroundingGeometry(), comparer) != 0)
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

            var geometryValidity = GetGroupEnvironmentPositionValidity(group);
            if (geometryValidity == GroupGeometryValidity.IsValid)
                return;

            var detail1 = $"Group geometry has the following validity error flag [{geometryValidity.ToString()}]";
            const string detail2 = "Note 1: Group geometry members have to be defined and stable except for the center!";
            const string detail3 = "Note 2: Group geometry members have to be unaffected by interaction filters of the center!";
            report.AddWarning(ModelMessageSource.CreateContentMismatchWarning(this, detail1, detail2, detail3));
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
            return permutationSource.PermutationCount * group.CenterCellReferencePosition.OccupationSet.ParticleCount;
        }

        /// <summary>
        ///     Finds the longest vector of the group geometry and returns the group maximum radius in internal units
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected double GetMaxGroupRange(IGroupInteraction group)
        {
            var transformer = ModelProject.CrystalSystemService.VectorTransformer;
            var start = group.CenterCellReferencePosition.AsPosition().Vector;

            return group.GetSurroundingGeometry()
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

            var envRange = envDef switch
            {
                IStableEnvironmentInfo stable => stable.MaxInteractionRange,
                IUnstableEnvironment unstable => unstable.MaxInteractionRange,
                _ => throw new ArgumentException("Group has unknown or null environment definition")
            };

            return envRange;
        }

        /// <summary>
        ///     Determines if a group contains any forbidden or non existing positions and returns the affiliated
        ///     <see cref="GroupGeometryValidity" />
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected GroupGeometryValidity GetGroupEnvironmentPositionValidity(IGroupInteraction group)
        {
            var ucProvider = ModelProject.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var analyzer = new GeometryGroupAnalyzer(ucProvider, ModelProject.SpaceGroupService);

            switch (group.CenterCellReferencePosition.Stability)
            {
                case PositionStability.Stable:
                    var stableFilters = DataReader.Access
                        .GetStableEnvironmentInfo()
                        .GetInteractionFilters()
                        .ToList();

                    return analyzer.CheckGroupGeometryValidity(group, stableFilters);

                case PositionStability.Unstable:
                    var unstableFilters = DataReader.Access
                        .GetUnstableEnvironment(group.CenterCellReferencePosition)
                        .GetInteractionFilters()
                        .ToList();

                    return analyzer.CheckGroupGeometryValidity(group, unstableFilters);

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
            return group.CenterCellReferencePosition.Stability switch
            {
                PositionStability.Stable => (object) DataReader.Access.GetStableEnvironmentInfo(),
                PositionStability.Unstable => DataReader.Access.GetUnstableEnvironments()
                    .SingleOrDefault(value => value.CellReferencePosition == group.CenterCellReferencePosition),
                _ => null
            };
        }
    }
}