using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.EntityBuilder
{
    /// <inheritdoc cref="ITransitionDbEntityBuilder" />
    public class TransitionDbEntityBuilder : DbEntityBuilder, ITransitionDbEntityBuilder
    {
        /// <inheritdoc />
        public TransitionDbEntityBuilder(IProjectModelContext modelContext)
            : base(modelContext)
        {
        }

        /// <inheritdoc />
        public SimulationTransitionModel BuildModel(ISimulationModel simulationModel)
        {
            var transitionModel = CreateNewModel(simulationModel);

            transitionModel.JumpCollections = GetJumpCollectionEntities(simulationModel);
            LinkModel(transitionModel);

            return transitionModel;
        }

        /// <summary>
        ///     Performs the internal linking operations for the passed transition model
        /// </summary>
        /// <param name="simulationTransitionModel"></param>
        protected void LinkModel(SimulationTransitionModel simulationTransitionModel)
        {
            simulationTransitionModel.JumpDirections = simulationTransitionModel.JumpCollections
                .Action(collection => collection.SimulationTransitionModel = simulationTransitionModel)
                .Action(collection => collection.JumpDirections.ForEach(direction => direction.JumpCollection = collection))
                .SelectMany(collection => collection.JumpDirections)
                .Action(direction => direction.SimulationTransitionModel = simulationTransitionModel)
                .ToList();
        }

        /// <summary>
        ///     Get the jump count table database entity that can be extracted from the passed simulation encoding model
        /// </summary>
        /// <param name="encodingModel"></param>
        /// <returns></returns>
        protected JumpCountTableEntity GetJumpCountTableEntity(ISimulationEncodingModel encodingModel)
        {
            return new JumpCountTableEntity(encodingModel.JumpCountTable);
        }

        /// <summary>
        ///     Get the jump assign table database entity that can be extracted from the passed simulation encoding model
        /// </summary>
        /// <param name="encodingModel"></param>
        /// <returns></returns>
        protected JumpAssignTableEntity GetJumpAssignTableEntity(ISimulationEncodingModel encodingModel)
        {
            return new JumpAssignTableEntity(encodingModel.JumpIndexAssignTable);
        }

        /// <summary>
        ///     Get the static tracker assign table database entity that can be extracted from the passed simulation tracking model
        /// </summary>
        /// <param name="trackingModel"></param>
        /// <returns></returns>
        protected TrackerAssignTableEntity GetStaticTrackerAssignTableEntity(ISimulationTrackingModel trackingModel)
        {
            return new TrackerAssignTableEntity(trackingModel.StaticTrackerMappingTable);
        }

        /// <summary>
        ///     Get the global tracker assign table database entity that can be extracted from the passed simulation tracking model
        /// </summary>
        /// <param name="trackingModel"></param>
        /// <returns></returns>
        protected TrackerAssignTableEntity GetGlobalTrackerAssignTableEntity(ISimulationTrackingModel trackingModel)
        {
            return new TrackerAssignTableEntity(trackingModel.GlobalTrackerMappingTable);
        }

        /// <summary>
        ///     Creates all jump collection database entities for the passed simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected List<JumpCollectionEntity> GetJumpCollectionEntities(ISimulationModel simulationModel)
        {
            return simulationModel.GetTransitionModels()
                .Select(x => GetJumpCollectionEntity(x, simulationModel))
                .ToList();
        }

        /// <summary>
        ///     Creates a jump collection database entity for the passed transition model in the context of the passed simulation
        ///     model
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected JumpCollectionEntity GetJumpCollectionEntity(ITransitionModel transitionModel, ISimulationModel simulationModel)
        {
            var entity = new JumpCollectionEntity
            {
                ObjectId = simulationModel.SimulationEncodingModel.TransitionModelToJumpCollectionId[transitionModel],
                SelectableParticlesMask = transitionModel.SelectableParticlesMask,
                JumpDirections = GetJumpDirectionEntities(transitionModel, simulationModel),
                JumpRuleList = GetJumpRuleListEntity(transitionModel, simulationModel)
            };

            return entity;
        }

        /// <summary>
        ///     Get all jump direction database entities that are described by the passed transition model in the context of the
        ///     defined simulation model
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected List<JumpDirectionEntity> GetJumpDirectionEntities(ITransitionModel transitionModel, ISimulationModel simulationModel)
        {
            var directions = transitionModel.GetMappingModels()
                .Select(x => GetJumpDirectionEntity(x, simulationModel))
                .ToList();

            return directions;
        }

        /// <summary>
        ///     Translates a mapping model into a jump direction database entity in the context of the provided kinetic
        ///     simulation model
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected JumpDirectionEntity GetJumpDirectionEntity(ITransitionMappingModel mappingModel, ISimulationModel simulationModel)
        {
            var entity = new JumpDirectionEntity
            {
                PathLength = mappingModel.PathLength,
                PositionId = mappingModel.StartVector4D.P,
                CollectionId = simulationModel.SimulationEncodingModel.TransitionModelToJumpCollectionId[mappingModel.GetTransitionModel()],
                ObjectId = simulationModel.SimulationEncodingModel.TransitionMappingToJumpDirectionId[mappingModel],
                FieldProjectionFactor = simulationModel.SimulationEncodingModel.TransitionMappingToElectricFieldFactors[mappingModel],
                JumpSequence = GetJumpSequenceEntity(mappingModel.GetTransitionSequence()),
                MovementSequence = GetMovementSequenceEntity(mappingModel)
            };

            return entity;
        }

        /// <summary>
        ///     Converts a set of encoded crystal vectors into a jump sequence database entity for the simulation
        /// </summary>
        /// <param name="crystalVectors"></param>
        /// <returns></returns>
        protected JumpSequenceEntity GetJumpSequenceEntity(IEnumerable<CrystalVector4D> crystalVectors)
        {
            var cVectors = crystalVectors.Select(x => new CVector4(x)).ToList();
            return new JumpSequenceEntity {Values = cVectors};
        }

        /// <summary>
        ///     Creates the movement sequence database entity for the passed mapping model
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <returns></returns>
        protected MoveSequenceEntity GetMovementSequenceEntity(ITransitionMappingModel mappingModel)
        {
            var cMoveVectors = mappingModel.GetMovementSequence()
                .Select(x => new CMoveVector(x))
                .ToList();

            return new MoveSequenceEntity {Values = cMoveVectors};
        }

        /// <summary>
        ///     Get the jump rule list database entity that results for the passed transition model in the context of the passed
        ///     simulation model
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected JumpRuleListEntity GetJumpRuleListEntity(ITransitionModel transitionModel, ISimulationModel simulationModel)
        {
            var jumpRules = new List<CJumpRule>();
            foreach (var ruleModel in transitionModel.GetRuleModels())
            {
                var jumpRule = GetBasicJumpRule(ruleModel);
                jumpRule.StateCode1 = ruleModel.TransitionStateCode;
                jumpRule.AttemptFrequencyFraction = ruleModel.AttemptFrequency / simulationModel.MaxAttemptFrequency;
                jumpRule.ElectricFieldFactor = simulationModel.SimulationEncodingModel.TransitionRuleToElectricFieldFactors[ruleModel];
                jumpRules.Add(jumpRule);
            }

            return new JumpRuleListEntity {Values = jumpRules};
        }

        /// <summary>
        ///     Creates a basic jump rule interop object from the passed transition rule model
        /// </summary>
        /// <param name="ruleModel"></param>
        /// <returns></returns>
        protected CJumpRule GetBasicJumpRule(ITransitionRuleModel ruleModel)
        {
            var trackerOder = BitConverter.GetBytes(ruleModel.FinalTrackerOrderCode);
            var cJumpRule = new CJumpRule
            {
                TrackerOrder = trackerOder,
                StateCode0 = ruleModel.StartStateCode,
                StateCode2 = ruleModel.FinalStateCode
            };

            return cJumpRule;
        }

        /// <summary>
        ///     Creates a default constructed transition database model that conforms to te passed simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected SimulationTransitionModel CreateNewModel(ISimulationModel simulationModel)
        {
            var model = new SimulationTransitionModel
            {
                JumpCountTable = GetJumpCountTableEntity(simulationModel.SimulationEncodingModel),
                JumpAssignTable = GetJumpAssignTableEntity(simulationModel.SimulationEncodingModel),
                GlobalTrackerAssignTable = GetGlobalTrackerAssignTableEntity(simulationModel.SimulationTrackingModel),
                StaticTrackerAssignTable = GetStaticTrackerAssignTableEntity(simulationModel.SimulationTrackingModel)
            };

            return model;
        }
    }
}