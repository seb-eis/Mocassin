using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.ProjectServices;
using ICon.Model.Simulations;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Represents a simulation translation context for handling of simulation translation data
    /// </summary>
    public interface ITranslationContext
    {
        /// <summary>
        /// The list of base simulations in order of translation
        /// </summary>
        IReadOnlyList<ISimulationBase> BaseSimulations { get; }

        /// <summary>
        /// Access to the MC project service that carries the NET model data structures
        /// </summary>
        IProjectServices ProjectServices { get; set; }

        /// <summary>
        /// Translator database context for storage of the translated interop information
        /// </summary>
        ITranslatorDbContext DbContext { get; set; }

        /// <summary>
        /// Translates the basic structure, transition and energy model of the current project service and adds them to the database
        /// </summary>
        void UpdateTopLevelDbModel();

        /// <summary>
        /// Creates and adds/updates the job model that results from the passed simulation description in/to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="simulation"></param>
        void AddSimulationToDatabase<T>(T simulation) where T : ISimulationBase;

        /// <summary>
        /// Creates and adds/updates the job models that result from the passed simualtion series in/to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="simulationSeries"></param>
        void AddSimulationSeriesToDatabase<T>(T simulationSeries) where T : ISimulationSeriesBase;

        /// <summary>
        /// Get the package context id of the passed simulation in the database context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="simulation"></param>
        /// <returns></returns>
        int FindSimulationPackageContextId<T>(T simulation) where T : ISimulationBase;
    }
}
