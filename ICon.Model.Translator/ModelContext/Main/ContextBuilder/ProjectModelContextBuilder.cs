using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ICon.Model.ProjectServices;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class ProjectModelContextBuilder : IProjectModelContextBuilder
    {
        /// <inheritdoc />
        public IProjectServices ProjectServices { get; set; }

        /// <inheritdoc />
        public IProjectModelContext ProjectModelContext { get; set; }

        /// <inheritdoc />
        public Task<IEnergyModelContext> EnergyModelContext { get; set; }

        /// <inheritdoc />
        public Task<IStructureModelContext> StructureModelContext { get; set; }

        /// <inheritdoc />
        public Task<ITransitionModelContext> TransitionModelContext { get; set; }

        /// <inheritdoc />
        public Task<ISimulationModelContext> SimulationModelContext { get; set; }

        /// <summary>
        /// Build a new context builder that uses the provided project access as a reference data source
        /// </summary>
        /// <param name="projectServices"></param>
        public ProjectModelContextBuilder(IProjectServices projectServices)
        {
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
            ProjectModelContext = new ProjectModelContext() { ProjectServices = projectServices };
        }

        /// <summary>
        /// Async creation of a new project model context and all its components
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        public async Task<IProjectModelContext> CreateNewContextAsync<T1>() where T1 : IProjectModelContext
        {
            ProjectModelContext = new ProjectModelContext()
            {
                ProjectServices = ProjectServices
            };

            await BuildContextComponents();
            await LinkContextComponents();

            return ProjectModelContext;
        }

        /// <summary>
        /// Creates all context components independently and awaits their completion
        /// </summary>
        /// <returns></returns>
        protected Task BuildContextComponents()
        {
            EnergyModelContext = new EnergyModelContextBuilder(this).CreateNewContext<EnergyModelContext>();
            StructureModelContext = new StructureModelContextBuilder(this).CreateNewContext<StructureModelContext>();
            TransitionModelContext = new TransitionModelContextBuilder(this).CreateNewContext<TransitionModelContext>();
            SimulationModelContext = new SimulationModelContextBuilder(this).CreateNewContext<SimulationModelContext>();
            return Task.WhenAll(EnergyModelContext, StructureModelContext, TransitionModelContext, SimulationModelContext);
        }

        protected async Task LinkContextComponents()
        {

        }
    }
}
