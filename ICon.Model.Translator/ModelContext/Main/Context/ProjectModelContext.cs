using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class ProjectModelContext : IProjectModelContext
    {
        /// <inheritdoc />
        public ProjectContextFlags ContextFlags { get; set; }

        /// <inheritdoc />
        public bool IsLinkable => ContextFlags.HasFlag(ProjectContextFlags.FullContext);

        /// <inheritdoc />
        public bool IsTranslatable => ContextFlags.HasFlag(ProjectContextFlags.TranslatableContext);

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