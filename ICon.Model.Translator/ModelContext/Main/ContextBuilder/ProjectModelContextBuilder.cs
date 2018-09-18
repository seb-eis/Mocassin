using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ICon.Model.ProjectServices;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Context builder for the main project model context
    /// </summary>
    public class ProjectModelContextBuilder : IProjectModelContextBuilder
    {
        /// <summary>
        /// Data access to the project refernce data the builder uses
        /// </summary>
        public IProjectServices ProjectServices { get; set; }

        /// <summary>
        /// The project model context that is being build by this context builder
        /// </summary>
        public IProjectModelContext ProjectModelContext { get; set; }

        /// <summary>
        /// The build task of the energy model context component
        /// </summary>
        public Task<IEnergyModelContext> EnergyModelContext { get; set; }

        /// <summary>
        /// The build task of the structure model context component
        /// </summary>
        public Task<IStructureModelContext> StructureModelContext { get; set; }

        /// <summary>
        /// The build task of the transition model context component
        /// </summary>
        public Task<ITransitionModelContext> TransitionModelContext { get; set; }

        /// <summary>
        /// The build task of the simulation model context component
        /// </summary>
        public Task<ISimulationModelContext> SimulationModelContext { get; set; }

        /// <summary>
        /// Build a new context builder that uses the provided project access as a reference data source
        /// </summary>
        /// <param name="projectServices"></param>
        public ProjectModelContextBuilder(IProjectServices projectServices)
        {
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
        }

        /// <summary>
        /// Async creation of a new project model context and all its components
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        public async Task<IProjectModelContext> CreatNewContextAsync<T1>() where T1 : IProjectModelContext
        {
            ProjectModelContext = new ProjectModelContext()
            {
                ProjectServices = ProjectServices
            };

            await BuildContextComponents();
            await LinkContextComponents();

            return ProjectModelContext;
        }

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
