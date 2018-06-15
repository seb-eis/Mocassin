using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base event arguments class for events that inform about reindexing events for model object collections
    /// </summary>
    public abstract class ModelIndexingEventArgs : EventArgs
    {
        /// <summary>
        /// The reindexing list that describes the indexing changes
        /// </summary>
        public ReindexingList ReindexingList { get; }

        /// <summary>
        /// Create new object reindexing event arguments with the provided reindexing list
        /// </summary>
        /// <param name="reindexingList"></param>
        protected ModelIndexingEventArgs(ReindexingList reindexingList)
        {
            ReindexingList = reindexingList ?? throw new ArgumentNullException(nameof(reindexingList));
        }
    }

    /// <summary>
    /// Reindexing event arguments for a specific type of model objects
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class ModelIndexingEventArgs<T1> : ModelIndexingEventArgs, IModelIndexingEventArgs<T1> where T1 : IModelObject
    {
        /// <summary>
        /// Create new object reindexing event arguments with the provided reindeing list
        /// </summary>
        /// <param name="reindexingList"></param>
        public ModelIndexingEventArgs(ReindexingList reindexingList) : base(reindexingList)
        {

        }
    }
}
