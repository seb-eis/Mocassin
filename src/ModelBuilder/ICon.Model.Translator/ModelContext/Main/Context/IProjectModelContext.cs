using System;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a full project data context that contains all data described within the model managers
    /// </summary>
    public interface IProjectModelContext
    {
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