using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ICon.Model.ProjectServices;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a project model context builder that coordinates the building process of the model context components
    /// </summary>
    public interface IProjectModelContextBuilder
    {
        /// <summary>
        /// Data access to the project reference data the builder uses
        /// </summary>
        IProjectServices ProjectServices { get; set; }

        /// <summary>
        /// The project model context build task that is currently running
        /// </summary>
        Task<IProjectModelContext> ProjectModelContextBuildTask { get; set; }

        /// <summary>
        /// The project context linker that creates component interconnections
        /// </summary>
        IProjectModelContextLinker ProjectModelContextLinker { get; set; }

        /// <summary>
        /// Get or set the energy model context builder
        /// </summary>
        IEnergyModelContextBuilder EnergyModelContextBuilder { get; set; }

        /// <summary>
        /// Get or set the structure model context builder
        /// </summary>
        IStructureModelContextBuilder StructureModelContextBuilder { get; set; }

        /// <summary>
        /// Get or set the transition model context builder
        /// </summary>
        ITransitionModelContextBuilder TransitionModelContextBuilder { get; set; }

        /// <summary>
        /// Get or set the simulation model context builder
        /// </summary>
        ISimulationModelContextBuilder SimulationModelContextBuilder { get; set; }
    }
}
