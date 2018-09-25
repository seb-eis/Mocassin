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

        /// <summary>
        /// Database set for translated simulation packages
        /// </summary>
        public DbSet<SimulationPackage> SimulationPackages { get; set; }

        /// <summary>
        /// Database set for translated structure models
        public DbSet<StructureModel> StructureModels { get; set; }

        /// <summary>
        /// Database set for translated transition models
        /// </summary>
        public DbSet<TransitionModel> TransitionModels { get; set; }

        /// <summary>
        /// Database set for translated energy models
        /// </summary>
        public DbSet<EnergyModel> EnergyModels { get; set; }

        /// <summary>
        /// Database set for translated job models
        /// </summary>
        public DbSet<JobModel> JobModels { get; set; }

        /// <summary>
        /// Database set for translated lattice models
        /// </summary>
        public DbSet<LatticeModel> LatticeModels { get; set; }

        /// <summary>
        /// Database set for all environment definitions
        /// </summary>
        public DbSet<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        /// <summary>
        /// Database set for all pair energy tables
        /// </summary>
        public DbSet<PairEnergyTableEntity> PairEnergyTables { get; set; }

        /// <summary>
        /// Database set for all cluster energy tables
        /// </summary>
        public DbSet<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }

        /// <summary>
        /// Database set for all jump collections
        /// </summary>
        public DbSet<JumpCollectionEntity> JumpCollections { get; set; }

        /// <summary>
        /// Database set for alljump directions
        /// </summary>
        public DbSet<JumpDirectionEntity> JumpDirections { get; set; }

        /// <summary>
        /// The database set for all entities that support storing as a blob
        /// </summary>
        public DbSet<BlobEntityBase> Blobs { get; set; }

        /// <summary>
        /// Database set for sqlite load queries. Describes how the simulator pulls data from the database
        /// </summary>
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

        /// <summary>
        /// Saves the changes to the database in two steps. First the actual changes and then converts all blob stored entities to actual blobs
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            base.SaveChanges();

            using (var marhshalProvider = new MarshalProvider())
            {
                foreach (var item in Blobs)
                {
                    item.ChangeStateToBinary(marhshalProvider);
                }

                PerformActionOnAllInteropEntities(a => a.ChangePropertyStatesToBinaries(marhshalProvider));

                return base.SaveChanges();
            }
        }

        /// <summary>
        /// Configures the database to use sqlite and the provided databse filepath
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DbFilename}");
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Specifies the model of the database to corretly store all blob properties
        /// </summary>
        /// <param name="modelBuilder"></param>
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
                if (typeof(InteropEntityBase).IsAssignableFrom(dbSetProperty.PropertyType.GetGenericArguments()[0]))
                {
                    foreach (var item in (IEnumerable<InteropEntityBase>) dbSetProperty.GetValue(this))
                    {
                        action(item);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all database sets property info of the context by reflection
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GetDbSetPropertyInfos()
        {
            foreach (var item in GetType().GetProperties().Where(a => a.PropertyType.IsGenericType))
            {
                if (item.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    yield return item;
                }
            }
        }
    }
}
