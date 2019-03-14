using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.SimulationModel;
using Mocassin.UI.Xml.StructureModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.Xml.ProjectLibrary
{
    /// <summary>
    ///     Represents a project library that stores all user defined data belonging to a
    ///     <see cref="Mocassin.Model.ModelProject.IModelProject" />
    /// </summary>
    public interface IMocassinProjectLibrary : IDisposable
    {
        /// <summary>
        ///     Get the <see cref="DbSet{TEntity}"/> of <see cref="MocassinProjectGraph"/> objects stored in the library
        /// </summary>
        DbSet<MocassinProjectGraph> MocassinProjectGraphs { get; }

        /// <summary>
        ///     Get the <see cref="DbSet{TEntity}"/> of <see cref="ProjectModelGraph"/> objects stored in the library
        /// </summary>
        DbSet<ProjectModelGraph> ProjectModelGraphs { get; }

        /// <summary>
        ///     Get the <see cref="DbSet{TEntity}"/> of <see cref="ProjectCustomizationGraph"/> objects stored in the library
        /// </summary>
        DbSet<ProjectCustomizationGraph> ProjectCustomizationGraphs { get; }

        /// <summary>
        ///     Get the <see cref="DbSet{TEntity}"/> of <see cref="ProjectJobTranslationGraph"/> objects stored in the library
        /// </summary>
        DbSet<ProjectJobTranslationGraph> ProjectJobTranslationGraphs { get; }

        /// <summary>
        ///     Get the <see cref="DbSet{TEntity}"/> of <see cref="MocassinProjectBuildGraph"/> objects stored in the library
        /// </summary>
        DbSet<MocassinProjectBuildGraph> MocassinProjectBuildGraphs { get; }

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
        ///     Get a <see cref="IObservable{T}"/> that informs that an internal change in the library happened
        /// </summary>
        IObservable<Unit> StateChangedNotification { get; }
    }
}