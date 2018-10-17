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
        protected SqLiteContext()
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(OptionsBuilderParameterString);
        }
    }

    /// <summary>
    ///     An abstract generic SQLite EFCore context class that supports the ICon context provider system and ensures that the
    ///     database is created
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class SqLiteContext<T1> : SqLiteContext where T1 : SqLiteContext, new()
    {
        /// <inheritdoc />
        protected SqLiteContext()
        {
        }

        /// <inheritdoc />
        protected SqLiteContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
        }

        /// <summary>
        ///     Creates a new context of this type with the provided options builder parameter string
        /// </summary>
        /// <param name="optionsBuilderParameterString"></param>
        /// <returns></returns>
        public abstract T1 CreateNewContext(string optionsBuilderParameterString);
    }
}