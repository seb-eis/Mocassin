using System;
using System.Linq;

using ICon.Framework.Extensions;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.ConflictHandling
{
    /// <summary>
    /// Resolves a change in the space group information and corrects the dependent internal data of the structure object
    /// </summary>
    public class SpaceGroupChangeHandler : ObjectConflictHandler<SpaceGroupInfo, StructureModelData>
    {
        /// <summary>
        /// Create new space group change handler with the provided data access and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public SpaceGroupChangeHandler(IDataAccessor<StructureModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }


        /// <summary>
        /// Resolves the conflicts that are the direct result of a change in the space group model parameter (Loads new space group into space group service if not already done)
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(SpaceGroupInfo groupInfo)
        {
            if (ProjectServices.SpaceGroupService.TryLoadGroup(groupInfo.GroupEntry) == false)
            {
                throw new InvalidOperationException("Space group loading to service failed");
            }
            var report = new ConflictReport();
            MatchCrystalSystemAndCellParameters(report);
            MatchUnitCellPositionsToSpaceGroup(report);
            return report;
        }

        /// <summary>
        /// Matches the crystal system to the space group and checks if the current cell parameters can be applied (Loads default values of the new system if not possible)
        /// </summary>
        /// <param name="report"></param>
        protected void MatchCrystalSystemAndCellParameters(ConflictReport report)
        {
            var oldParameters = DataAccess.Query(data => data.CrystalParameters);
            if (ProjectServices.CrystalSystemService.LoadNewSystem(ProjectServices.SpaceGroupService.LoadedGroup) == false)
            {
                return;
            }

            if (ProjectServices.CrystalSystemService.TrySetParameters(oldParameters.ParameterSet) == false)
            {
                OverwriteCellParameters(ProjectServices.CrystalSystemService.CrystalSystem.GetDefaultParameterSet());

                var detail0 = "The original cell parameters are no longer compatible with the new space group";
                var detail1 = "Conflict resolved by loading a default parameter set for the new crystal system";
                report.Warnings.Add(ModelMessages.CreateContentResetWarning(this, detail0, detail1));
            }

            oldParameters.ParameterSet = ProjectServices.CrystalSystemService.GetCurrentParameterSet();
        }

        /// <summary>
        /// Checks the unit cell position list from first to last and marks later duplicates as deprecated if they produce equal sequence of wyckoffs to former positions
        /// </summary>
        /// <param name="report"></param>
        protected void MatchUnitCellPositionsToSpaceGroup(ConflictReport report)
        {
            var currentPositions = DataAccess.Query(data => data.UnitCellPositions.Select((position) => position.Vector.AsFractional()));
            var extendedPositions = ProjectServices.SpaceGroupService.GetAllWyckoffPositions(currentPositions);

            var comparer = ProjectServices.SpaceGroupService.Comparer.ToEqualityComparer();

            for (int i = 0; i < extendedPositions.Count; i++)
            {
                if (DataAccess.Query(data => data.UnitCellPositions[i].IsDeprecated == false))
                {
                    for (int j = i + 1; j < extendedPositions.Count; j++)
                    {
                        if (extendedPositions[i].SequenceEqual(extendedPositions[j], comparer) == true)
                        {
                            DataAccess.Query(data => data.UnitCellPositions[j].Deprecate());

                            var detail0 = $"The unit cell position at index ({j}) is now equivalent to position at index ({i})";
                            var detail1 = $"Conflict was resolved by marking position at index ({j}) as deprecated";
                            report.Warnings.Add(ModelMessages.CreateConflictHandlingWarning(this, detail0, detail1));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Overwrites the existing cell parameters by a new set both in the data and project services
        /// </summary>
        /// <param name="parameters"></param>
        protected void OverwriteCellParameters(CellParameters parameters)
        {
            if (ProjectServices.CrystalSystemService.TrySetParameters(parameters.ParameterSet) == false)
            {
                throw new InvalidOperationException("Function was called with an invalid parameter set");
            }
            DataAccess.Query(data => data.CrystalParameters = parameters);
        }
    }
}
