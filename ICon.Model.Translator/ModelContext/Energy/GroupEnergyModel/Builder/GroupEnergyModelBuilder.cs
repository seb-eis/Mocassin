using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IGroupEnergyModelBuilder" />
    public class GroupEnergyModelBuilder : ModelBuilderBase, IGroupEnergyModelBuilder
    {
        /// <inheritdoc />
        public GroupEnergyModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IGroupEnergyModel> BuildModels(IList<IGroupInteraction> groupInteractions)
        {
            var manager = ModelProject.Manager<IEnergyManager>();
            var positionGroupInfos = manager.DataAccess.Query(port => port.GetPositionGroupInfos());
            var index = 0;
            var groupEnergyModels = groupInteractions
                .Select(interaction => CreateEnergyModel(interaction, positionGroupInfos, ref index))
                .ToList();

            return groupEnergyModels;
        }

        /// <summary>
        ///     Builds a group energy model from the passed group interaction, position group infos and index counter
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <param name="positionGroupInfos"></param>
        /// <param name="index"></param>
        protected IGroupEnergyModel CreateEnergyModel(IGroupInteraction groupInteraction,
            IReadOnlyList<IPositionGroupInfo> positionGroupInfos, ref int index)
        {
            var groupInfo = positionGroupInfos.Single(a => a.GroupInteraction.Index == groupInteraction.Index);
            var energyModel = new GroupEnergyModel(groupInteraction)
            {
                ModelId = index++,
                PositionGroupInfo = groupInfo
            };

            CreateSymmetryExtendedEnergyInfoOnModel(energyModel);
            CreateAllGroupLookupCodesOnModel(energyModel);
            CreateGroupEnergyTableOnModel(energyModel);

            return energyModel;
        }

        /// <summary>
        ///     CBuilds an copies all symmetry extended energy and occupation information on the passed model
        /// </summary>
        /// <param name="energyModel"></param>
        protected void CreateSymmetryExtendedEnergyInfoOnModel(IGroupEnergyModel energyModel)
        {
            RestoreSymmetryReducedInformation(energyModel);

            var index = 0;
            energyModel.ParticleIndexToTableMapping = new Dictionary<IParticle, int>();

            foreach (var entry in energyModel.GroupInteraction.CenterCellSite.OccupationSet.GetParticles())
                energyModel.ParticleIndexToTableMapping.Add(entry, index++);
        }

        /// <summary>
        ///     Restores the full set of group energy entries and occupation states of a group energy model
        /// </summary>
        /// <param name="groupEnergyModel"></param>
        /// <returns></returns>
        protected void RestoreSymmetryReducedInformation(IGroupEnergyModel groupEnergyModel)
        {
            var energyEntries = new SetList<GroupEnergyEntry>();
            var occupationStates = new SetList<IOccupationState>();
            var reorders = groupEnergyModel.PositionGroupInfo.PointOperationGroup.GetUniqueProjectionOrders().ToList();

            foreach (var energyEntry in groupEnergyModel.PositionGroupInfo.GetEnergyEntryList())
            {
                energyEntries.Add(energyEntry);
                occupationStates.Add(energyEntry.GroupOccupation);

                foreach (var newOrder in reorders)
                {
                    var reorderedEntry = energyEntry.CreateReordered(newOrder);
                    energyEntries.Add(reorderedEntry);
                    occupationStates.Add(reorderedEntry.GroupOccupation);
                }
            }

            groupEnergyModel.EnergyEntries = energyEntries;
            groupEnergyModel.OccupationStates = occupationStates;
        }

        /// <summary>
        ///     Creates the sorted list of group lookup codes
        /// </summary>
        /// <param name="groupEnergyModel"></param>
        /// <returns></returns>
        protected void CreateAllGroupLookupCodesOnModel(IGroupEnergyModel groupEnergyModel)
        {
            var codes = new SetList<long>(Comparer<long>.Default, groupEnergyModel.OccupationStates.Count);
            var buffer = new byte[8];

            foreach (var state in groupEnergyModel.OccupationStates)
            {
                var index = 0;

                for (; index < state.StateLength; index++)
                    buffer[index] = (byte) state.Particles[index].Index;

                var code = BitConverter.ToInt64(buffer, 0);
                codes.Add(code);

                for (var i = 0; i < index; i++) buffer[i] = 0;
            }

            groupEnergyModel.GroupLookupCodes = codes;
        }

        /// <summary>
        ///     Creates the energy table of the group an adds it to the group energy model
        /// </summary>
        /// <param name="groupEnergyModel"></param>
        protected void CreateGroupEnergyTableOnModel(IGroupEnergyModel groupEnergyModel)
        {
            var rowCount = groupEnergyModel.ParticleIndexToTableMapping.Count;
            var colCount = groupEnergyModel.GroupLookupCodes.Count;
            var energyTable = new double[rowCount, colCount];

            var index = 0;
            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                    energyTable[row, col] = groupEnergyModel.EnergyEntries[index++].Energy;
            }

            groupEnergyModel.EnergyTable = energyTable;
        }
    }
}