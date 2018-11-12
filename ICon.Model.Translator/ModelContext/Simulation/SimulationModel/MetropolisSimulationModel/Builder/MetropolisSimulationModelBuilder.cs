using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
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
                simulationModel.MappingAssignMatrix = CreateMappingAssignMatrix<IMetropolisMappingModel, IMetropolisTransitionModel>(simulationModel.TransitionModels);
                simulationModel.JumpModelMatrix = CreateJumpModelMatrix(simulationModel);
            }
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
        /// Creates the jump matrix for the passed metropolis simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected IMetropolisLocalJumpModel[,,] CreateJumpModelMatrix(IMetropolisSimulationModel simulationModel)
        {
            var modelId = 0;
            var result = CreateRawJumpModelMatrix(simulationModel);
            for (var positionId = 0; positionId < result.GetLength(0); positionId++)
            {
                for (var particleId = 0; particleId < result.GetLength(1); particleId++)
                {
                    var targetIndex = 0;
                    for (var objId = 0; objId < simulationModel.MappingAssignMatrix.GetLength(2); objId++)
                    {
                        var mappingModel = simulationModel.MappingAssignMatrix[positionId, particleId, objId];
                        if (mappingModel == null)
                            break;

                        foreach (var ruleModel in mappingModel.TransitionModel.RuleModels.Where(a => a.SelectableParticle.Index == particleId))
                        {
                            var jumpModel = new MetropolisLocalJumpModel
                            {
                                ModelId = modelId++,
                                RuleModel = ruleModel,
                                MappingModel = mappingModel
                            };
                            result[positionId, particleId, targetIndex++] = jumpModel;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Determines the required size and creates the raw and empty jump model matrix
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        protected IMetropolisLocalJumpModel[,,] CreateRawJumpModelMatrix(IMetropolisSimulationModel simulationModel)
        {
            var dimension0 = simulationModel.MappingAssignMatrix.GetLength(0);
            var dimension1 = simulationModel.MappingAssignMatrix.GetLength(1);
            var dimension2 = GetMaxJumpOptionsCount(simulationModel);
            return new IMetropolisLocalJumpModel[dimension0, dimension1, dimension2];
        }

        /// <summary>
        /// Determines the maximum count of jump model options on a single position in the passed metropolis simulation model
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