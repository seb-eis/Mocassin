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
        /// Resolves the conflicts that are the direct result of a change in the space group model parameter (Loads new space group into space group service if not already done)
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public override ConflictReport Resolve(SpaceGroupInfo groupInfo, IDataAccessor<StructureModelData> dataAccess, IProjectServices projectServices)
        {
            if (projectServices.SpaceGroupService.TryLoadGroup(groupInfo.GroupEntry) == false)
            {
                throw new InvalidOperationException("Space group loading to service failed");
            }
            var report = new ConflictReport();
            MatchCrystalSystemAndCellParameters(dataAccess, report, projectServices);
            MatchUnitCellPositionsToSpaceGroup(dataAccess, report, projectServices);
            return report;
        }

        /// <summary>
        /// Matches the crystal system to the space group and checks if the current cell parameters can be applied (Loads default values of the new system if not possible)
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <param name="dataAccess"></param>
        /// <param name="report"></param>
        protected void MatchCrystalSystemAndCellParameters(IDataAccessor<StructureModelData> dataAccess, ConflictReport report, IProjectServices projectServices)
        {
            var oldParameters = dataAccess.Query(data => data.CrystalParameters);
            if (projectServices.CrystalSystemService.LoadNewSystem(projectServices.SpaceGroupService.LoadedGroup) == false)
            {
                return;
            }

            if (projectServices.CrystalSystemService.TrySetParameters(oldParameters.ParameterSet) == false)
            {
                OverwriteCellParameters(projectServices.CrystalSystemService.CrystalSystem.GetDefaultParameterSet(), dataAccess, projectServices);

                var detail0 = "The original cell parameters are no longer compatible with the new space group";
                var detail1 = "Conflict resolved by loading a default parameter set for the new crystal system";
                report.Warnings.Add(ModelMessages.CreateContentResetWarning(this, detail0, detail1));
            }

            oldParameters.ParameterSet = projectServices.CrystalSystemService.GetCurrentParameterSet();
        }

        /// <summary>
        /// Checks the unit cell position list from first to last and marks later duplicates as deprecated if they produce equal sequence of wyckoffs to former positions
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="report"></param>
        protected void MatchUnitCellPositionsToSpaceGroup(IDataAccessor<StructureModelData> dataAccess, ConflictReport report, IProjectServices projectServices)
        {
            var currentPositions = dataAccess.Query(data => data.UnitCellPositions.Select((position) => position.Vector.AsFractional()));
            var extendedPositions = projectServices.SpaceGroupService.GetAllWyckoffPositions(currentPositions);

            var comparer = projectServices.SpaceGroupService.Comparer.ToEqualityComparer();

            for (int i = 0; i < extendedPositions.Count; i++)
            {
                if (dataAccess.Query(data => data.UnitCellPositions[i].IsDeprecated == false))
                {
                    for (int j = i + 1; j < extendedPositions.Count; j++)
                    {
                        if (extendedPositions[i].SequenceEqual(extendedPositions[j], comparer) == true)
                        {
                            dataAccess.Query(data => data.UnitCellPositions[j].Deprecate());

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
        /// <param name="dataAccess"></param>
        protected void OverwriteCellParameters(CellParameters parameters, IDataAccessor<StructureModelData> dataAccess, IProjectServices projectServices)
        {
            if (projectServices.CrystalSystemService.TrySetParameters(parameters.ParameterSet) == false)
            {
                throw new InvalidOperationException("Function was called with an invalid parameter set");
            }
            dataAccess.Query(data => data.CrystalParameters = parameters);
        }
    }
}
