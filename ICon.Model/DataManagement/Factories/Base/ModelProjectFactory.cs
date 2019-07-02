using System;
using System.Collections.Generic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Static factory class that provides methods to build new <see cref="IModelProject" />
    /// </summary>
    public static class ModelProjectFactory
    {
        /// <summary>
        ///     Get a <see cref="IReadOnlyCollection{T}" /> of all default <see cref="IModelManagerFactory" /> instances
        /// </summary>
        public static IReadOnlyCollection<IModelManagerFactory> DefaultManagerFactories { get; }

        /// <summary>
        ///     Static constructor that searches the assembly for all manager factories
        /// </summary>
        static ModelProjectFactory()
        {
            DefaultManagerFactories = new List<IModelManagerFactory>
            {
                new ParticleManagerFactory(),
                new StructureManagerFactory(),
                new EnergyManagerFactory(),
                new TransitionManagerFactory(),
                new SimulationManagerFactory(),
                new LatticeManagerFactory()
            }.AsReadOnly();
        }

        /// <summary>
        ///     Creates an <see cref="IModelProject" /> interface that provides the default set of components and custom settings
        /// </summary>
        /// <returns></returns>
        public static IModelProject Create(ProjectSettings projectSettings)
        {
            if (projectSettings == null) throw new ArgumentNullException(nameof(projectSettings));
            var modelProject = ModelProject.ModelProject.Create(projectSettings);
            modelProject.CreateAndRegisterMany(DefaultManagerFactories);
            return modelProject;
        }

        /// <summary>
        ///     Creates an <see cref="IModelProject" /> interface that provides the default set of components and default settings
        /// </summary>
        /// <returns></returns>
        public static IModelProject CreateDefault()
        {
            return Create(ProjectSettings.CreateDefault());
        }
    }
}