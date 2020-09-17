using System.Linq;
using System.Threading.Tasks;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class ProjectModelContextLinker : IProjectModelContextLinker
    {
        /// <summary>
        ///     The project model context builder that creates the linkable context components
        /// </summary>
        protected IProjectModelContextBuilder ProjectModelContextBuilder { get; set; }

        /// <summary>
        ///     The simulation model context that is linked
        /// </summary>
        protected IStructureModelContext StructureModelContext { get; set; }

        /// <summary>
        ///     The simulation model context that is linked
        /// </summary>
        protected IEnergyModelContext EnergyModelContext { get; set; }

        /// <summary>
        ///     The simulation model context that is linked
        /// </summary>
        protected ITransitionModelContext TransitionModelContext { get; set; }

        /// <summary>
        ///     The simulation model context that is linked
        /// </summary>
        protected ISimulationModelContext SimulationModelContext { get; set; }

        /// <inheritdoc />
        public async Task LinkContextComponents(IProjectModelContextBuilder contextBuilder)
        {
            ProjectModelContextBuilder = contextBuilder;
            await SetContextBuildResults();
            LinkStructureComponents();
            LinkSimulationComponents();
        }

        /// <summary>
        ///     Links all externally build structure related components into the structure context object
        /// </summary>
        /// <returns></returns>
        protected void LinkStructureComponents()
        {
            LinkPositionTransitionModelsIntoStructure();
            LinkEnergyModelsIntoStructure();
        }

        /// <summary>
        ///     Links the position transition models into the structure context
        /// </summary>
        protected void LinkPositionTransitionModelsIntoStructure()
        {
            foreach (var positionModel in StructureModelContext.PositionModels)
            {
                positionModel.PositionTransitionModel = TransitionModelContext.PositionTransitionModels
                                                                              .Single(a => a.CellSite == positionModel.CellSite);
            }
        }

        /// <summary>
        ///     Links the pair and group energy models into the structure context
        /// </summary>
        protected void LinkEnergyModelsIntoStructure()
        {
            foreach (var environmentModel in StructureModelContext.EnvironmentModels)
            {
                foreach (var pairInteractionModel in environmentModel.PairInteractionModels)
                {
                    pairInteractionModel.PairEnergyModel = EnergyModelContext.PairEnergyModels
                                                                             .Single(a => a.PairInteraction ==
                                                                                          pairInteractionModel.PairEnergyModel.PairInteraction);
                }

                foreach (var groupInteractionModel in environmentModel.GroupInteractionModels)
                {
                    groupInteractionModel.GroupEnergyModel = EnergyModelContext.GroupEnergyModels
                                                                               .Single(a => a.GroupInteraction ==
                                                                                            groupInteractionModel.GroupEnergyModel.GroupInteraction);
                }
            }
        }

        /// <summary>
        ///     Links the simulation context components to their affiliated objects from other context objects
        /// </summary>
        protected void LinkSimulationComponents()
        {
            LinkTransitionsIntoMetropolisModel();
            LinkTransitionsIntoKineticModel();
        }

        /// <summary>
        ///     Links the transition models into the metropolis simulations by replacing internal placeholders
        /// </summary>
        protected void LinkTransitionsIntoMetropolisModel()
        {
            var transitionModels = TransitionModelContext.MetropolisTransitionModels;
            foreach (var simulationModel in SimulationModelContext.MetropolisSimulationModels)
            {
                simulationModel.TransitionModels = simulationModel.TransitionModels
                                                                  .SelectMany(a => transitionModels.Where(b => b.Transition == a.Transition))
                                                                  .ToList();
            }
        }

        /// <summary>
        ///     Links the transition models into the kinetic simulations by replacing internal placeholders
        /// </summary>
        protected void LinkTransitionsIntoKineticModel()
        {
            var transitionModels = TransitionModelContext.KineticTransitionModels;
            foreach (var simulationModel in SimulationModelContext.KineticSimulationModels)
            {
                simulationModel.TransitionModels = simulationModel.TransitionModels
                                                                  .SelectMany(a => transitionModels.Where(b => b.Transition == a.Transition))
                                                                  .ToList();
            }
        }

        /// <summary>
        ///     Awaits and sets the main context build results for linking
        /// </summary>
        /// <returns></returns>
        protected async Task SetContextBuildResults()
        {
            await Task.WhenAll(
                ProjectModelContextBuilder.StructureModelContextBuilder.BuildTask,
                ProjectModelContextBuilder.EnergyModelContextBuilder.BuildTask,
                ProjectModelContextBuilder.TransitionModelContextBuilder.BuildTask,
                ProjectModelContextBuilder.SimulationModelContextBuilder.BuildTask);

            StructureModelContext = ProjectModelContextBuilder.StructureModelContextBuilder.BuildTask.Result;
            EnergyModelContext = ProjectModelContextBuilder.EnergyModelContextBuilder.BuildTask.Result;
            TransitionModelContext = ProjectModelContextBuilder.TransitionModelContextBuilder.BuildTask.Result;
            SimulationModelContext = ProjectModelContextBuilder.SimulationModelContextBuilder.BuildTask.Result;
        }
    }
}