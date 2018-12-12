using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Mocassin.Model.Translator
{
    /// <inheritdoc cref="Mocassin.Model.Translator.ISimulationDbContext" />
    public sealed class SimulationDbContext : DbContext, ISimulationDbContext
    {
        /// <summary>
        ///     List of actions performed on model building
        /// </summary>
        private static IList<Action<ModelBuilder>> ModelBuildActions { get; set; }

        /// <summary>
        ///     The database filename
        /// </summary>
        private string DbFilename { get; }

        /// <inheritdoc />
        public DbSet<SimulationJobPackageModel> SimulationPackages { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationStructureModel> StructureModels { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationTransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationEnergyModel> EnergyModels { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationJobModel> JobModels { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationLatticeModel> LatticeModels { get; set; }

        /// <inheritdoc />
        public DbSet<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        /// <inheritdoc />
        public DbSet<PairEnergyTableEntity> PairEnergyTables { get; set; }

        /// <inheritdoc />
        public DbSet<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }

        /// <inheritdoc />
        public DbSet<JumpCollectionEntity> JumpCollections { get; set; }

        /// <inheritdoc />
        public DbSet<JumpDirectionEntity> JumpDirections { get; set; }

        /// <inheritdoc />
        public DbSet<BlobEntityBase> Blobs { get; set; }

        /// <inheritdoc />
        public DbSet<SqliteQueryEntity> SqliteQueries { get; set; }

        /// <summary>
        ///     Create new database context using the provided filepath. Dop creates a new database if specified
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dropCreate"></param>
        public SimulationDbContext(string filename, bool dropCreate)
        {
            DbFilename = filename;
            if (dropCreate) Database.EnsureDeleted();

            Database.EnsureCreated();
        }

        /// <inheritdoc cref="ISimulationDbContext.SaveChanges" />
        public override int SaveChanges()
        {
            base.SaveChanges();

            using (var marshalProvider = new MarshalProvider())
            {
                foreach (var item in Blobs)
                    item.ChangeStateToBinary(marshalProvider);

                PerformActionOnAllInteropEntities(a => a.ChangePropertyStatesToBinaries(marshalProvider));

                return base.SaveChanges();
            }
        }


        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DbFilename}");
            base.OnConfiguring(optionsBuilder);
        }


        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            RedirectBinaryObjects(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        ///     Detects and redirects all blob entities on model creation to the blobs database set
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void RedirectBinaryObjects(ModelBuilder modelBuilder)
        {
            if (ModelBuildActions == null) 
                ModelBuildActions = CreateRedirectionDelegates();

            foreach (var item in ModelBuildActions)
                item.Invoke(modelBuilder);
        }


        /// <summary>
        ///     Searches all db set for inheritance from interop binary object and creates redirects to the binary object table
        /// </summary>
        /// <returns></returns>
        private List<Action<ModelBuilder>> CreateRedirectionDelegates()
        {
            var list = new List<Action<ModelBuilder>>();
            foreach (var dbSetProperty in GetDbSetPropertyInfos())
            {
                foreach (var property in dbSetProperty.PropertyType.GetGenericArguments()[0].GetProperties())
                {
                    if (typeof(BlobEntityBase).IsAssignableFrom(property.PropertyType))
                        list.Add(builder => builder.Entity(property.PropertyType).ToTable(nameof(Blobs)));
                }
            }

            return list;
        }

        /// <summary>
        ///     Performs the passed action on all properties of database set entries that are interop entities
        /// </summary>
        /// <param name="action"></param>
        private void PerformActionOnAllInteropEntities(Action<InteropEntityBase> action)
        {
            foreach (var dbSetProperty in GetDbSetPropertyInfos())
            {
                if (!typeof(InteropEntityBase).IsAssignableFrom(dbSetProperty.PropertyType.GetGenericArguments()[0]))
                    continue;

                foreach (var item in (IEnumerable<InteropEntityBase>) dbSetProperty.GetValue(this))
                    action(item);
            }
        }

        /// <summary>
        ///     Gets all database sets property info of the context by reflection
        /// </summary>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetDbSetPropertyInfos()
        {
            return GetType().GetProperties()
                .Where(a => a.PropertyType.IsGenericType)
                .Where(item => item.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));
        }
    }
}