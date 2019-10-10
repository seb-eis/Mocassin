using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Mocassin.Framework.SQLiteCore
{
    /// <summary>
    ///     Adapter class to provide <see cref="DbContext"/> instances for read only query access
    /// </summary>
    public sealed class ReadOnlyDbContext : IDisposable, IQueryableDataSource
    {
        /// <summary>
        ///     The wrapped <see cref="DbContext"/>
        /// </summary>
        private readonly DbContext _dbContext;

        /// <summary>
        ///     Creates new <see cref="ReadOnlyDbContext"/> for the passed <see cref="DbContext"/>
        /// </summary>
        /// <param name="dbContext"></param>
        public ReadOnlyDbContext(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        ///     Get a <see cref="IQueryable{T}"/> for the requested entity type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IQueryable<TEntity> Set<TEntity>() where TEntity : class
        {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}