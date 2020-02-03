using System;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.ConflictHandling
{
    /// <summary>
    ///     Resolves a change in the space group information and corrects the dependent internal data of the structure object
    /// </summary>
    public class SpaceGroupChangeHandler : ObjectConflictHandler<SpaceGroupInfo, StructureModelData>
    {
        /// <inheritdoc />
        public SpaceGroupChangeHandler(IDataAccessor<StructureModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(SpaceGroupInfo groupInfo)
        {
            if (!ModelProject.SpaceGroupService.TryLoadGroup(groupInfo.GroupEntry))
                throw new InvalidOperationException("Space group loading to service failed");

            var report = new ConflictReport();
            MatchCrystalSystemAndCellParameters(report);
            MatchCellReferencePositionsToSpaceGroup(report);
            return report;
        }

        /// <summary>
        ///     Matches the crystal system to the space group and checks if the current cell parameters can be applied (Loads
        ///     default values of the new system if not possible)
        /// </summary>
        /// <param name="report"></param>
        protected void MatchCrystalSystemAndCellParameters(ConflictReport report)
        {
            var oldParameters = DataAccess.Query(data => data.CrystalParameters);
            if (!ModelProject.CrystalSystemService.LoadNewSystem(ModelProject.SpaceGroupService.LoadedGroup))
                return;

            if (!ModelProject.CrystalSystemService.TrySetParameters(oldParameters.ParameterSet))
            {
                OverwriteCellParameters(ModelProject.CrystalSystemService.CrystalSystem.GetDefaultParameterSet());

                const string detail0 = "The original cell parameters are no longer compatible with the new space group";
                const string detail1 = "Conflict resolved by loading a default parameter set for the new crystal system";
                report.Warnings.Add(ModelMessageSource.CreateContentResetWarning(this, detail0, detail1));
            }

            oldParameters.ParameterSet = ModelProject.CrystalSystemService.GetCurrentParameterSet();
        }

        /// <summary>
        ///     Checks the unit cell position list from first to last and marks later duplicates as deprecated if they produce equal sequences to former positions
        /// </summary>
        /// <param name="report"></param>
        protected void MatchCellReferencePositionsToSpaceGroup(ConflictReport report)
        {
            var currentPositions = DataAccess.Query(data => data.CellReferencePositions.Select(position => position.Vector.AsFractional()));
            var extendedPositions = ModelProject.SpaceGroupService.GetUnitCellP1PositionExtensions(currentPositions);

            var comparer = ModelProject.SpaceGroupService.Comparer.ToEqualityComparer();

            for (var i = 0; i < extendedPositions.Count; i++)
            {
                var i1 = i;
                if (!DataAccess.Query(data => data.CellReferencePositions[i1].IsDeprecated))
                {
                    for (var j = i + 1; j < extendedPositions.Count; j++)
                    {
                        if (!extendedPositions[i].SequenceEqual(extendedPositions[j], comparer))
                            continue;

                        var j1 = j;
                        DataAccess.Query(data => data.CellReferencePositions[j1].Deprecate());

                        var detail0 = $"The unit cell position at index ({j}) is now equivalent to position at index ({i})";
                        var detail1 = $"Conflict was resolved by marking position at index ({j}) as deprecated";
                        report.Warnings.Add(ModelMessageSource.CreateConflictHandlingWarning(this, detail0, detail1));
                    }
                }
            }
        }

        /// <summary>
        ///     Overwrites the existing cell parameters by a new set both in the data and project services
        /// </summary>
        /// <param name="parameters"></param>
        protected void OverwriteCellParameters(CellParameters parameters)
        {
            if (!ModelProject.CrystalSystemService.TrySetParameters(parameters.ParameterSet))
                throw new InvalidOperationException("Function was called with an invalid parameter set");

            DataAccess.Query(data => data.CrystalParameters = parameters);
        }
    }
}