using ICon.Model.ProjectServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Implementation fo the full project model data context that carries all data for simulation generation/evaluation
    /// </summary>
    public class ProjectModelContext : IProjectModelContext
    {
        /// <summary>
        /// Access to the project services the model context was generated from
        /// </summary>
        public IProjectServices ProjectServices { get; set; }

        /// <summary>
        /// The structure model context of the linked project
        /// </summary>
        public IStructureModelContext StructureModelContext { get; set; }

        /// <summary>
        /// The transition model context of the linked project
        /// </summary>
        public ITransitionModelContext TransitionModelContext { get; set; }

        /// <summary>
        /// The energy model context of the linked project
        /// </summary>
        public IEnergyModelContext EnergyModelContext { get; set; }

        /// <summary>
        /// The simulation model context of the linked project
        /// </summary>
        public ISimulationModelContext SimulationModelContext { get; set; }
    }
}
