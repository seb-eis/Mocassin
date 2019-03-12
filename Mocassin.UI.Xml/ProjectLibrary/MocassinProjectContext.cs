using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;
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
    ///     The <see cref="DbContext" /> for the <see cref="IMocassinProjectLibrary"/> that stores user project data
    /// </summary>
    public sealed class MocassinProjectContext : SqLiteContext<MocassinProjectContext>, IMocassinProjectLibrary
    {
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
        void IMocassinProjectLibrary.Add<TEntity>(TEntity entity)
        {
            Add(entity);
        }

        /// <inheritdoc />
        public bool HasUnsavedChanges()
        {
            return ChangeTracker.Entries().Any(x => x.State == EntityState.Added ||
                                                    x.State == EntityState.Modified ||
                                                    x.State == EntityState.Deleted);
        }

        /// <inheritdoc />
        public MocassinProjectContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {

        }
    }
}