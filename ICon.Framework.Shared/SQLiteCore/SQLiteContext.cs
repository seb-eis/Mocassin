using System;
using Microsoft.EntityFrameworkCore;

namespace ICon.Framework.SQLiteCore
{
    /// <summary>
    /// An abstract SQLite EFCore context class that supports the ICon context provider system and ensures that the database is created
    /// </summary>
    public abstract class SQLiteContext : DbContext
    {
        /// <summary>
        /// The total filestring parameter passed to the options builder to find the database
        /// </summary>
        public String OptionsBuilderParameterString { get; internal set; }

        /// <summary>
        /// Creates a new context with the provided options builder string parameter and ensures that the database is created
        /// </summary>
        /// <param name="optionsBuilderParameterString"></param>
        protected SQLiteContext(String optionsBuilderParameterString)
        {
            OptionsBuilderParameterString = optionsBuilderParameterString;
            Database.EnsureCreated();
        }

        /// <summary>
        /// Creates new empty context
        /// </summary>
        protected SQLiteContext()
        {
        }

        /// <summary>
        /// Ensures that the database uses the defined filepath and the SQLite API
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(OptionsBuilderParameterString);
        }
    }

    /// <summary>
    /// An abstract generic SQLite EFCore context class that supports the ICon context provider system and ensures that the database is created
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class SQLiteContext<T1> : SQLiteContext where T1 : SQLiteContext, new()
    {
        /// <summary>
        /// Creates new empty context
        /// </summary>
        protected SQLiteContext() : base()
        {
        }

        /// <summary>
        /// Creates a new context with the provided options builder string parameter and ensures that the database is created
        /// </summary>
        /// <param name="optionsBuilderParameterString"></param>
        protected SQLiteContext(String optionsBuilderParameterString) : base(optionsBuilderParameterString)
        {
        }

        /// <summary>
        /// Creates a new context of this type with the provided options builder parameter string
        /// </summary>
        /// <param name="optionsBuilderParameterString"></param>
        /// <returns></returns>
        public abstract T1 CreateNewContext(String optionsBuilderParameterString);
    }
}
