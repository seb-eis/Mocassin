using System.Threading.Tasks;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ISimulationModelContextBuilder" />
    public class SimulationModelContextBuilder : ModelContextBuilderBase<ISimulationModelContext>, ISimulationModelContextBuilder
    {
        /// <inheritdoc />
        public IMetropolisSimulationModelBuilder MetropolisSimulationModelBuilder { get; set; }

        /// <inheritdoc />
        public IKineticSimulationModelBuilder KineticSimulationModelBuilder { get; set; }

        /// <inheritdoc />
        public SimulationModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder)
            : base(projectModelContextBuilder)
        {
        }

        /// <inheritdoc />
        protected override ISimulationModelContext PopulateContext(ISimulationModelContext modelContext)
        {
            var manager = ModelProject.GetManager<ISimulationManager>();
            var metropolisSimulations = manager.QueryPort.Query(port => port.GetMetropolisSimulations());
            var kineticSimulations = manager.QueryPort.Query(port => port.GetKineticSimulations());

            modelContext.MetropolisSimulationModels = MetropolisSimulationModelBuilder.BuildModels(metropolisSimulations);
            modelContext.KineticSimulationModels = KineticSimulationModelBuilder.BuildModels(kineticSimulations);
            return modelContext;
        }

        /// <inheritdoc />
        protected override ISimulationModelContext GetEmptyDefaultContext()
        {
            return new SimulationModelContext();
        }

        /// <inheritdoc />
        protected override void SetNullBuildersToDefault()
        {
            MetropolisSimulationModelBuilder = MetropolisSimulationModelBuilder ?? new MetropolisSimulationModelBuilder(ModelProject);
            KineticSimulationModelBuilder = KineticSimulationModelBuilder ?? new KineticSimulationModelBuilder(ModelProject);
        }

        /// <inheritdoc />
        public override Task BuildLinkDependentComponents()
        {
            var tasks = new[]
            {
                Task.Run(
                    () => MetropolisSimulationModelBuilder.BuildLinkingDependentComponents(BuildTask.Result.MetropolisSimulationModels)),
                Task.Run(
                    () => KineticSimulationModelBuilder.BuildLinkingDependentComponents(BuildTask.Result.KineticSimulationModels))
            };
            return Task.WhenAll(tasks);
        }
    }
}