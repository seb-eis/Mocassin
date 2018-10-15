using System.Collections.Generic;
using System.Linq;
using ICon.Framework.Extensions;
using ICon.Model.ProjectServices;
using ICon.Model.Simulations;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IKineticSimulationModelBuilder"/>
    public class KineticSimulationModelBuilder : ModelBuilderBase, IKineticSimulationModelBuilder
    {
        /// <inheritdoc />
        public KineticSimulationModelBuilder(IProjectServices projectServices)
            : base(projectServices)
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

        /// <summary>
        /// Builds a single kinetic simulation model for the passed kinetic simulation
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
        /// Indexes the passed simulation model using the passed indexing set reference
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="indexing"></param>
        protected void IndexTrackerModels(IKineticSimulationModel simulationModel, ref (int Global, int Static, int Probability) indexing)
        {
            foreach (var globalTrackerModel in simulationModel.KineticTrackingModel.GlobalTrackerModels)
            {
                globalTrackerModel.ModelId = indexing.Global++;
            }

            foreach (var staticTrackerModel in simulationModel.KineticTrackingModel.StaticTrackerModels)
            {
                staticTrackerModel.ModelId = indexing.Static++;
            }

            foreach (var probabilityTrackerModel in simulationModel.KineticTrackingModel.ProbabilityTrackerModels)
            {
                probabilityTrackerModel.ModelId = indexing.Probability++;
            }
        }

        /// <summary>
        /// Builds and adds the kinetic tracking model for the passed simulation model
        /// </summary>
        /// <param name="simulationModel"></param>
        protected void AddKineticTrackingModel(IKineticSimulationModel simulationModel)
        {
            var trackingModel = new KineticTrackingModel
            {
                SimulationModel = simulationModel,
                GlobalTrackerModels = CreateGlobalTrackerModels(simulationModel.Simulation),
                StaticTrackerModels = CreateStaticTrackerModels(simulationModel.Simulation),
                ProbabilityTrackerModels = CreateProbabilityTrackerModels(simulationModel.Simulation)
            };

            simulationModel.KineticTrackingModel = trackingModel;
        }

        /// <summary>
        /// Creates the list of raw global tracker models for a kinetic simulation object
        /// </summary>
        /// <param name="simulation"></param>
        /// <remarks> Created values contain placeholder transition models and require post-creation linking  </remarks>
        /// <returns></returns>
        protected IList<IGlobalTrackerModel> CreateGlobalTrackerModels(IKineticSimulation simulation)
        {
            return null;
        }

        /// <summary>
        /// Creates the list of static tracker models for a kinetic simulation object
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        protected IList<IStaticTrackerModel> CreateStaticTrackerModels(IKineticSimulation simulation)
        {
            return null;
        }

        /// <summary>
        /// Creates the list of raw probability tracker models for a kinetic simulation
        /// </summary>
        /// <param name="simulation"></param>
        /// <remarks> Created values contain placeholder transition models and require post-creation linking </remarks>
        /// <returns></returns>
        protected IList<IProbabilityTrackerModel> CreateProbabilityTrackerModels(IKineticSimulation simulation)
        {
            return null;
        }
    }
}