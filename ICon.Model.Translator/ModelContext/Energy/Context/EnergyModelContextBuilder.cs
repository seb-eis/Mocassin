using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ICon.Framework.Extensions;
using System.Threading.Tasks;
using ICon.Framework.Collections;
using ICon.Model.Energies;
using ICon.Model.Particles;
using ICon.Model.ProjectServices;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Builder for the energy model context. Expands the reference energy data to a full data context for simulation generation/evaluation
    /// </summary>
    public class EnergyModelContextBuilder : ModelContextBuilder<IEnergyModelContext>
    {
        /// <summary>
        /// Create new energy model context builder that uses the passed project access
        /// </summary>
        /// <param name="projectServices"></param>
        public EnergyModelContextBuilder(IProjectServices projectServices) : base(projectServices)
        {

        }

        /// <summary>
        /// Populates the currently set energy context data
        /// </summary>
        protected override void PopulateContext()
        {
            var pairBuild = Task.Run(() => BuildPairEnergyModels());
            var groupBuild = Task.Run(() => BuildGroupEnergyModels());
            ModelContext.PairEnergyModels = pairBuild.Result;
            ModelContext.GroupEnergyModels = groupBuild.Result;
        }

        /// <summary>
        /// Creates the list of pair energy models that exist in the current project
        /// </summary>
        /// <returns></returns>
        protected IList<IPairEnergyModel> BuildPairEnergyModels()
        {
            var pairEnergyModels = new List<IPairEnergyModel>();
            var energyManager = ProjectServices.GetManager<IEnergyManager>();

            var symmetricPairs = energyManager.QueryPort.Query(port => port.GetStablePairInteractions());
            var asymmetricPairs = energyManager.QueryPort.Query(port => port.GetUnstablePairInteractions());

            AddPairInteractionsToModels(pairEnergyModels, symmetricPairs);
            AddPairInteractionsToModels(pairEnergyModels, asymmetricPairs);

            return pairEnergyModels;
        }

        /// <summary>
        /// Create and add a set of pair inetraction models to the end of the model list
        /// </summary>
        /// <param name="pairEnergyModels"></param>
        /// <param name="interactions"></param>
        protected void AddPairInteractionsToModels(IList<IPairEnergyModel> pairEnergyModels, IEnumerable<IPairInteraction> interactions)
        {
            foreach (var interaction in interactions)
            {
                var pairEnergyModel = new PairEnergyModel(interaction)
                {
                    ModelId = pairEnergyModels.Count
                };

                int maxParticleIndex = CopyEnergyEntriesToModel(pairEnergyModel);
                pairEnergyModel.EnergyTable = BuildPairEnergyTable(pairEnergyModel.EnergyEntries, maxParticleIndex);
                pairEnergyModels.Add(pairEnergyModel);
            }
        }

        /// <summary>
        /// Copies all energy entries of the set interaction into the energy model list an returns the largest found particle index
        /// </summary>
        /// <param name="energyModel"></param>
        /// <returns></returns>
        protected int CopyEnergyEntriesToModel(IPairEnergyModel energyModel)
        {
            int lastIndex = 0;
            energyModel.EnergyEntries = new List<PairEnergyEntry>();

            foreach (var entry in energyModel.PairInteraction.GetEnergyEntries())
            {
                int checkIndex = entry.ParticlePair.Particle0.Index;
                lastIndex = (lastIndex > checkIndex) ? lastIndex : checkIndex;
                energyModel.EnergyEntries.Add(entry);
            }

            return lastIndex;
        }

        /// <summary>
        /// Builds the energy table based upon the passed energy entry collection
        /// </summary>
        /// <param name="energyDictionary"></param>
        /// <param name="largestIndex"></param>
        /// <returns></returns>
        protected double[,] BuildPairEnergyTable(IList<PairEnergyEntry> energyEntries, int largestIndex)
        {
            var table = new double[largestIndex+1, largestIndex+1];

            foreach (var entry in energyEntries)
            {
                int id0 = entry.ParticlePair.Particle0.Index;
                int id1 = entry.ParticlePair.Particle1.Index;
                table[id0, id1] = entry.Energy;

                if (entry.ParticlePair is SymmetricParticlePair)
                {
                    table[id1, id0] = entry.Energy;
                }
            }

            return table;
        }

        /// <summary>
        /// Creates the sorted list of group energy models that exist in the current project
        /// </summary>
        /// <returns></returns>
        protected IList<IGroupEnergyModel> BuildGroupEnergyModels()
        {
            var groupEnergyModels = new List<IGroupEnergyModel>();
            var energyManager = ProjectServices.GetManager<IEnergyManager>();

            var groupInteractions = energyManager.QueryPort.Query(port => port.GetGroupInteractions());

            AddGroupInteractionsToModels(groupEnergyModels, groupInteractions, energyManager);

            return groupEnergyModels;
        }

        /// <summary>
        /// Builds and adds the group energy models for the passed group interactions the the group energy model list
        /// </summary>
        /// <param name="groupEnergyModels"></param>
        /// <param name="groupInteractions"></param>
        /// <param name="energyManager"></param>
        protected void AddGroupInteractionsToModels(IList<IGroupEnergyModel> groupEnergyModels, IEnumerable<IGroupInteraction> groupInteractions, IEnergyManager energyManager)
        {
            var positionGroupInfos = energyManager.QueryPort.Query(port => port.GetPositionGroupInfos());
            foreach (var interaction in groupInteractions)
            {
                var energyModel = new GroupEnergyModel(interaction)
                {
                    ModelId = groupEnergyModels.Count,
                    PositionGroupInfo = positionGroupInfos[interaction.CenterUnitCellPosition.Index]
                };

                CreateSymmetryExtendedEnergyInfoOnModel(energyModel);
                CreateAllGroupLookupCodesOnModel(energyModel);
                CreateGroupEnergyTableOnModel(energyModel);
            }
        }

        /// <summary>
        /// CBuilds an copies all symmetry extended energy and occupation information on the passed model
        /// </summary>
        /// <param name="energyModel"></param>
        protected void CreateSymmetryExtendedEnergyInfoOnModel(IGroupEnergyModel energyModel)
        {
            RestoreSymmetryReducedInformation(energyModel);

            int index = 0;
            energyModel.CenterParticleIndexing = new Dictionary<IParticle, int>();

            foreach (var entry in energyModel.GroupInteraction.CenterUnitCellPosition.OccupationSet.GetParticles())
            {
                energyModel.CenterParticleIndexing.Add(entry, index++);
            }
        }

        /// <summary>
        ///  Restrores the full set of group energy entries and occupation states of a group energy model
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
        /// Creates the sorted list of group lookup codes
        /// </summary>
        /// <param name="occupationStates"></param>
        /// <returns></returns>
        protected void CreateAllGroupLookupCodesOnModel(IGroupEnergyModel groupEnergyModel)
        {
            var codes = new SetList<long>(Comparer<long>.Default, groupEnergyModel.OccupationStates.Count);
            var buffer = new byte[8];

            foreach (var state in groupEnergyModel.OccupationStates)
            {
                int index = 0;
                for (; index < state.StateLength; index++)
                {
                    buffer[index] = (byte)state.Particles[index].Index;
                    long code = BitConverter.ToInt64(buffer, 0);
                    codes.Add(code);
                }
                for (int i = 0; i < index; i++)
                {
                    buffer[i] = 0;
                }
            }
            groupEnergyModel.GroupLookupCodes = codes;
        }

        /// <summary>
        /// Creates the energy table of the group an adds it to the group energy model
        /// </summary>
        /// <param name="groupEnergyModel"></param>
        protected void CreateGroupEnergyTableOnModel(IGroupEnergyModel groupEnergyModel)
        {
            
        }
    }
}
