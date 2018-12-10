using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <inheritdoc cref="Mocassin.Model.Translator.DbBuilder.IStructureDbModelBuilder"/>
    public class StructureDbModelBuilder : DbModelBuilder, IStructureDbModelBuilder
    {
        
        /// <inheritdoc />
        public StructureDbModelBuilder(IProjectModelContext modelContext)
            : base(modelContext)
        {
        }

        /// <inheritdoc />
        public SimulationStructureModel BuildModel(ISimulationModel simulationModel)
        {
            if (!ModelContext.SimulationModelContext.KineticSimulationModels.Any(x => ReferenceEquals(x, simulationModel)))
                throw new InvalidOperationException("Connected model context does not contain the passed simulation model");

            var structureModel = CreateNewModel();
            structureModel.NumOfGlobalTrackers = GetNumberOfGlobalTrackers(simulationModel);
            structureModel.NumOfTrackersPerCell = GetNumberOfTrackersPerCell(simulationModel);
            LinkModel(structureModel);

            return structureModel;
        }

        /// <summary>
        ///     Get the number of static trackers per cell that the passed simulation model requires
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        public int GetNumberOfTrackersPerCell(ISimulationModel simulationModel)
        {
            return simulationModel.SimulationTrackingModel.StaticTrackerCount;
        }

        /// <summary>
        ///     Get the number of global trackers that the passed simulation model requires
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        public int GetNumberOfGlobalTrackers(ISimulationModel simulationModel)
        {
            return simulationModel.SimulationTrackingModel.GlobalTrackerCount;
        }

        /// <summary>
        ///     Get the interaction range interop object for the current project context
        /// </summary>
        /// <returns></returns>
        public InteropObject<CInteractionRange> GetInteractionRange()
        {
            var cStructure = new CInteractionRange
            {
                A = ModelContext.StructureModelContext.InteractionRangeModel.CellsInDirectionA,
                B = ModelContext.StructureModelContext.InteractionRangeModel.CellsInDirectionB,
                C = ModelContext.StructureModelContext.InteractionRangeModel.CellsInDirectionC
            };

            return InteropObject.Create(cStructure);
        }

        /// <summary>
        ///     Get the list of environment definitions that is defined in the current project model context
        /// </summary>
        /// <returns></returns>
        public List<EnvironmentDefinitionEntity> GetEnvironmentDefinitions()
        {
            var definitions = ModelContext.StructureModelContext.PositionModels
                .Select(GetEnvironmentDefinition)
                .ToList();

            return definitions;
        }

        /// <summary>
        ///     Get the environment definition database entity for the passed position model
        /// </summary>
        /// <param name="positionModel"></param>
        /// <returns></returns>
        public EnvironmentDefinitionEntity GetEnvironmentDefinition(IPositionModel positionModel)
        {
            var entity = new EnvironmentDefinitionEntity
            {
                ObjectId = positionModel.ModelId,
                PositionParticleIds = GetParticleIdBuffer(positionModel.UnitCellPosition.OccupationSet),
                PairDefinitionList = GetPairDefinitionList(positionModel),
                ClusterDefinitionList = GetClusterDefinitionList(positionModel.EnvironmentModel.GroupInteractionModels)
            };

            return entity;
        }

        /// <summary>
        ///     Get a 64 byte particle id buffer that contains the passed particle set ids that are not deprecated and sets all
        ///     other values to the invalid id value
        /// </summary>
        /// <param name="particleSet"></param>
        /// <returns></returns>
        public InteropObject<CByteBuffer64> GetParticleIdBuffer(IParticleSet particleSet)
        {
            var buffer = new byte[64].Populate(byte.MaxValue);
            var cStruct = new CByteBuffer64
            {
                Buffer = buffer
            };

            var index = 0;

            foreach (var particle in particleSet)
            {
                if (particle.IsDeprecated)
                    continue;

                buffer[index++] = (byte) particle.Index;
            }

            return InteropObject.Create(cStruct);
        }

        /// <summary>
        ///     Get the pair definition list entity that results from the passed position model
        /// </summary>
        /// <param name="positionModel"></param>
        /// <returns></returns>
        public PairDefinitionListEntity GetPairDefinitionList(IPositionModel positionModel)
        {
            var definitions = positionModel.TargetPositionInfos
                .Select((info, id) => GetPairDefinitionStruct(positionModel.EnvironmentModel.PairInteractionModels[id], info))
                .ToList();

            return new PairDefinitionListEntity
            {
                Values = definitions
            };
        }

        /// <summary>
        ///     Get the unmanaged C representation of the passed combination of pair interaction model an position info
        /// </summary>
        /// <param name="pairModel"></param>
        /// <param name="positionInfo"></param>
        /// <returns></returns>
        public CPairDefinition GetPairDefinitionStruct(IPairInteractionModel pairModel, ITargetPositionInfo positionInfo)
        {
            var cVector = new CVector4(positionInfo.RelativeVector4D);

            return new CPairDefinition
            {
                TableId = pairModel.PairEnergyModel.ModelId,
                RelativeVector = cVector
            };
        }

        /// <summary>
        ///     Get the cluster definition list entity that results from the passed group interaction model list
        /// </summary>
        /// <param name="groupModels"></param>
        /// <returns></returns>
        public ClusterDefinitionListEntity GetClusterDefinitionList(IList<IGroupInteractionModel> groupModels)
        {
            var definitions = groupModels
                .Select(GetClusterDefinitionStruct)
                .ToList();

            return new ClusterDefinitionListEntity
            {
                Values = definitions
            };
        }

        /// <summary>
        ///     Get the unmanaged C representation of the passed group interaction model
        /// </summary>
        /// <param name="groupModel"></param>
        /// <returns></returns>
        public CClusterDefinition GetClusterDefinitionStruct(IGroupInteractionModel groupModel)
        {
            var buffer = new int[groupModel.PairIndexCoding.Length];
            groupModel.PairIndexCoding.CopyTo(buffer, 0);

            return new CClusterDefinition
            {
                TableId = groupModel.GroupEnergyModel.ModelId,
                RelativePositionIds = buffer
            };
        }

        /// <summary>
        ///     Creates a new structure model with all basic data set to the default values
        /// </summary>
        /// <returns></returns>
        protected SimulationStructureModel CreateNewModel()
        {
            return new SimulationStructureModel
            {
                InteractionRange = GetInteractionRange(),
                EnvironmentDefinitions = GetEnvironmentDefinitions()
            };
        }

        /// <summary>
        /// Performs all required internal linking operations of the structure model and its components
        /// </summary>
        /// <param name="simulationStructureModel"></param>
        protected void LinkModel(SimulationStructureModel simulationStructureModel)
        {
            foreach (var entity in simulationStructureModel.EnvironmentDefinitions)
            {
                entity.SimulationStructureModel = simulationStructureModel;
            }
        }
    }
}