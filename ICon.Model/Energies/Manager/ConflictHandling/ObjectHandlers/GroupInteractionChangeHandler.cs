using System;
using System.Collections.Generic;
using System.Linq;
using ICon.Model.Structures;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies.ConflictHandling
{
    public class GroupInteractionChangeHandler : ObjectConflictHandler<GroupInteraction, EnergyModelData>
    {
        /// <summary>
        /// Create a new group interaction change handler that uses the provided data accessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public GroupInteractionChangeHandler(IDataAccessor<EnergyModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Handle conflicts within the energy model data if a group interaction is changed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(GroupInteraction obj)
        {
            var report = new ConflictReport();
            UpdateGroupEnergyDictionary(obj, report);
            LinkToUnstableEnvironment(obj, report);
            return report;
        }

        /// <summary>
        /// Updates the energy dictionary of the group interaction and tries to salvage old energy information
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void UpdateGroupEnergyDictionary(GroupInteraction group, ConflictReport report)
        {
            var ucProvider = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var analyzer = new GeometryGroupAnalyzer(ucProvider, ProjectServices.SpaceGroupService);
            var extGroup = analyzer.CreateExtendedPositionGroup(group);
            SalvageAndUpdateEnergyDictonarySet(group, extGroup, report);
        }

        /// <summary>
        /// Links the group interaction to the affiliated unstable environment if not already in its linking list
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void LinkToUnstableEnvironment(GroupInteraction group, ConflictReport report)
        {
            if (group.CenterUnitCellPosition.Status != PositionStatus.Unstable)
            {
                return;
            }

            var environment = DataAccess.Query(data => data.UnstableEnvironmentInfos.Find(value => value.Index == group.CenterUnitCellPosition.Index));
            if (!environment.GroupInteractions.Contains(group))
            {
                environment.GroupInteractions.Add(group);
                var detail0 = $"The group interaction as automatically linked to the unstable environment at index ({environment.Index})";
                report.AddWarning(ModelMessages.CreateConflictHandlingWarning(this, detail0));
            }
        }

        /// <summary>
        /// Updates the energy information and slavages potential matches from the original dictionary (Salvaging is curtently not supported)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="extGroup"></param>
        /// <param name="report"></param>
        protected void SalvageAndUpdateEnergyDictonarySet(GroupInteraction group, ExtendedPositionGroup extGroup, ConflictReport report)
        {
            if (group.EnergyDictionarySet != null && group.EnergyDictionarySet.Count != 0)
            {
                var detail0 = $"Salvaging of group energy information is currently not supported";
                var detail1 = $"Created a new zero intialized energ dictionary to replace the original one";
                report.AddWarning(ModelMessages.CreateContentResetWarning(this, detail0, detail1));
            }
            group.EnergyDictionarySet = extGroup.UniqueEnergyDictionary;
        }
    }
}
