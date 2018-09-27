using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Interop database context for communication with the unmanaged 'C' Simulator implementation 
    /// </summary>
    public class CInteropDbContext : DbContext, ITranslatorDbContext
    {
        /// <summary>
        /// List of actions performed on model building
        /// </summary>
        private static IList<Action<ModelBuilder>> ModelBuildActions { get; set; }

        /// <summary>
        /// The database filename 
        /// </summary>
        private string DbFilename { get; }

        /// <inheritdoc />
        public DbSet<SimulationPackage> SimulationPackages { get; set; }

        /// <inheritdoc />
        public DbSet<StructureModel> StructureModels { get; set; }

        /// <inheritdoc />
        public DbSet<TransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public DbSet<EnergyModel> EnergyModels { get; set; }

        /// <inheritdoc />
        public DbSet<JobModel> JobModels { get; set; }

        /// <inheritdoc />
        public DbSet<LatticeModel> LatticeModels { get; set; }

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
        /// Create new mccs database context using the provided filepath. Dop creates a new database if specfified
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dropCreate"></param>
        public CInteropDbContext(string filename, bool dropCreate)
        {
            DbFilename = filename;
            if (dropCreate)
            {
                Database.EnsureDeleted();
            }
            Database.EnsureCreated();
        }

        /// <inheritdoc cref="ITranslatorDbContext.SaveChanges"/>
        public override int SaveChanges()
        {
            base.SaveChanges();

            using (var marshalProvider = new MarshalProvider())
            {
                foreach (var item in Blobs)
                {
                    item.ChangeStateToBinary(marshalProvider);
                }

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
        /// Detects and redirects all blob entities on model creation to the blobs database set
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected void RedirectBinaryObjects(ModelBuilder modelBuilder)
        {
            if (ModelBuildActions == null)
            {
                ModelBuildActions = CreateRedirectionDelegates();
            }

            foreach (var item in ModelBuildActions)
            {
                item.Invoke(modelBuilder);
            }
        }


        /// <summary>
        /// Searches all db set for inheritance from interop binary object and creates redirects to the binary object table
        /// </summary>
        /// <returns></returns>
        protected List<Action<ModelBuilder>> CreateRedirectionDelegates()
        {
            var list = new List<Action<ModelBuilder>>();
            foreach (var dbSetProperty in GetDbSetPropertyInfos())
            {
                foreach (var property in dbSetProperty.PropertyType.GetGenericArguments()[0].GetProperties())
                {
                    if (typeof(BlobEntityBase).IsAssignableFrom(property.PropertyType))
                    {
                        list.Add(builder => builder.Entity(property.PropertyType).ToTable(nameof(Blobs)));
                    }
                }
            }
            return list;
        }

        protected void PerformActionOnAllInteropEntities(Action<InteropEntityBase> action)
        {
            foreach (var dbSetProperty in GetDbSetPropertyInfos())
            {
                if (!typeof(InteropEntityBase).IsAssignableFrom(dbSetProperty.PropertyType.GetGenericArguments()[0]))
                    continue;

                foreach (var item in (IEnumerable<InteropEntityBase>) dbSetProperty.GetValue(this))
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Gets all database sets property info of the context by reflection
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GetDbSetPropertyInfos()
        {
            return GetType().GetProperties()
                .Where(a => a.PropertyType.IsGenericType)
                .Where(item =>item.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));
        }
    }
}
