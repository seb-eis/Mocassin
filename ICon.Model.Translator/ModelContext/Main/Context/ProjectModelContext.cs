using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class ProjectModelContext : IProjectModelContext
    {
        /// <inheritdoc />
        public IModelProject ModelProject { get; set; }

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