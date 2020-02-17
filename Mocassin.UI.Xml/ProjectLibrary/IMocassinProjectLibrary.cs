using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.Xml.ProjectLibrary
{
    /// <summary>
    ///     Represents a project library that stores all user defined data belonging to a
    ///     <see cref="Mocassin.Model.ModelProject.IModelProject" />
    /// </summary>
    public interface IMocassinProjectLibrary : IDisposable
    {
        /// <summary>
        ///     Get a boolean flag if the context is disposed
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        ///     Get a <see cref="string" /> that describes the data source (e.g. file location)
        /// </summary>
        string SourceName { get; }

        /// <summary>
        ///     Get the <see cref="DbSet{TEntity}" /> of <see cref="MocassinProject" /> objects stored in the library
        /// </summary>
        DbSet<MocassinProject> MocassinProjectGraphs { get; }

        /// <summary>
        ///     Get a <see cref="IObservable{T}" /> that informs that a model change happened in the library
        /// </summary>
        IObservable<Unit> ModelChangedNotification { get; }

        /// <summary>
        ///     Adds the given <see cref="TEntity" /> objects to the <see cref="IMocassinProjectLibrary" />
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        void Add<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        ///     Adds the given objects to the <see cref="IMocassinProjectLibrary" />
        /// </summary>
        /// <param name="entities"></param>
        void AddRange(IEnumerable<object> entities);

        /// <summary>
        ///     Saves the current changes made to the <see cref="IMocassinProjectLibrary" />
        /// </summary>
        int SaveChanges();

        /// <summary>
        ///     Saves the current changes made to the <see cref="IMocassinProjectLibrary" /> asynchronously
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Checks if the project library has unsaved changes
        /// </summary>
        /// <returns></returns>
        bool HasUnsavedChanges();

        /// <summary>
        ///     Get a hash value for the entire project content
        /// </summary>
        /// <returns></returns>
        int GetProjectModelHash();

        /// <summary>
        ///     Checks if the contents of the library have changed since the last check and triggers affiliated events
        /// </summary>
        bool CheckForModelChanges();

        /// <summary>
        ///     Async checks if the contents of the library have changed since the last check and triggers affiliated events if
        ///     required
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckForModelChangesAsync();
    }
}