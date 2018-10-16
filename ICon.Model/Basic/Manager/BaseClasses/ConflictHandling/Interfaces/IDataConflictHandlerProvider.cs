namespace ICon.Model.Basic
{
    /// <summary>
    ///     Represents a conflict resolver provider that offers resolvers for manager intern data updating induced by changes
    /// </summary>
    public interface IDataConflictHandlerProvider<T1> where T1 : ModelData
    {
        /// <summary>
        ///     The handler access for newly added model objects
        /// </summary>
        IDataConflictHandler<T1, ModelObject> NewModelObjectHandler { get; }

        /// <summary>
        ///     The handler access for changed existing model objects
        /// </summary>
        IDataConflictHandler<T1, ModelObject> ChangedModelObjectsHandler { get; }

        /// <summary>
        ///     The handler access for reindexing of existing model object lists
        /// </summary>
        IDataConflictHandler<T1, ModelObject> ReindexedModelObjectsHandler { get; }

        /// <summary>
        ///     The handler access for removed existing model objects
        /// </summary>
        IDataConflictHandler<T1, ModelObject> RemovedModelObjectsHandler { get; }

        /// <summary>
        ///     The handler access for changed model parameter objects
        /// </summary>
        IDataConflictHandler<T1, ModelParameter> ChangedModelParameterHandler { get; }
    }
}