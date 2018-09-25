using ICon.Model.ProjectServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class ProjectModelContext : IProjectModelContext
    {
        /// <inheritdoc />
        public IProjectServices ProjectServices { get; set; }

        /// <inheritdoc />
        public IStructureModelContext StructureModelContext { get; set; }

        /// <inheritdoc />
        public ITransitionModelContext TransitionModelContext { get; set; }

        /// <inheritdoc />
        public IEnergyModelContext EnergyModelContext { get; set; }

        /// <inheritdoc />
        public ISimulationModelContext SimulationModelContext { get; set; }
    }
}