using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ICon.Model.ProjectServices;
using ICon.Model.Transitions;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using ICon.Mathematics.Coordinates;
using ICon.Framework.Extensions;
using ICon.Model.Particles;
using ICon.Framework.Collections;
using ICon.Mathematics.Comparers;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.ITransitionModelContextBuilder"/>
    public class TransitionModelContextBuilder : ModelContextBuilderBase<ITransitionModelContext>, ITransitionModelContextBuilder
    {
        /// <inheritdoc />
        public IMetropolisTransitionModelBuilder MetropolisTransitionModelBuilder { get; set; }

        /// <inheritdoc />
        public IKineticTransitionModelBuilder KineticTransitionModelBuilder { get; set; }

        /// <inheritdoc />
        public  IPositionTransitionModelBuilder PositionTransitionModelBuilder { get; set; }

        /// <inheritdoc />
        public TransitionModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder) : base(projectModelContextBuilder)
        {
            MetropolisTransitionModelBuilder = new MetropolisTransitionModelBuilder(ProjectServices);
            KineticTransitionModelBuilder = new KineticTransitionModelBuilder(ProjectServices);
            PositionTransitionModelBuilder = new PositionTransitionModelBuilder(ProjectServices);
        }

        /// <inheritdoc />
        protected override ITransitionModelContext PopulateContext(ITransitionModelContext modelContext)
        {
            var manager = ProjectServices.GetManager<ITransitionManager>();
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
    }
}
