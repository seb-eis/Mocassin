using System.Linq;
using System.Threading.Tasks;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ITransitionModelContextBuilder" />
    public class TransitionModelContextBuilder : ModelContextBuilderBase<ITransitionModelContext>, ITransitionModelContextBuilder
    {
        /// <inheritdoc />
        public IMetropolisTransitionModelBuilder MetropolisTransitionModelBuilder { get; set; }

        /// <inheritdoc />
        public IKineticTransitionModelBuilder KineticTransitionModelBuilder { get; set; }

        /// <inheritdoc />
        public IPositionTransitionModelBuilder PositionTransitionModelBuilder { get; set; }

        /// <inheritdoc />
        public TransitionModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder)
            : base(projectModelContextBuilder)
        {
        }

        /// <inheritdoc />
        public override bool CheckBuildRequirements()
        {
            return ModelProject?.Managers().Any(x => x is ITransitionManager) ?? false;
        }

        /// <inheritdoc />
        protected override ITransitionModelContext PopulateContext(ITransitionModelContext modelContext)
        {
            if (!CheckBuildRequirements()) return modelContext;

            var manager = ModelProject.Manager<ITransitionManager>();
            var metropolisTransitions = manager.DataAccess.Query(port => port.GetMetropolisTransitions());
            var kineticTransitions = manager.DataAccess.Query(port => port.GetKineticTransitions());

            var kineticTask = Task.Run(
                () => modelContext.KineticTransitionModels = KineticTransitionModelBuilder.BuildModels(kineticTransitions));

            var metropolisTask = Task.Run(
                () => modelContext.MetropolisTransitionModels = MetropolisTransitionModelBuilder.BuildModels(metropolisTransitions));

            var awaitTask = Task.WhenAll(kineticTask, metropolisTask);
            var positionTask = Task.Run(() => PositionTransitionModelBuilder.BuildModels(modelContext, awaitTask));

            modelContext.PositionTransitionModels = positionTask.Result;
            return modelContext;
        }

        /// <inheritdoc />
        protected override ITransitionModelContext GetEmptyDefaultContext() => new TransitionModelContext();

        /// <inheritdoc />
        protected override void SetNullBuildersToDefault()
        {
            MetropolisTransitionModelBuilder ??= new MetropolisTransitionModelBuilder(ModelProject);
            KineticTransitionModelBuilder ??= new KineticTransitionModelBuilder(ModelProject);
            PositionTransitionModelBuilder ??= new PositionTransitionModelBuilder(ModelProject);
        }
    }
}