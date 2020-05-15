using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.Events;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Model.DataManagement;
using Mocassin.Model.ModelProject;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.Xml.ProjectLibrary
{
    /// <summary>
    ///     The <see cref="DbContext" /> for the <see cref="IMocassinProjectLibrary" /> that stores user project data
    /// </summary>
    public sealed class MocassinProjectContext : SqLiteContext<MocassinProjectContext>, IMocassinProjectLibrary
    {
        private readonly object lockObject = new object();
        private bool isDisposed;

        /// <inheritdoc />
        public bool IsDisposed
        {
            get
            {
                lock (lockObject)
                {
                    return isDisposed;
                }
            }
        }

        /// <inheritdoc />
        public string SourceName => FileName;

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> that relays internal state change events
        /// </summary>
        private ReactiveEvent<Unit> StateChangedEvent { get; }

        /// <inheritdoc />
        public DbSet<MocassinProject> MocassinProjectGraphs { get; set; }

        /// <inheritdoc />
        public IObservable<Unit> ModelChangedNotification => StateChangedEvent.AsObservable();

        /// <inheritdoc />
        public MocassinProjectContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
            StateChangedEvent = new ReactiveEvent<Unit>();
            ChangeTracker.StateChanged += RelayEntityChangeEvent;
            ChangeTracker.Tracked += RelayEntityChangeEvent;
        }

        /// <inheritdoc />
        public int GetProjectModelHash()
        {
            var result = 517;
            foreach (var projectGraph in MocassinProjectGraphs.Local)
            {
                unchecked
                {
                    result += projectGraph.ProjectModelData.ToJson().GetHashCode();
                }
            }

            return result;
        }

        /// <inheritdoc />
        public bool CheckForModelChanges()
        {
            return !IsDisposed && HasUnsavedChanges();
        }

        /// <inheritdoc />
        public Task<bool> CheckForModelChangesAsync()
        {
            return Task.Run(CheckForModelChanges);
        }

        /// <inheritdoc />
        public IModelProject LoadDefaultModelProject(Expression<Func<MocassinProject, bool>> selectorExpression, ProjectSettings settings = null)
        {
            var project = MocassinProjectGraphs.Single(selectorExpression);
            var modelProject = ModelProjectFactory.Create(settings ?? ProjectSettings.CreateDefault());
            modelProject.InputPipeline.PushToProject(project.ProjectModelData.GetInputSequence());
            return modelProject;
        }

        /// <inheritdoc />
        void IMocassinProjectLibrary.Add<TEntity>(TEntity entity)
        {
            if (IsDisposed) return;
            Add(entity);
        }

        /// <inheritdoc />
        public bool HasUnsavedChanges()
        {
            lock (lockObject)
            {
                return ChangeTracker.Entries().Any(x => x.State == EntityState.Added ||
                                                        x.State == EntityState.Modified ||
                                                        x.State == EntityState.Deleted);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            lock (lockObject)
            {
                base.Dispose();
                isDisposed = true;
            }

            StateChangedEvent.OnCompleted();
        }

        /// <summary>
        ///     Relays the <see cref="DbContext" /> entity changed event to the internal <see cref="ReactiveEvent{TSubject}" />
        /// </summary>
        private void RelayEntityChangeEvent(object sender, EventArgs args)
        {
            StateChangedEvent.OnNext(Unit.Default);
        }

        /// <inheritdoc cref="DbContext" />
        public override int SaveChanges()
        {
            Clean();
            return base.SaveChanges();
        }
    }
}