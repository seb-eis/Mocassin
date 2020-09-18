namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Model list indexing event arguments interface that allows the covariant usage of the event arguments with
    ///     non-interface types
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    internal interface IModelIndexingEventArgs<out T1> where T1 : IModelObject
    {
        /// <summary>
        ///     The reindexing list that describes the new object indexing instruction
        /// </summary>
        ReindexingList ReindexingList { get; }
    }
}