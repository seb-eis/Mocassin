using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

using ICon.Framework.SQLiteCore;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// The space group SQLite EFCore database context
    /// </summary>
    public sealed class SpaceGroupContext : SqLiteContext<SpaceGroupContext>
    {
        /// <summary>
        /// Creates
        /// </summary>
        public SpaceGroupContext()
        {
        }

        /// <summary>
        /// Creates a new space group context with the provided options builder parameter string
        /// </summary>
        /// <param name="optionsBuilderParameterString"></param>
        public SpaceGroupContext(String optionsBuilderParameterString) : base(optionsBuilderParameterString)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Space group database sets
        /// </summary>
        public DbSet<SpaceGroupEntity> SpaceGroups { get; set; }

        /// <summary>
        /// Symmetry operation database sets
        /// </summary>
        public DbSet<SymmetryOperationEntity> SymmetryOperations { get; set; }

        /// <summary>
        /// Creates a new space group context with the provided options builder parameter string
        /// </summary>
        /// <param name="optionsBuilderParameterString"></param>
        /// <returns></returns>
        public override SpaceGroupContext CreateNewContext(String optionsBuilderParameterString)
        {
            return new SpaceGroupContext(optionsBuilderParameterString);
        }
    }
}
