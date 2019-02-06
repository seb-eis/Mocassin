using System;
using Mocassin.Framework.Operations;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Represents a dummy conflict resolver that does nothing except returning an empty resolver report
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class DataConflictHandlerDummy<T1, T2> : IDataConflictHandler<T1, T2>
        where T1 : ModelData
    {
        /// <inheritdoc />
        public IConflictReport ResolveConflicts(T2 source, IDataAccessor<T1> dataAccess)
        {
            return new ConflictReport();
        }
    }
}