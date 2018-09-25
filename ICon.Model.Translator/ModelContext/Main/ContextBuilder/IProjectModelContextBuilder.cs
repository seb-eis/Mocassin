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
        /// The project model context that is build by the builder
        /// </summary>
        IProjectModelContext ProjectModelContext { get; set; }

        /// <summary>
        /// The build task of the energy model context component
        /// </summary>
        Task<IEnergyModelContext> EnergyModelContext { get; set; }

        /// <summary>
        /// The build task of the structure model context component
        /// </summary>
        Task<IStructureModelContext> StructureModelContext { get; set; }

        /// <summary>
        /// The build task of the transition model context component
        /// </summary>
        Task<ITransitionModelContext> TransitionModelContext { get; set; }

        /// <summary>
        /// The build task of the simulation model context component
        /// </summary>
        Task<ISimulationModelContext> SimulationModelContext { get; set; }
    }
}
