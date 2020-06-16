using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.EntityBuilder
{
    /// <inheritdoc cref="IStructureDbEntityBuilder" />
    public class StructureDbEntityBuilder : DbEntityBuilder, IStructureDbEntityBuilder
    {
        /// <inheritdoc />
        public StructureDbEntityBuilder(IProjectModelContext modelContext)
            : base(modelContext)
        {
        }

        /// <inheritdoc />
        public SimulationStructureModel BuildModel(ISimulationModel simulationModel)
        {
            var structureModel = CreateNewModel();
            structureModel.StructureMetaData = GetStructureMetaData(simulationModel);
            structureModel.NumOfGlobalTrackers = GetGlobalTrackerCount(simulationModel);
            structureModel.NumOfTrackersPerCell = GetStaticTrackerCountPerCell(simulationModel);
            SetUpdateAndSelectionInformation(structureModel, simulationModel.SimulationEncodingModel);
            LinkModel(structureModel);

            return structureModel;
        }

        /// <summary>
        ///     Sets the update and selection info on the simulation structure model as describes by the passed simulation encoding
        ///     model
        /// </summary>
        /// <param name="structureModel"></param>
        /// <param name="encodingModel"></param>
        protected void SetUpdateAndSelectionInformation(SimulationStructureModel structureModel, ISimulationEncodingModel encodingModel)
        {
            foreach (var environmentDefinition in structureModel.EnvironmentDefinitions)
            {
                environmentDefinition.UpdateParticleIds = GetUpdateParticleIdBuffer(environmentDefinition.ObjectId, encodingModel);
                environmentDefinition.SelectionParticleMask = GetParticleSelectionMask(environmentDefinition.ObjectId, encodingModel);
            }
        }

        /// <summary>
        ///     Creates the particle selection mask for the passed position id in the context of a simulation encoding model
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="encodingModel"></param>
        /// <returns></returns>
        protected long GetParticleSelectionMask(int positionId, ISimulationEncodingModel encodingModel)
        {
            var (mask, particleId) = (0L, 0);
            foreach (var mobilityType in encodingModel.PositionIndexToMobilityTypesSet[positionId])
            {
                if (mobilityType == MobilityType.Selectable)
                    mask |= 1L << particleId;

                particleId++;
            }

            return mask;
        }

        /// <summary>
        ///     Builds the update particle id buffer for a position id that fits the provided simulation encoding model and removes
        ///     always 0 energy entries from the update process
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="encodingModel"></param>
        /// <returns></returns>
        protected InteropObject<CByteBuffer64> GetUpdateParticleIdBuffer(int positionId, ISimulationEncodingModel encodingModel)
        {
            var buffer = new byte[64].Populate(SimulationConstants.InvalidParticleId);

            (byte particleId, byte bufferId) = (0, 0);
            foreach (var _ in encodingModel.PositionIndexToMobilityTypesSet[positionId])
            {
                if (ParticlePositionPairHasNonZeroEnergies(positionId, particleId)) buffer[bufferId++] = particleId;
                particleId++;
            }

            return InteropObject.Create(new CByteBuffer64 {Buffer = buffer});
        }

        /// <summary>
        ///     Checks if a position id and particle id combination has any non zero energy entries in affiliated pair or group
        ///     tables
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="particleId"></param>
        /// <returns></returns>
        protected bool ParticlePositionPairHasNonZeroEnergies(int positionId, byte particleId)
        {
            var comparer = ModelContext.ModelProject.CommonNumeric.RangeComparer;
            var environmentModel = ModelContext.StructureModelContext.PositionModels[positionId].EnvironmentModel;
            if (environmentModel.CellSite.OccupationSet.All(particle => particle.Index != particleId)) return false;

            if (environmentModel.PairInteractionModels.Select(x => x.PairEnergyModel)
                .Any(energyModel => energyModel.EnergyEntries.Any(x =>
                    x.ParticleInteractionPair.Particle0.Index == particleId && comparer.Compare(x.Energy, 0.0) != 0)))
                return true;

            return environmentModel.GroupInteractionModels.Select(x => x.GroupEnergyModel).Any(energyModel =>
                energyModel.EnergyEntries.Any(x => x.CenterParticle.Index == particleId && comparer.Compare(x.Energy, 0.0) != 0));
        }

        /// <summary>
        ///     Get the number of static trackers per cell that the passed simulation model requires
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected int GetStaticTrackerCountPerCell(ISimulationModel simulationModel)
        {
            return simulationModel.SimulationTrackingModel.StaticTrackerCount;
        }

        /// <summary>
        ///     Get the number of global trackers that the passed simulation model requires
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected int GetGlobalTrackerCount(ISimulationModel simulationModel)
        {
            return simulationModel.SimulationTrackingModel.GlobalTrackerCount;
        }

        /// <summary>
        ///     Get the wrapped <see cref="CInteractionRange" /> interop object for the current project context
        /// </summary>
        /// <returns></returns>
        protected InteropObject<CInteractionRange> GetInteractionRange()
        {
            var cStructure = new CInteractionRange
            {
                Vector = new CVector4
                {
                    A = ModelContext.StructureModelContext.InteractionRangeModel.CellsInDirectionA,
                    B = ModelContext.StructureModelContext.InteractionRangeModel.CellsInDirectionB,
                    C = ModelContext.StructureModelContext.InteractionRangeModel.CellsInDirectionC,
                    D = 0
                }
            };

            return InteropObject.Create(cStructure);
        }

        /// <summary>
        ///     Get the <see cref="CStructureMetaData" /> object from the current project context that belongs to the passed
        ///     <see cref="ISimulationModel" />
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected InteropObject<CStructureMetaData> GetStructureMetaData(ISimulationModel simulationModel)
        {
            var (cellVectorA, cellVectorB, cellVectorC) = ModelContext.ModelProject
                .Manager<IStructureManager>()
                .DataAccess.Query(x => x.GetVectorEncoder())
                .Transformer.FractionalSystem.GetBaseVectors();

            var charges = new double[64].Populate(double.NaN);
            foreach (var particle in ModelContext.ModelProject.Manager<IParticleManager>().DataAccess.Query(x => x.GetParticles()))
                charges[particle.Index] = particle.Charge;

            var cStructure = new CStructureMetaData
            {
                ParticleCharges = charges,
                NormalizedElectricFieldVector = new CVector3(simulationModel.NormalizedElectricFieldVector),
                UnitCellVectorA = new CVector3(cellVectorA),
                UnitCellVectorB = new CVector3(cellVectorB),
                UnitCellVectorC = new CVector3(cellVectorC)
            };

            return InteropObject.Create(cStructure);
        }

        /// <summary>
        ///     Get the list of environment definitions that is defined in the current project model context
        /// </summary>
        /// <returns></returns>
        protected List<EnvironmentDefinitionEntity> GetEnvironmentDefinitions()
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
        protected EnvironmentDefinitionEntity GetEnvironmentDefinition(IPositionModel positionModel)
        {
            var entity = new EnvironmentDefinitionEntity
            {
                ObjectId = positionModel.ModelId,
                PositionParticleIds = GetParticleIdBuffer(positionModel.CellSite.OccupationSet),
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
        protected InteropObject<CByteBuffer64> GetParticleIdBuffer(IParticleSet particleSet)
        {
            var buffer = new byte[64].Populate(SimulationConstants.InvalidParticleId);
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
        protected PairDefinitionListEntity GetPairDefinitionList(IPositionModel positionModel)
        {
            var definitions = positionModel.TargetPositionInfos
                .Select(info => GetPairDefinitionStruct(info.PairInteractionModel, info))
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
        protected CPairInteraction GetPairDefinitionStruct(IPairInteractionModel pairModel, ITargetPositionInfo positionInfo)
        {
            var cVector = new CVector4(positionInfo.RelativeCrystalVector);

            return new CPairInteraction
            {
                PairTableId = pairModel.PairEnergyModel.ModelId,
                RelativeVector = cVector
            };
        }

        /// <summary>
        ///     Get the cluster definition list entity that results from the passed group interaction model list
        /// </summary>
        /// <param name="groupModels"></param>
        /// <returns></returns>
        protected ClusterDefinitionListEntity GetClusterDefinitionList(IList<IGroupInteractionModel> groupModels)
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
        protected CClusterInteraction GetClusterDefinitionStruct(IGroupInteractionModel groupModel)
        {
            var buffer = new int[groupModel.PairIndexCoding.Length];
            groupModel.PairIndexCoding.CopyTo(buffer, 0);

            return new CClusterInteraction
            {
                ClusterTableId = groupModel.GroupEnergyModel.ModelId,
                RelativePositionIds = buffer
            };
        }

        /// <summary>
        ///     Creates a new structure model with all basic data set to the default values
        /// </summary>
        /// <returns></returns>
        protected SimulationStructureModel CreateNewModel()
        {
            var model = new SimulationStructureModel
            {
                InteractionRange = GetInteractionRange(),
                EnvironmentDefinitions = GetEnvironmentDefinitions()
            };

            model.NumOfEnvironmentDefinitions = model.EnvironmentDefinitions.Count;
            return model;
        }

        /// <summary>
        ///     Performs all required internal linking operations of the structure model and its components
        /// </summary>
        /// <param name="simulationStructureModel"></param>
        protected void LinkModel(SimulationStructureModel simulationStructureModel)
        {
            foreach (var entity in simulationStructureModel.EnvironmentDefinitions)
                entity.SimulationStructureModel = simulationStructureModel;
        }
    }
}