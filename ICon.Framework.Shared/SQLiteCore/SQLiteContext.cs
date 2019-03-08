using System;
using Microsoft.EntityFrameworkCore;

namespace Mocassin.Framework.SQLiteCore
{
    /// <summary>
    ///     An abstract SQLite EFCore context class that supports the ICon context provider system and ensures that the
    ///     database is created
    /// </summary>
    public abstract class SqLiteContext : DbContext
    {
        /// <summary>
        ///     The total file string parameter passed to the options builder to find the database
        /// </summary>
        public string OptionsBuilderParameterString { get; internal set; }

        /// <summary>
        ///     Creates a new context with the provided options builder string parameter and ensures that the database is created
        /// </summary>
        /// <param name="optionsBuilderParameterString"></param>
        protected SqLiteContext(string optionsBuilderParameterString)
        {
            OptionsBuilderParameterString = optionsBuilderParameterString;
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(OptionsBuilderParameterString);
        }

        /// <summary>
        ///     Creates a new <see cref="DbContext" /> of type <see cref="T1" /> using the provided file path and ensures that the
        ///     database is created
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dropCreate"></param>
        /// <returns></returns>
        public static T1 OpenDatabase<T1>(string filePath, bool dropCreate = false) where T1 : SqLiteContext
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            var context = (T1) Activator.CreateInstance(typeof(T1), $"Filename={filePath}");
            if (dropCreate) context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }
    }

    /// <summary>
    ///     An abstract generic SQLite EFCore context class that supports the ICon context provider system and ensures that the
    ///     database is created
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class SqLiteContext<T1> : SqLiteContext where T1 : SqLiteContext
    {
        /// <inheritdoc />
        protected SqLiteContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
        }
    }
}