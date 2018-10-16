﻿using System;
using ICon.Framework.Operations;

namespace ICon.Model.Basic
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
            Console.WriteLine($"Dummy conflict resolver {GetType()} called for {source.GetType()}");
            return new ConflictReport();
        }
    }
}