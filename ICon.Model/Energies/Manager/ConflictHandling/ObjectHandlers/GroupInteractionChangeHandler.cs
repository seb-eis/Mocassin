using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies.ConflictHandling
{
    /// <summary>
    /// Object conflict handler for changes in group interaction objects
    /// </summary>
    public class GroupInteractionChangeHandler : ObjectConflictHandler<GroupInteraction, EnergyModelData>
    {
        /// <inheritdoc />
        public GroupInteractionChangeHandler(IDataAccessor<EnergyModelData> dataAccess, IModelProject modelProject)
            : base(dataAccess, modelProject)
        {
        }


        /// <inheritdoc />
        public override ConflictReport HandleConflicts(GroupInteraction obj)
        {
            var report = new ConflictReport();
            UpdateGroupEnergyDictionary(obj, report);
            LinkToUnstableEnvironment(obj, report);
            return report;
        }

        /// <summary>
        ///     Updates the energy dictionary of the group interaction and tries to salvage old energy information
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void UpdateGroupEnergyDictionary(GroupInteraction group, ConflictReport report)
        {
            var ucProvider = ModelProject.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var analyzer = new GeometryGroupAnalyzer(ucProvider, ModelProject.SpaceGroupService);
            var extGroup = analyzer.CreateExtendedPositionGroup(group);
            SalvageAndUpdateEnergyDictionarySet(group, extGroup, report);
        }

        /// <summary>
        ///     Links the group interaction to the affiliated unstable environment if not already in its linking list
        /// </summary>
        /// <param name="group"></param>
        /// <param name="report"></param>
        protected void LinkToUnstableEnvironment(GroupInteraction group, ConflictReport report)
        {
            if (group.CenterUnitCellPosition.Status != PositionStatus.Unstable) return;

            var environment = DataAccess.Query(data =>
                data.UnstableEnvironmentInfos.Find(value => value.Index == group.CenterUnitCellPosition.Index));

            if (environment.GroupInteractions.Contains(group))
                return;

            environment.GroupInteractions.Add(group);
            var detail0 = $"The group interaction as automatically linked to the unstable environment at index ({environment.Index})";
            report.AddWarning(ModelMessageSource.CreateConflictHandlingWarning(this, detail0));
        }

        /// <summary>
        ///     Updates the energy information and salvages potential matches from the original dictionary (Salvaging is currently
        ///     not supported)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="extGroup"></param>
        /// <param name="report"></param>
        protected void SalvageAndUpdateEnergyDictionarySet(GroupInteraction group, ExtendedPositionGroup extGroup, ConflictReport report)
        {
            if (group.EnergyDictionarySet != null && group.EnergyDictionarySet.Count != 0)
            {
                const string detail0 = "Salvaging of group energy information is currently not supported";
                const string detail1 = "Created a new zero initialized energy dictionary to replace the original one";
                report.AddWarning(ModelMessageSource.CreateContentResetWarning(this, detail0, detail1));
            }

            group.EnergyDictionarySet = extGroup.UniqueEnergyDictionary;
        }
    }
}