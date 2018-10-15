using System;
using System.Threading.Tasks;
using ICon.Framework.Async;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Generic awaitable model context builder for asynchronous creation of extended model data context objects
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IModelContextBuilder<TContext> where TContext : class
    {
        /// <summary>
        /// Get the currently active build task
        /// </summary>
        Task<TContext> BuildTask { get; }

        /// <summary>
        /// Builds a default context from the currently linked project reference data
        /// </summary>
        Task<TContext> BuildContext();

        /// <summary>
        /// Builds all link dependent components on the context
        /// </summary>
        /// <returns></returns>
        Task BuildLinkDependentComponents();

        /// <summary>
        /// Rebuilds on the passed model context instead of creating a new one
        /// </summary>
        /// <param name="modelContext"></param>
        Task<TContext> RebuildContext(TContext modelContext);
    }
}