using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <inheritdoc cref="Mocassin.Model.Translator.DbBuilder.ITransitionDbModelBuilder" />
    public class TransitionDbModelBuilder : DbModelBuilder, ITransitionDbModelBuilder
    {
        /// <inheritdoc />
        public TransitionDbModelBuilder(IProjectModelContext modelContext)
            : base(modelContext)
        {
        }

        /// <inheritdoc />
        public TransitionModel BuildModel(IKineticSimulationModel simulationModel)
        {
            var transitionModel = CreateNewModel(simulationModel.SimulationEncodingModel);
            
            transitionModel.JumpCollections = GetJumpCollectionEntities(simulationModel);    
            LinkModel(transitionModel);

            return transitionModel;
        }

        /// <inheritdoc />
        public TransitionModel BuildModel(IMetropolisSimulationModel simulationModel)
        {
            var transitionModel = CreateNewModel(simulationModel.SimulationEncodingModel);

            return transitionModel;
        }

        /// <summary>
        ///     Performs the internal linking operations for the passed transition model
        /// </summary>
        /// <param name="transitionModel"></param>
        protected void LinkModel(TransitionModel transitionModel)
        {
            transitionModel.JumpDirections = transitionModel.JumpCollections
                .Action(collection => collection.TransitionModel = transitionModel)
                .Action(collection => collection.JumpDirections.ForEach(direction => direction.JumpCollection = collection))
                .SelectMany(collection => collection.JumpDirections)
                .Action(direction => direction.TransitionModel = transitionModel)
                .ToList();
        }

        /// <summary>
        /// Get the jump count table database entity that can be extracted from the passed simulation encoding model
        /// </summary>
        /// <param name="encodingModel"></param>
        /// <returns></returns>
        protected JumpCountTableEntity GetJumpCountTableEntity(ISimulationEncodingModel encodingModel)
        {
            var entity = new JumpCountTableEntity(encodingModel.JumpCountTable);
            return entity;
        }

        /// <summary>
        /// Get the jump assign table database entity that can be extracted from the passed simulation encoding model
        /// </summary>
        /// <param name="encodingModel"></param>
        /// <returns></returns>
        protected JumpAssignTableEntity GetJumpAssignTableEntity(ISimulationEncodingModel encodingModel)
        {
            var entity = new JumpAssignTableEntity(encodingModel.JumpIndexAssignTable);
            return entity;
        }

        /// <summary>
        ///     Creates all jump collection database entities for the passed kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected List<JumpCollectionEntity> GetJumpCollectionEntities(IKineticSimulationModel simulationModel)
        {
            return simulationModel.TransitionModels
                .Select(x => GetJumpCollectionEntity(x, simulationModel))
                .ToList();
        }

        /// <summary>
        ///     Creates a jump collection database entity for the passed kinetic transition model in the context of the passed
        ///     simulation model
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected JumpCollectionEntity GetJumpCollectionEntity(IKineticTransitionModel transitionModel,
            IKineticSimulationModel simulationModel)
        {
            var entity = new JumpCollectionEntity
            {
                ObjectId = simulationModel.SimulationEncodingModel.TransitionModelToJumpCollectionId[transitionModel],
                ParticleMask = transitionModel.SelectableParticleMask,
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
        protected List<JumpDirectionEntity> GetJumpDirectionEntities(IKineticTransitionModel transitionModel,
            IKineticSimulationModel simulationModel)
        {
            var directions = transitionModel.MappingModels
                .Select(x => GetJumpDirectionEntity(x, simulationModel))
                .ToList();

            return directions;
        }

        /// <summary>
        ///     Translates a kinetic mapping model into a jump direction database entity in the context of the provided kinetic
        ///     simulation model
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected JumpDirectionEntity GetJumpDirectionEntity(IKineticMappingModel mappingModel, IKineticSimulationModel simulationModel)
        {
            var entity = new JumpDirectionEntity
            {
                JumpLength = mappingModel.TransitionSequence4D.Count,
                PositionId = mappingModel.StartVector4D.P,
                CollectionId = simulationModel.SimulationEncodingModel.TransitionModelToJumpCollectionId[mappingModel.TransitionModel],
                ObjectId = simulationModel.SimulationEncodingModel.TransitionMappingToJumpDirectionId[mappingModel],
                FieldProjectionFactor = simulationModel.SimulationEncodingModel.TransitionMappingToElectricFieldFactors[mappingModel],
                JumpSequence = GetJumpSequenceEntity(mappingModel.TransitionSequence4D),
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
        ///     Creates the movement sequence database entity for the passed kinetic mapping model
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <returns></returns>
        protected MoveSequenceEntity GetMovementSequenceEntity(IKineticMappingModel mappingModel)
        {
            var cMoveVectors = mappingModel.GetMovementSequence()
                .Select(x => new CMoveVector(x))
                .ToList();

            return new MoveSequenceEntity {Values = cMoveVectors};
        }

        /// <summary>
        ///     Get the jump rule list database entity that results for the passed kinetic transition model in the context of the
        ///     passed simulation model
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected JumpRuleListEntity GetJumpRuleListEntity(IKineticTransitionModel transitionModel, IKineticSimulationModel simulationModel)
        {
            var jumpRules = new List<CJumpRule>(transitionModel.RuleModels.Count);
            foreach (var ruleModel in transitionModel.RuleModels)
            {
                var jumpRule = GetBasicJumpRule(ruleModel);
                jumpRule.StateCode1 = ruleModel.TransitionStateCode;
                jumpRule.AttemptFrequencyFraction = ruleModel.KineticRule.AttemptFrequency / simulationModel.MaxAttemptFrequency;
                jumpRule.ElectricFieldFactor = simulationModel.SimulationEncodingModel.TransitionRuleToElectricFieldFactors[ruleModel];
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
        ///     Creates a default constructed transition database model that conforms to te passed simulation encoding model
        /// </summary>
        /// <param name="encodingModel"></param>
        /// <returns></returns>
        protected TransitionModel CreateNewModel(ISimulationEncodingModel encodingModel)
        {
            var model = new TransitionModel
            {
                JumpCountTable = GetJumpCountTableEntity(encodingModel),
                JumpAssignTable = GetJumpAssignTableEntity(encodingModel)
            };

            return model;
        }
    }
}