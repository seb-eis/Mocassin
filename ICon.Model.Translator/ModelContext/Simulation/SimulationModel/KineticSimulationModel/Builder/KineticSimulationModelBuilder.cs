using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IKineticSimulationModelBuilder" />
    public class KineticSimulationModelBuilder : ModelBuilderBase, IKineticSimulationModelBuilder
    {
        /// <inheritdoc />
        public KineticSimulationModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IKineticSimulationModel> BuildModels(IEnumerable<IKineticSimulation> kineticSimulations)
        {
            var indexing = (0, 0, 0);
            return kineticSimulations
                .Select(BuildSimulationModel)
                .Action(model => IndexTrackerModels(model, ref indexing))
                .ToList();
        }

        /// <inheritdoc />
        public void BuildLinkingDependentComponents(IEnumerable<IKineticSimulationModel> simulationModels)
        {
            throw new NotImplementedException();
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

            AddKineticTrackingModel(simulationModel);
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
        ///     Builds and adds the kinetic tracking model for the passed simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        protected void AddKineticTrackingModel(IKineticSimulationModel simulationModel)
        {
            var trackingModel = new KineticTrackingModel
            {
                SimulationModel = simulationModel,
                GlobalTrackerModels = CreateGlobalMovementTrackerModels(simulationModel.Simulation),
                StaticTrackerModels = CreateStaticMovementTrackerModels(simulationModel.Simulation),
                ProbabilityTrackerModels = CreateProbabilityTrackerModels(simulationModel.Simulation)
            };

            simulationModel.KineticTrackingModel = trackingModel;
        }

        /// <summary>
        ///     Creates the list of raw global tracker models for a kinetic simulation object
        /// </summary>
        /// <param name="simulation"></param>
        /// <remarks> Created values contain placeholder transition models and require post-creation linking  </remarks>
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
        /// <remarks> Created values contain placeholder transition models and require post-creation linking </remarks>
        /// <returns></returns>
        protected IList<IMovementTrackerModel> CreateStaticMovementTrackerModels(IKineticSimulation simulation)
        {
            return new List<IMovementTrackerModel>();
        }

        /// <summary>
        ///     Creates the list of raw probability tracker models for a kinetic simulation
        /// </summary>
        /// <param name="simulation"></param>
        /// <remarks> Created values contain placeholder transition models and require post-creation linking </remarks>
        /// <returns></returns>
        protected IList<IProbabilityTrackerModel> CreateProbabilityTrackerModels(IKineticSimulation simulation)
        {
            return new List<IProbabilityTrackerModel>();
        }
    }
}