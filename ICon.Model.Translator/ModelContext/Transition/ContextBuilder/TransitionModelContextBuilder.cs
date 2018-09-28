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
    /// <summary>
    /// Transition model context builder. Extends the reference transition model information into a full data context
    /// </summary>
    public class TransitionModelContextBuilder : ModelContextBuilderBase<ITransitionModelContext>
    {
        /// <summary>
        /// The model builder for the metropolis transition model collection
        /// </summary>
        public IMetropolisTransitionModelBuilder MetropolisTransitionModelBuilder { get; set; }

        /// <summary>
        /// The model builder for the kinetic transition model collection
        /// </summary>
        public IKineticTransitionModelBuilder KineticTransitionModelBuilder { get; set; }

        /// <summary>
        /// The model builder for the position transition model collection
        /// </summary>
        public  IPositionTransitionModelBuilder PositionTransitionModelBuilder { get; set; }

        /// <inheritdoc />
        public TransitionModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder) : base(projectModelContextBuilder)
        {
            MetropolisTransitionModelBuilder = new MetropolisTransitionModelBuilder(ProjectServices);
            KineticTransitionModelBuilder = new KineticTransitionModelBuilder(ProjectServices);
            PositionTransitionModelBuilder = new PositionTransitionModelBuilder(ProjectServices);
        }

        /// <inheritdoc />
        protected override void PopulateContext()
        {
            var manager = ProjectServices.GetManager<ITransitionManager>();
            var metropolisTransitions = manager.QueryPort.Query(port => port.GetMetropolisTransitions());
            var kineticTransitions = manager.QueryPort.Query(port => port.GetKineticTransitions());

            var kineticTask = Task.Run(() => KineticTransitionModelBuilder.BuildModels(kineticTransitions));
            var metropolisTask = Task.Run(() => MetropolisTransitionModelBuilder.BuildModels(metropolisTransitions));
            var awaitTask = Task.WhenAll(kineticTask, metropolisTask);
            var positionTask = Task.Run(() => PositionTransitionModelBuilder.BuildModels(ModelContext, awaitTask));

            ModelContext.KineticTransitionModels = kineticTask.Result;
            ModelContext.MetropolisTransitionModels = metropolisTask.Result;
            ModelContext.PositionTransitionModels = positionTask.Result;
        }
    }
}
