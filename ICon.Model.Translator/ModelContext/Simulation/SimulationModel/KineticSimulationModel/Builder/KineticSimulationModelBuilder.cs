﻿using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
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
            var indexing = (0, 0, 0);
            foreach (var simulationModel in simulationModels)
            {
                simulationModel.MappingAssignMatrix =
                    CreateMappingAssignMatrix<IKineticMappingModel, IKineticTransitionModel>(simulationModel.TransitionModels);
                AddLocalJumpModels(simulationModel);
                AddKineticTrackingModel(simulationModel);
                IndexTrackerModels(simulationModel, ref indexing);
            }
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
        ///     Indexes the passed simulation model using the passed indexing set reference
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="indexing"></param>
        protected void IndexTrackerModels(IKineticSimulationModel simulationModel, ref (int Global, int Static, int Probability) indexing)
        {
            foreach (var globalTrackerModel in simulationModel.KineticTrackingModel.GlobalTrackerModels)
                globalTrackerModel.ModelId = indexing.Global++;

            foreach (var staticTrackerModel in simulationModel.KineticTrackingModel.StaticTrackerModels)
                staticTrackerModel.ModelId = indexing.Static++;

            foreach (var probabilityTrackerModel in simulationModel.KineticTrackingModel.ProbabilityTrackerModels)
                probabilityTrackerModel.ModelId = indexing.Probability++;
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

            jumpModel.NormalizedElectricFieldInfluence =
                GetNormalizedElectricInfluence(jumpModel.ChargeTransportVector, simulationModel.NormalizedElectricFieldVector);

            return jumpModel;
        }

        /// <summary>
        ///     Get the charge transport vector [charge*movement] for the passed combination of kinetic mapping model and kinetic
        ///     rule model
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <param name="ruleModel"></param>
        /// <returns></returns>
        protected Cartesian3D GetChargeTransportVector(IKineticMappingModel mappingModel, IKineticRuleModel ruleModel)
        {
            var transportMatrix = mappingModel.PositionMovementMatrix * ruleModel.ChargeTransportMatrix.GetTransposed();
            var transportVector = new Cartesian3D(transportMatrix[0, 0], transportMatrix[1, 0], transportMatrix[2, 0]);
            return transportVector;
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
            var trackingModel = new KineticTrackingModel
            {
                SimulationModel = simulationModel,
                StaticTrackerModels = CreateStaticMovementTrackerModels(simulationModel.Simulation),              
                GlobalTrackerModels = CreateGlobalMovementTrackerModels(simulationModel.Simulation),
                ProbabilityTrackerModels = CreateProbabilityTrackerModels(simulationModel.Simulation)
            };

            simulationModel.KineticTrackingModel = trackingModel;
        }

        /// <summary>
        ///     Creates the list of raw global tracker models for a kinetic simulation object
        /// </summary>
        /// <param name="simulation"></param>
        /// <remarks> Requires finished linking of the passed simulation </remarks>
        /// <returns></returns>
        protected IList<IMovementTrackerModel> CreateGlobalMovementTrackerModels(IKineticSimulation simulation)
        {
            return simulation.Transitions
                .Select(transition => new KineticTransitionModel {Transition = transition})
                .Select(transitionModel => new MovementTrackerModel {KineticTransitionModel = transitionModel})
                .Cast<IMovementTrackerModel>()
                .ToList();
        }

        /// <summary>
        ///     Creates the list of static tracker models for a kinetic simulation object
        /// </summary>
        /// <param name="simulation"></param>
        /// <remarks> Requires finished linking of the passed simulation </remarks>
        /// <returns></returns>
        protected IList<IStaticMovementTrackerModel> CreateStaticMovementTrackerModels(IKineticSimulation simulation)
        {
            var result = new List<IStaticMovementTrackerModel>();

            return result;
        }

        /// <summary>
        ///     Creates the list of raw probability tracker models for a kinetic simulation
        /// </summary>
        /// <param name="simulation"></param>
        /// <remarks> Requires finished linking of the passed simulation </remarks>
        /// <returns></returns>
        protected IList<IProbabilityTrackerModel> CreateProbabilityTrackerModels(IKineticSimulation simulation)
        {
            var result = new List<IProbabilityTrackerModel>();

            return result;
        }
    }
}