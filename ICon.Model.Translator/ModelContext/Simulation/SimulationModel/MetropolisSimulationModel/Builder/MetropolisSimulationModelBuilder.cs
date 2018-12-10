using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IMetropolisSimulationModelBuilder" />
    public class MetropolisSimulationModelBuilder : SimulationModelBuilderBase, IMetropolisSimulationModelBuilder
    {
        /// <inheritdoc />
        public MetropolisSimulationModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IMetropolisSimulationModel> BuildModels(IEnumerable<IMetropolisSimulation> metropolisSimulations)
        {
            return metropolisSimulations
                .Select(a => new MetropolisSimulationModel {Simulation = a})
                .Action(a => a.TransitionModels = CreateTransitionModelPlaceholders(a.Simulation.Transitions))
                .Cast<IMetropolisSimulationModel>()
                .ToList();
        }

        /// <inheritdoc />
        public void BuildLinkingDependentComponents(IEnumerable<IMetropolisSimulationModel> simulationModels)
        {
            foreach (var simulationModel in simulationModels)
            {
                simulationModel.MappingAssignMatrix =
                    CreateMappingAssignMatrix<IMetropolisMappingModel, IMetropolisTransitionModel>(simulationModel.TransitionModels);

                AddLocalJumpModels(simulationModel);
                AddEncodingModel(simulationModel);
            }
        }

        /// <summary>
        ///     Creates and adds the kinetic indexing model for the passed and fully created simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        private void AddEncodingModel(IMetropolisSimulationModel simulationModel)
        {
            var encodingModel = new SimulationEncodingModel
            {
                TransitionModelToJumpCollectionId = GetTransitionModelIndexing(simulationModel.TransitionModels),
                TransitionMappingToJumpDirectionId = GetTransitionMappingIndexing(simulationModel.TransitionModels),
                JumpCountTable = GetJumpCountTable(simulationModel.MappingAssignMatrix, simulationModel.LocalJumpModels),
                PositionIndexToMobilityTypesSet = GetPositionIndexToMobilitySetMapping(simulationModel.LocalJumpModels)
            };

            AddElectricFieldInfluenceMappings(encodingModel, simulationModel.LocalJumpModels);
            AddJumpIndexAssignTable(encodingModel, simulationModel.MappingAssignMatrix);
            simulationModel.SimulationEncodingModel = encodingModel;
        }

        /// <summary>
        ///     Takes a sequence of metropolis transitions and creates placeholder transition models for the later linking process
        /// </summary>
        /// <param name="transitions"></param>
        /// <returns></returns>
        protected IList<IMetropolisTransitionModel> CreateTransitionModelPlaceholders(IEnumerable<IMetropolisTransition> transitions)
        {
            return transitions
                .Select(a => new MetropolisTransitionModel {Transition = a})
                .Cast<IMetropolisTransitionModel>()
                .ToList();
        }

        /// <summary>
        ///     Creates and adds the list of metropolis local simulation models to the passed kinetic simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        protected void AddLocalJumpModels(IMetropolisSimulationModel simulationModel)
        {
            var jumpModels = new List<IMetropolisLocalJumpModel>();
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

            jumpModels.RemoveDuplicates(EqualityComparer<IMetropolisLocalJumpModel>.Default);
            simulationModel.LocalJumpModels = jumpModels;
        }

        /// <summary>
        ///     Creates the local jump model for the provided combination of metropolis rule model and mapping model in the context
        ///     of the defined metropolis simulation model
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <param name="ruleModel"></param>
        /// <param name="simulationModel"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        protected IMetropolisLocalJumpModel CreateLocalJumpModel(IMetropolisMappingModel mappingModel, IMetropolisRuleModel ruleModel,
            IMetropolisSimulationModel simulationModel, ref int modelId)
        {
            var jumpModel = new MetropolisLocalJumpModel
            {
                ModelId = modelId++,
                MappingModel = mappingModel,
                RuleModel = ruleModel
            };

            return jumpModel;
        }

        /// <summary>
        ///     Determines the maximum count of jump model options on a single position in the passed metropolis simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected int GetMaxJumpOptionsCount(IMetropolisSimulationModel simulationModel)
        {
            var result = 0;
            for (var positionId = 0; positionId < simulationModel.MappingAssignMatrix.GetLength(0); positionId++)
            {
                for (var particleId = 0; particleId < simulationModel.MappingAssignMatrix.GetLength(1); particleId++)
                {
                    var count = 0;
                    for (var objId = 0; objId < simulationModel.MappingAssignMatrix.GetLength(2); objId++)
                    {
                        var mappingModel = simulationModel.MappingAssignMatrix[positionId, particleId, objId];
                        if (mappingModel == null)
                            break;

                        count += mappingModel.TransitionModel.RuleModels
                            .Count(ruleModel => ruleModel.SelectableParticle.Index == particleId);
                    }

                    result = result > count ? result : count;
                }
            }

            return result;
        }
    }
}