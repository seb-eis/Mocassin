using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ICon.Model.Simulations;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.ISimulationModelContextBuilder"/>
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
            var manager = ProjectServices.GetManager<ISimulationManager>();
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
            MetropolisSimulationModelBuilder = MetropolisSimulationModelBuilder ?? new MetropolisSimulationModelBuilder(ProjectServices);
            KineticSimulationModelBuilder = KineticSimulationModelBuilder ?? new KineticSimulationModelBuilder(ProjectServices);
        }

        /// <inheritdoc />
        public override Task BuildLinkDependentComponents()
        {
            var tasks = new[]
            {
                Task.Run(
                    () => MetropolisSimulationModelBuilder.BuildLinkingDependentComponents(BuildTask.Result.MetropolisSimulationModels))
            };
            return Task.WhenAll(tasks);
        }
    }
}
