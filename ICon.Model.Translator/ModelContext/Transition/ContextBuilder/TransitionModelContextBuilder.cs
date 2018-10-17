using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Mocassin.Model.ModelProject;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.Comparers;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ITransitionModelContextBuilder"/>
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
        protected override ITransitionModelContext PopulateContext(ITransitionModelContext modelContext)
        {
            var manager = ModelProject.GetManager<ITransitionManager>();
            var metropolisTransitions = manager.QueryPort.Query(port => port.GetMetropolisTransitions());
            var kineticTransitions = manager.QueryPort.Query(port => port.GetKineticTransitions());

            var kineticTask = Task.Run(() => KineticTransitionModelBuilder.BuildModels(kineticTransitions));
            var metropolisTask = Task.Run(() => MetropolisTransitionModelBuilder.BuildModels(metropolisTransitions));
            var awaitTask = Task.WhenAll(kineticTask, metropolisTask);
            var positionTask = Task.Run(() => PositionTransitionModelBuilder.BuildModels(modelContext, awaitTask));

            modelContext.KineticTransitionModels = kineticTask.Result;
            modelContext.MetropolisTransitionModels = metropolisTask.Result;
            modelContext.PositionTransitionModels = positionTask.Result;
            return modelContext;
        }

        /// <inheritdoc />
        protected override ITransitionModelContext GetEmptyDefaultContext()
        {
            return new TransitionModelContext();
        }

        /// <inheritdoc />
        protected override void SetNullBuildersToDefault()
        {
            MetropolisTransitionModelBuilder = MetropolisTransitionModelBuilder ?? new MetropolisTransitionModelBuilder(ModelProject);
            KineticTransitionModelBuilder = KineticTransitionModelBuilder ?? new KineticTransitionModelBuilder(ModelProject);
            PositionTransitionModelBuilder = PositionTransitionModelBuilder ?? new PositionTransitionModelBuilder(ModelProject);
        }
    }
}
