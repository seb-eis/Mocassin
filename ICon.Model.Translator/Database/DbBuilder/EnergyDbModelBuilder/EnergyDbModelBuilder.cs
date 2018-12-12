using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <inheritdoc cref="Mocassin.Model.Translator.DbBuilder.IEnergyDbModelBuilder" />
    public class EnergyDbModelBuilder : DbModelBuilder, IEnergyDbModelBuilder
    {
        /// <inheritdoc />
        public EnergyDbModelBuilder(IProjectModelContext modelContext)
            : base(modelContext)
        {
        }

        /// <inheritdoc />
        public SimulationEnergyModel BuildModel(ISimulationModel simulationModel)
        {
            var energyModel = CreateNewModel();
            LinkModel(energyModel);
            return energyModel;
        }

        /// <summary>
        ///     Creates the list of interop pair energy database entities that result from the pair interaction models of the
        ///     current energy model context
        /// </summary>
        /// <returns></returns>
        public List<PairEnergyTableEntity> GetPairEnergyTableEntities()
        {
            var entities = ModelContext.EnergyModelContext.PairEnergyModels
                .Select(GetEnergyTableEntity)
                .ToList();

            return entities;
        }

        /// <summary>
        ///     Creates a new pair energy database entity from the passed pair energy model
        /// </summary>
        /// <param name="pairModel"></param>
        /// <returns></returns>
        public PairEnergyTableEntity GetEnergyTableEntity(IPairEnergyModel pairModel)
        {
            var energyTable = new EnergyTableEntity(pairModel.EnergyTable);
            var entity = new PairEnergyTableEntity
            {
                ObjectId = pairModel.ModelId,
                EnergyTable = energyTable
            };

            return entity;
        }

        /// <summary>
        ///     Creates the list of interop cluster energy database entities that result from the group interaction models of the
        ///     current energy model context
        /// </summary>
        /// <returns></returns>
        public List<ClusterEnergyTableEntity> GetClusterEnergyTableEntities()
        {
            var entities = ModelContext.EnergyModelContext.GroupEnergyModels
                .Select(GetEnergyTableEntity)
                .ToList();

            return entities;
        }

        /// <summary>
        ///     Creates a new cluster energy database entity from the passed group energy model
        /// </summary>
        /// <param name="groupModel"></param>
        /// <returns></returns>
        public ClusterEnergyTableEntity GetEnergyTableEntity(IGroupEnergyModel groupModel)
        {
            var energyTable = new EnergyTableEntity(groupModel.EnergyTable);
            var codeList = new OccupationCodeListEntity {Values = groupModel.GroupLookupCodes};

            var entity = new ClusterEnergyTableEntity
            {
                ObjectId = groupModel.ModelId,
                EnergyTable = energyTable,
                ParticleToTableIds = GetParticleToEnergyTableMapping(groupModel),
                OccupationCodeList = codeList
            };

            return entity;
        }

        /// <summary>
        ///     Get the mapping buffer that assigns each particle index its valid energy table row index for the passed group
        ///     energy model and sets invalid entries to an invalid index
        /// </summary>
        /// <param name="groupModel"></param>
        /// <returns></returns>
        public InteropObject<CByteBuffer64> GetParticleToEnergyTableMapping(IGroupEnergyModel groupModel)
        {
            var buffer = new byte[64].Populate(byte.MaxValue);
            var cBuffer = new CByteBuffer64 {Buffer = buffer};

            foreach (var item in groupModel.ParticleIndexToTableMapping)
                buffer[item.Key.Index] = (byte) item.Value;

            return InteropObject.Create(cBuffer);
        }

        /// <summary>
        ///     Performs the internal linking for the passed energy database model
        /// </summary>
        /// <param name="simulationEnergyModel"></param>
        public void LinkModel(SimulationEnergyModel simulationEnergyModel)
        {
            foreach (var entity in simulationEnergyModel.PairEnergyTables)
            {
                entity.SimulationEnergyModel = simulationEnergyModel;
            }

            foreach (var entity in simulationEnergyModel.ClusterEnergyTables)
            {
                entity.SimulationEnergyModel = simulationEnergyModel;
            }
        }

        /// <summary>
        ///     Creates new energy database model with all values set to default
        /// </summary>
        /// <returns></returns>
        public SimulationEnergyModel CreateNewModel()
        {
            return new SimulationEnergyModel
            {
                PairEnergyTableCount = ModelContext.EnergyModelContext.PairEnergyModels.Count,
                ClusterEnergyTableCount = ModelContext.EnergyModelContext.GroupEnergyModels.Count,
                PairEnergyTables = GetPairEnergyTableEntities(),
                ClusterEnergyTables = GetClusterEnergyTableEntities()
            };
        }
    }
}