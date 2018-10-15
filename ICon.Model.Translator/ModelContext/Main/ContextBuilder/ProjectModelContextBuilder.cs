﻿using System;
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
        public Task<IProjectModelContext> ProjectModelContextBuildTask { get; set; }

        /// <inheritdoc />
        public IProjectModelContextLinker ProjectModelContextLinker { get; set; }

        /// <inheritdoc />
        public IEnergyModelContextBuilder EnergyModelContextBuilder { get; set; }

        /// <inheritdoc />
        public IStructureModelContextBuilder StructureModelContextBuilder { get; set; }

        /// <inheritdoc />
        public ITransitionModelContextBuilder TransitionModelContextBuilder { get; set; }

        /// <inheritdoc />
        public ISimulationModelContextBuilder SimulationModelContextBuilder { get; set; }


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
        /// <returns></returns>
        public async Task<IProjectModelContext> BuildNewContext()
        {
            var projectModelContext = new ProjectModelContext()
            {
                ProjectServices = ProjectServices
            };

            StartComponentBuildProcess();

            projectModelContext.EnergyModelContext = await EnergyModelContextBuilder.BuildTask;
            projectModelContext.SimulationModelContext = await SimulationModelContextBuilder.BuildTask;
            projectModelContext.TransitionModelContext = await TransitionModelContextBuilder.BuildTask;
            projectModelContext.StructureModelContext = await StructureModelContextBuilder.BuildTask;

            await ProjectModelContextLinker.LinkContextComponents(this);
            await BuildLinkDependentComponents();

            return projectModelContext;
        }

        /// <summary>
        /// Creates all context components independently and awaits their completion
        /// </summary>
        /// <returns></returns>
        protected void StartComponentBuildProcess()
        {
            UseDefaultsForNullComponents();

            EnergyModelContextBuilder.BuildContext();
            StructureModelContextBuilder.BuildContext();
            TransitionModelContextBuilder.BuildContext();
            SimulationModelContextBuilder.BuildContext();
        }

        /// <summary>
        /// Calls the link dependent build routines on all context builders
        /// </summary>
        protected Task BuildLinkDependentComponents()
        {
            var tasks = new[]
            {
                EnergyModelContextBuilder.BuildLinkDependentComponents(),
                StructureModelContextBuilder.BuildLinkDependentComponents(),
                TransitionModelContextBuilder.BuildLinkDependentComponents(),
                SimulationModelContextBuilder.BuildLinkDependentComponents()
            };
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Set all builder components that are null to a default build system
        /// </summary>
        protected void UseDefaultsForNullComponents()
        {
            StructureModelContextBuilder = StructureModelContextBuilder ?? new StructureModelContextBuilder(this);
            EnergyModelContextBuilder = EnergyModelContextBuilder ?? new EnergyModelContextBuilder(this);
            TransitionModelContextBuilder = TransitionModelContextBuilder ?? new TransitionModelContextBuilder(this);
            SimulationModelContextBuilder = SimulationModelContextBuilder ?? new SimulationModelContextBuilder(this);
            ProjectModelContextLinker = ProjectModelContextLinker ?? new ProjectModelContextLinker();
        }
    }
}
