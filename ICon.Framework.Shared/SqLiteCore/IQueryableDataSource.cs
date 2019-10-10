using System.Linq;

namespace Mocassin.Framework.SQLiteCore
{
    /// <summary>
    ///     Provides a general interface for data sources that provide <see cref="IQueryable{T}" />
    /// </summary>
    public interface IQueryableDataSource
    {
        /// <summary>
        ///     Gets an <see cref="IQueryable{T}" /> of the
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IQueryable<TEntity> Set<TEntity>() where TEntity : class;
    }
}