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
        /// The list of blob redirection actions on the model
        /// </summary>
        protected static List<Action<ModelBuilder>> BlobRedirections { get; set; }

        /// <summary>
        /// The full filepath used for opening the database
        /// </summary>
        public string Filename { get; protected set; }

        public DbSet<TestEntity> Tests { get; set; }

        /// <summary>
        /// The database set for all blobs
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
                item.DataToBinary();
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
            if (BlobRedirections == null)
            {
                BlobRedirections = MakeBlobRedirectionList();
            }
            BlobRedirections.ForEach(a => a(modelBuilder));
        }

        /// <summary>
        /// Searches all data entities for blob inheriting properties and creates a list of model builder table redirections for their types
        /// </summary>
        /// <returns></returns>
        protected List<Action<ModelBuilder>> MakeBlobRedirectionList()
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
