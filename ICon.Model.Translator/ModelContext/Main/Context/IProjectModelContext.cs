using System;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Defines status flags for the <see cref="IProjectModelContext" />
    /// </summary>
    [Flags]
    public enum ProjectContextFlags
    {
        /// <summary>
        ///     Defines that the context is only partial build
        /// </summary>
        PartialContext = 1,

        /// <summary>
        ///     Defines that the context is complete
        /// </summary>
        FullContext = 1 << 1,

        /// <summary>
        ///     Defines that the context components are completely interlinked
        /// </summary>
        LinkedContext = 1 << 2,

        /// <summary>
        ///     Defines that the context can be used for job translations
        /// </summary>
        TranslatableContext = FullContext | LinkedContext
    }

    /// <summary>
    ///     Represents a full project data context that contains all data described within the model managers
    /// </summary>
    public interface IProjectModelContext
    {
        /// <summary>
        ///     Get the currently set <see cref="ProjectContextFlags" />
        /// </summary>
        ProjectContextFlags ContextFlags { get; }

        /// <summary>
        ///     Get a boolean value if the context is rdy for component interlinking
        /// </summary>
        bool IsLinkable { get; }

        /// <summary>
        ///     Get boolean value if the context can be used for job translations
        /// </summary>
        bool IsTranslatable { get; }

        /// <summary>
        ///     Access to the project services the model context was generated from
        /// </summary>
        IModelProject ModelProject { get; set; }

        /// <summary>
        ///     The structure model context of the linked project
        /// </summary>
        IStructureModelContext StructureModelContext { get; set; }

        /// <summary>
        ///     The transition model context of the linked project
        /// </summary>
        ITransitionModelContext TransitionModelContext { get; set; }

        /// <summary>
        ///     The energy model context of the linked project
        /// </summary>
        IEnergyModelContext EnergyModelContext { get; set; }

        /// <summary>
        ///     The simulation model context of the linked project
        /// </summary>
        ISimulationModelContext SimulationModelContext { get; set; }
    }
}