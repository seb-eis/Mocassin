using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base event arguments class for events that inform about reindexing events for model object collections
    /// </summary>
    public abstract class ModelReindexingEventArgs : EventArgs
    {
        /// <summary>
        ///     The reindexing list that describes the indexing changes
        /// </summary>
        public ReindexingList ReindexingList { get; }

        /// <summary>
        ///     Create new object reindexing event arguments with the provided reindexing list
        /// </summary>
        /// <param name="reindexingList"></param>
        protected ModelReindexingEventArgs(ReindexingList reindexingList)
        {
            ReindexingList = reindexingList ?? throw new ArgumentNullException(nameof(reindexingList));
        }
    }

    /// <summary>
    ///     Reindexing event arguments for a specific type of model objects
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class ModelReindexingEventArgs<T1> : ModelReindexingEventArgs 
        where T1 : IModelObject
    {
        /// <summary>
        ///     Create new object reindexing event arguments with the provided reindexing list
        /// </summary>
        /// <param name="reindexingList"></param>
        protected ModelReindexingEventArgs(ReindexingList reindexingList)
            : base(reindexingList)
        {
        }
    }
}