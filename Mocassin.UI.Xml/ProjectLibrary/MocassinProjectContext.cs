using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.Events;
using Mocassin.Framework.SQLiteCore;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.ProjectLibrary
{
    /// <summary>
    ///     The <see cref="DbContext" /> for the <see cref="IMocassinProjectLibrary" /> that stores user project data
    /// </summary>
    public sealed class MocassinProjectContext : SqLiteContext<MocassinProjectContext>, IMocassinProjectLibrary
    {
        private readonly object lockObject = new object();
        private bool isDisposed;
        private int lastHash;

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

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> that relays internal state change events
        /// </summary>
        private ReactiveEvent<Unit> StateChangedEvent { get; }

        /// <inheritdoc />
        public DbSet<MocassinProjectGraph> MocassinProjectGraphs { get; set; }

        /// <inheritdoc />
        public DbSet<ProjectModelGraph> ProjectModelGraphs { get; set; }

        /// <inheritdoc />
        public DbSet<ProjectCustomizationGraph> ProjectCustomizationGraphs { get; set; }

        /// <inheritdoc />
        public DbSet<ProjectJobTranslationGraph> ProjectJobTranslationGraphs { get; set; }

        /// <inheritdoc />
        public DbSet<MocassinProjectBuildGraph> MocassinProjectBuildGraphs { get; set; }

        /// <inheritdoc />
        public IObservable<Unit> StateChangedNotification => StateChangedEvent.AsObservable();

        /// <inheritdoc />
        public MocassinProjectContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
            StateChangedEvent = new ReactiveEvent<Unit>();
            ChangeTracker.StateChanged += RelayEntityChangeEvent;
            ChangeTracker.Tracked += RelayEntityChangeEvent;
        }

        /// <inheritdoc />
        public int GetProjectHash()
        {
            var result = 517;
            foreach (var projectGraph in MocassinProjectGraphs.Local)
            {
                unchecked
                {
                    result += projectGraph.ToJson().GetHashCode();
                }
            }

            return result;
        }

        /// <inheritdoc />
        public bool CheckForContentChange()
        {
            if (IsDisposed || !HasUnsavedChanges()) return false;
            lock (lockObject)
            {
                var hash = GetProjectHash();
                if (hash != lastHash) StateChangedEvent.OnNext(Unit.Default);
                lastHash = hash;
                return lastHash != hash;
            }
        }

        /// <inheritdoc />
        public Task<bool> CheckForContentChangeAsync()
        {
            return Task.Run(CheckForContentChange);
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

        /// <summary>
        ///     Relays the <see cref="DbContext" /> entity changed event to the internal <see cref="ReactiveEvent{TSubject}" />
        /// </summary>
        private void RelayEntityChangeEvent(object sender, EventArgs args)
        {
            StateChangedEvent.OnNext(Unit.Default);
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
    }
}