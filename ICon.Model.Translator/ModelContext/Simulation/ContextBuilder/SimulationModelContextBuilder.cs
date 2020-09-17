using System.Linq;
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
        public override bool CheckBuildRequirements()
        {
            return ModelProject?.Managers().Any(x => x is ISimulationManager) ?? false;
        }

        /// <inheritdoc />
        public override Task BuildLinkDependentComponents()
        {
            if (!CheckLinkDependentBuildRequirements()) return Task.CompletedTask;
            var tasks = new[]
            {
                Task.Run(
                    () => MetropolisSimulationModelBuilder.BuildLinkingDependentComponents(BuildTask.Result.MetropolisSimulationModels)),
                Task.Run(
                    () => KineticSimulationModelBuilder.BuildLinkingDependentComponents(BuildTask.Result.KineticSimulationModels))
            };
            return Task.WhenAll(tasks);
        }

        /// <inheritdoc />
        protected override ISimulationModelContext PopulateContext(ISimulationModelContext modelContext)
        {
            if (!CheckBuildRequirements()) return modelContext;

            var manager = ModelProject.Manager<ISimulationManager>();
            var metropolisSimulations = manager.DataAccess.Query(port => port.GetMetropolisSimulations());
            var kineticSimulations = manager.DataAccess.Query(port => port.GetKineticSimulations());

            modelContext.MetropolisSimulationModels = MetropolisSimulationModelBuilder.BuildModels(metropolisSimulations);
            modelContext.KineticSimulationModels = KineticSimulationModelBuilder.BuildModels(kineticSimulations);
            return modelContext;
        }

        /// <inheritdoc />
        protected override ISimulationModelContext GetEmptyDefaultContext() => new SimulationModelContext();

        /// <inheritdoc />
        protected override void SetNullBuildersToDefault()
        {
            MetropolisSimulationModelBuilder ??= new MetropolisSimulationModelBuilder(ModelProject);
            KineticSimulationModelBuilder ??= new KineticSimulationModelBuilder(ModelProject);
        }
    }
}