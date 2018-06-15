namespace ICon.Model.Basic
{
    /// <summary>
    /// Model list indexing event arguments interface that allows the covariant usage of the event arguments with non-interface types
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    interface IModelIndexingEventArgs<T1> where T1 : IModelObject
    {
        /// <summary>
        /// The reindexing list that describes the new obect indexing instruction
        /// </summary>
        ReindexingList ReindexingList { get; }
    }
}
