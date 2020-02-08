using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Simulations;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IKineticSimulationModelBuilder" />
    public class KineticSimulationModelBuilder : SimulationModelBuilderBase, IKineticSimulationModelBuilder
    {
        /// <inheritdoc />
        public KineticSimulationModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IKineticSimulationModel> BuildModels(IEnumerable<IKineticSimulation> kineticSimulations)
        {
            return kineticSimulations
                .Select(BuildSimulationModel)
                .ToList();
        }

        /// <inheritdoc />
        public void BuildLinkingDependentComponents(IEnumerable<IKineticSimulationModel> simulationModels)
        {
            foreach (var simulationModel in simulationModels)
            {
                var indexing = (0, 0);
                simulationModel.MappingAssignMatrix =
                    CreateMappingAssignMatrix<IKineticMappingModel, IKineticTransitionModel>(simulationModel.TransitionModels);

                AddAttemptFrequencyInformation(simulationModel);
                AddLocalJumpModels(simulationModel);
                AddKineticTrackingModel(simulationModel);
                IndexTrackerModels(simulationModel, ref indexing);
                AddTrackingModelMappingTables(simulationModel.SimulationTrackingModel);
                AddEncodingModel(simulationModel);
            }
        }

        /// <summary>
        ///     Creates and adds the kinetic indexing model for the passed and fully created simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        protected void AddEncodingModel(IKineticSimulationModel simulationModel)
        {
            var encodingModel = new SimulationEncodingModel
            {
                TransitionModelToJumpCollectionId = GetTransitionModelIndexing(simulationModel.TransitionModels),
                TransitionMappingToJumpDirectionId = GetTransitionMappingIndexing(simulationModel.TransitionModels),
                JumpCountMappingTable = GetJumpCountTable(simulationModel.MappingAssignMatrix, simulationModel.LocalJumpModels),
                PositionIndexToMobilityTypesSet = GetPositionIndexToMobilitySetMapping(simulationModel.LocalJumpModels)
            };

            AddElectricFieldInfluenceMappings(encodingModel, simulationModel.LocalJumpModels);
            AddJumpIndexAssignTable(encodingModel, simulationModel.MappingAssignMatrix);
            simulationModel.SimulationEncodingModel = encodingModel;
        }

        /// <summary>
        ///     Builds a single kinetic simulation model for the passed kinetic simulation
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        protected IKineticSimulationModel BuildSimulationModel(IKineticSimulation simulation)
        {
            var simulationModel = new KineticSimulationModel
            {
                Simulation = simulation,
                TransitionModels = simulation.Transitions
                    .Select(a => new KineticTransitionModel {Transition = a})
                    .Cast<IKineticTransitionModel>()
                    .ToList()
            };

            AddNormalizedElectricFieldVector(simulationModel);
            return simulationModel;
        }

        /// <summary>
        ///     Determines and adds the attempt frequency information to the passed kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        protected void AddAttemptFrequencyInformation(IKineticSimulationModel simulationModel)
        {
            simulationModel.MaxAttemptFrequency = simulationModel.TransitionModels
                .SelectMany(x => x.RuleModels)
                .Max(x => x.KineticRule.AttemptFrequency);
        }

        /// <summary>
        ///     Indexes the passed simulation model using the passed indexing set reference
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="indexing"></param>
        protected void IndexTrackerModels(IKineticSimulationModel simulationModel, ref (int Static, int Global) indexing)
        {
            foreach (var staticTrackerModel in simulationModel.SimulationTrackingModel.StaticTrackerModels)
                staticTrackerModel.ModelId = indexing.Static++;

            foreach (var globalTrackerModel in simulationModel.SimulationTrackingModel.GlobalTrackerModels)
                globalTrackerModel.ModelId = indexing.Global++;
        }

        /// <summary>
        ///     Creates and adds the tracking model mapping tables to the passed kinetic tracking model
        /// </summary>
        /// <param name="trackingModel"></param>
        /// <remarks> For a valid mapping to be created the indexing of the tracking model has to be final </remarks>
        protected void AddTrackingModelMappingTables(ISimulationTrackingModel trackingModel)
        {
            trackingModel.StaticTrackerMappingTable = CreateStaticTrackerMappingTable(trackingModel.StaticTrackerModels);
            trackingModel.GlobalTrackerMappingTable = CreateGlobalTrackerMappingTable(trackingModel.GlobalTrackerModels);
        }

        /// <summary>
        ///     Creates a 2D index mapping table that assigns each positionId/particleId combination its basic static movement
        ///     tracker index for the simulation
        /// </summary>
        /// <param name="trackerModels"></param>
        /// <returns></returns>
        /// <remarks> Assigns each combination that does not support tracking the value -1 </remarks>
        protected int[,] CreateStaticTrackerMappingTable(IList<IStaticMovementTrackerModel> trackerModels)
        {
            var maxPositionId = ModelProject.GetManager<IStructureManager>().QueryPort
                .Query(port => port.GetLinearizedExtendedPositionCount());

            var maxParticleId = ModelProject.GetManager<IParticleManager>().QueryPort
                .Query(port => port.ParticleCount);

            var result = new int[maxPositionId, maxParticleId].Populate(() => -1);

            foreach (var trackerModel in trackerModels)
                result[trackerModel.TrackedPositionIndex, trackerModel.TrackedParticleIndex] = trackerModel.ModelId;

            return result;
        }

        /// <summary>
        ///     Creates a 2D index mapping table that assigns each transitionId/particleId its global tracker index for the
        ///     simulation
        /// </summary>
        /// <param name="trackerModels"></param>
        /// <returns></returns>
        /// <remarks> Assigns each combination that does not support tracking the value -1 </remarks>
        protected int[,] CreateGlobalTrackerMappingTable(IList<IGlobalTrackerModel> trackerModels)
        {
            var maxTransitionId = 1 + trackerModels.Select(a => a.KineticTransitionModel.ModelId).Max();
            var maxParticleId = ModelProject.GetManager<IParticleManager>().QueryPort
                .Query(port => port.ParticleCount);

            var result = new int[maxTransitionId, maxParticleId].Populate(() => -1);

            foreach (var trackerModel in trackerModels)
                result[trackerModel.KineticTransitionModel.ModelId, trackerModel.TrackedParticle.Index] = trackerModel.ModelId;

            return result;
        }

        /// <summary>
        ///     Calculates and adds the normalized cartesian electric field vector to the passed kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        protected void AddNormalizedElectricFieldVector(IKineticSimulationModel simulationModel)
        {
            var encoder = ModelProject.GetManager<IStructureManager>().QueryPort.Query(port => port.GetVectorEncoder());
            var fieldVector = encoder.Transformer.ToCartesian(simulationModel.Simulation.ElectricFieldVector);
            simulationModel.NormalizedElectricFieldVector = fieldVector.GetNormalized();
        }

        /// <summary>
        ///     Creates and adds the list of kinetic local simulation models to the passed kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        protected void AddLocalJumpModels(IKineticSimulationModel simulationModel)
        {
            var jumpModels = new List<IKineticLocalJumpModel>();
            var modelId = 0;
            for (var positionId = 0; positionId < simulationModel.MappingAssignMatrix.GetLength(0); positionId++)
            {
                for (var particleId = 0; particleId < simulationModel.MappingAssignMatrix.GetLength(1); particleId++)
                {
                    for (var objId = 0; objId < simulationModel.MappingAssignMatrix.GetLength(2); objId++)
                    {
                        var mappingModel = simulationModel.MappingAssignMatrix[positionId, particleId, objId];
                        if (mappingModel == null)
                            break;

                        foreach (var ruleModel in mappingModel.TransitionModel.RuleModels.Where(rule =>
                            rule.SelectableParticle.Index == particleId))
                        {
                            var jumpModel = CreateLocalJumpModel(mappingModel, ruleModel, simulationModel, ref modelId);
                            jumpModels.Add(jumpModel);
                        }
                    }
                }
            }

            jumpModels.RemoveDuplicates(EqualityComparer<IKineticLocalJumpModel>.Default);
            simulationModel.LocalJumpModels = jumpModels.ToList();
        }

        /// <summary>
        ///     Creates the local jump model for the provided combination of kinetic rule model and mapping model in the context of
        ///     the defined kinetic simulation model
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <param name="ruleModel"></param>
        /// <param name="simulationModel"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        protected IKineticLocalJumpModel CreateLocalJumpModel(IKineticMappingModel mappingModel, IKineticRuleModel ruleModel,
            IKineticSimulationModel simulationModel, ref int modelId)
        {
            var jumpModel = new KineticLocalJumpModel
            {
                ModelId = modelId++,
                MappingModel = mappingModel,
                RuleModel = ruleModel,
                ChargeTransportVector = GetChargeTransportVector(mappingModel, ruleModel)
            };
            AddElectricFieldInfluenceFactors(jumpModel, simulationModel);

            return jumpModel;
        }

        /// <summary>
        ///     Determines the electric influence factor of a <see cref="IKineticLocalJumpModel" /> in the context of the passed
        ///     <see cref="IKineticSimulationModel" />
        /// </summary>
        /// <param name="jumpModel"></param>
        /// <param name="simulationModel"></param>
        protected void AddElectricFieldInfluenceFactors(IKineticLocalJumpModel jumpModel, IKineticSimulationModel simulationModel)
        {
            jumpModel.NormalizedElectricFieldInfluence =
                GetNormalizedElectricInfluence(jumpModel.ChargeTransportVector, simulationModel.NormalizedElectricFieldVector);

            jumpModel.ElectricFieldMappingFactor = jumpModel.NormalizedElectricFieldInfluence;

            if (jumpModel.NormalizedElectricFieldInfluence < 0 && jumpModel.ElectricFieldRuleInfluence < 0)
                jumpModel.ElectricFieldMappingFactor *= -1.0;

            if (jumpModel.NormalizedElectricFieldInfluence > 0 && jumpModel.ElectricFieldRuleInfluence < 0)
                jumpModel.ElectricFieldMappingFactor *= -1.0;
        }

        /// <summary>
        ///     Get the charge transport vector [charge*movement] for the passed combination of kinetic mapping model and kinetic
        ///     rule model in [eV * m / V]
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <param name="ruleModel"></param>
        /// <returns></returns>
        protected Cartesian3D GetChargeTransportVector(IKineticMappingModel mappingModel, IKineticRuleModel ruleModel)
        {
            var transformer = ModelProject.GetManager<IStructureManager>().QueryPort.Query(port => port.GetVectorEncoder()).Transformer;
            var transportMatrix = mappingModel.PositionMovementMatrix * ruleModel.ChargeTransportMatrix.GetTransposed();
            var transportVector = new Fractional3D(transportMatrix[0, 0], transportMatrix[1, 0], transportMatrix[2, 0]);
            return transformer.ToCartesian(transportVector) * 1.0e-10;
        }

        /// <summary>
        ///     Get the normalized electric field influence that results from a charge transport vector and the normalized
        ///     electric field vector
        /// </summary>
        /// <param name="chargeTransportVector"></param>
        /// <param name="normalizedFieldVector"></param>
        /// <returns></returns>
        protected double GetNormalizedElectricInfluence(in Cartesian3D chargeTransportVector, in Cartesian3D normalizedFieldVector)
        {
            return chargeTransportVector * normalizedFieldVector;
        }

        /// <summary>
        ///     Builds and adds the kinetic tracking model for the passed simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        protected void AddKineticTrackingModel(IKineticSimulationModel simulationModel)
        {
            var trackingModel = new SimulationTrackingModel
            {
                SimulationModel = simulationModel,
                StaticTrackerModels = CreateStaticMovementTrackerModels(simulationModel),
                GlobalTrackerModels = CreateGlobalTrackerModels(simulationModel)
            };

            simulationModel.SimulationTrackingModel = trackingModel;
        }

        /// <summary>
        ///     Creates the list of static tracker models for a kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <remarks> Requires finished linking of the passed simulation </remarks>
        /// <returns></returns>
        protected IList<IStaticMovementTrackerModel> CreateStaticMovementTrackerModels(IKineticSimulationModel simulationModel)
        {
            var result = new SetList<IStaticMovementTrackerModel>();

            foreach (var trackerModel in simulationModel.TransitionModels.SelectMany(CreateRequiredStaticTrackerModels))
                result.Add(trackerModel);

            return result;
        }

        /// <summary>
        ///     Creates the required static movement trackers that result from a single kinetic transition model (Duplicates are
        ///     removed)
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <returns></returns>
        protected ICollection<IStaticMovementTrackerModel> CreateRequiredStaticTrackerModels(IKineticTransitionModel transitionModel)
        {
            var result = new HashSet<IStaticMovementTrackerModel>();

            for (var i = 0; i < transitionModel.AbstractMovement.Count; i++)
            {
                if (transitionModel.AbstractMovement[i] == 0)
                    continue;

                foreach (var mappingModel in transitionModel.MappingModels)
                {
                    foreach (var ruleModel in transitionModel.RuleModels)
                    {
                        var trackerModel = CreateStaticTrackerByStepIndex(mappingModel, ruleModel, i);
                        trackerModel.KineticTransitionModel = transitionModel;
                        result.Add(trackerModel);
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Creates the static movement tracker for a specific transition step of a kinetic mapping model/rule model
        ///     combination
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <param name="ruleModel"></param>
        /// <param name="stepIndex"></param>
        /// <returns></returns>
        protected IStaticMovementTrackerModel CreateStaticTrackerByStepIndex(IKineticMappingModel mappingModel, IKineticRuleModel ruleModel,
            int stepIndex)
        {
            var trackerModel = new StaticMovementTrackerModel
            {
                TrackedPositionIndex = mappingModel.PositionSequence4D[stepIndex].P,
                TrackedParticle = ruleModel.StartState[stepIndex]
            };
            return trackerModel;
        }

        /// <summary>
        ///     Creates the list of raw global tracker models for a kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <remarks> Requires finished linking of the passed simulation </remarks>
        /// <returns></returns>
        protected IList<IGlobalTrackerModel> CreateGlobalTrackerModels(IKineticSimulationModel simulationModel)
        {
            var result = new List<IGlobalTrackerModel>();

            foreach (var transitionModel in simulationModel.TransitionModels)
            {
                foreach (var particle in transitionModel.GetMobileParticles())
                {
                    var trackerModel = CreateGlobalTrackerModel(transitionModel, particle);
                    result.Add(trackerModel);
                }
            }

            return result;
        }

        /// <summary>
        ///     Creates a new probability tracker model for the passed combination of transition model and tracked particle
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="trackedParticle"></param>
        /// <returns></returns>
        protected IGlobalTrackerModel CreateGlobalTrackerModel(IKineticTransitionModel transitionModel, IParticle trackedParticle)
        {
            var trackerModel = new GlobalTrackerModel
            {
                KineticTransitionModel = transitionModel,
                TrackedParticle = trackedParticle
            };
            return trackerModel;
        }
    }
}