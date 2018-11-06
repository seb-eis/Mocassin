using Mocassin.Framework.Operations;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Represents a data conflict resolver that handles internal data conflicts within a manager that are induces by
    ///     direct input to the manager
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface IDataConflictHandler<in T1, in T2> where T1 : ModelData
    {
        /// <summary>
        ///     Takes the source of a potential conflict as an object and a data accessor instance to the data to resolve potential
        ///     conflicts
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        IConflictReport ResolveConflicts(T2 source, IDataAccessor<T1> dataAccess);
    }
}