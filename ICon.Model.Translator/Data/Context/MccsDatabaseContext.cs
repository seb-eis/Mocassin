using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Mccs database context for encoded simulation data and result entities
    /// </summary>
    public class MccsDatabaseContext : DbContext
    {
        /// <summary>
        /// The list of context creation model builder actions
        /// </summary>
        protected static List<Action<ModelBuilder>> ModelBuilderActions { get; set; }

        /// <summary>
        /// The full filepath used for opening the database
        /// </summary>
        public string Filename { get; protected set; }

        /// <summary>
        /// The database set for the mcs package entities
        /// </summary>
        public DbSet<McsPackage> McsPackages { get; set; }

        /// <summary>
        /// The database set for the mcs parent entities
        /// </summary>
        public DbSet<McsParent> McsParents { get; set; }

        /// <summary>
        /// The database set for the mcs job entities
        /// </summary>
        public DbSet<McsJob> McsJobs { get; set; }

        /// <summary>
        /// The database set for the mcs job entities
        /// </summary>
        public DbSet<McsJobResult> McsJobResults { get; set; }

        /// <summary>
        /// The database set for the mcs structure component entities
        /// </summary>
        public DbSet<McsStructure> McsStructures { get; set; }

        /// <summary>
        /// The database set for the mcs transition component entities
        /// </summary>
        public DbSet<McsTransitions> McsTransitions { get; set; }

        /// <summary>
        /// The database set for the mcs energy component entities
        /// </summary>
        public DbSet<McsEnergies> McsEnergies { get; set; }

        /// <summary>
        /// The database set for the position environment entities
        /// </summary>
        public DbSet<Environment> Environments { get; set; }

        /// <summary>
        /// The database set for the jump collection entities
        /// </summary>
        public DbSet<JumpCollection> JumpCollections { get; set; }

        /// <summary>
        /// The database set for the jump direction entities
        /// </summary>
        public DbSet<JumpDirection> JumpDirections { get; set; }

        /// <summary>
        /// The database set for the pair energy table entities
        /// </summary>
        public DbSet<PairEnergyTable> PairEnergyTables { get; set; }

        /// <summary>
        /// The database set for the cluster energy table entities
        /// </summary>
        public DbSet<ClusterEnergyTable> ClusterEnergyTables { get; set; }

        /// <summary>
        /// The database set for all entities that support storing as a blob
        /// </summary>
        public DbSet<BlobEntity> Blobs { get; set; }

        /// <summary>
        /// Create new mccs database context using the provided filepath. Dop creates a new database if specfified
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dropCreate"></param>
        public MccsDatabaseContext(string filename, bool dropCreate)
        {
            Filename = filename;
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
            foreach (var item in Blobs)
            {
                item.ChangeToBlobState();
            }
            return base.SaveChanges();
        }

        /// <summary>
        /// Configures the database to use sqlite and the provided databse filepath
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={Filename}");
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Specifies the model of the database to corretly store all blob properties
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            RedirectBlobEntities(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Detects and redirects all blob entities on model creation to the blobs database set
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected void RedirectBlobEntities(ModelBuilder modelBuilder)
        {
            if (ModelBuilderActions == null)
            {
                ModelBuilderActions = CreateRedirectionDelegates();
            }
            ModelBuilderActions.ForEach(a => a(modelBuilder));
        }

        /// <summary>
        /// Searches all data entities for blob inheriting properties and creates a list of model builder table redirections for their types
        /// </summary>
        /// <returns></returns>
        protected List<Action<ModelBuilder>> CreateRedirectionDelegates()
        {
            var list = new List<Action<ModelBuilder>>();
            foreach (var item in GetType().GetProperties().Where(a => a.PropertyType.IsGenericType))
            {
                if (item.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    foreach (var property in item.PropertyType.GetGenericArguments()[0].GetProperties())
                    {
                        if (typeof(BlobEntity).IsAssignableFrom(property.PropertyType))
                        {
                            list.Add(builder => builder.Entity(property.PropertyType).ToTable(nameof(Blobs)));
                        }
                    }
                }
            }
            return list;
        }
    }
}
