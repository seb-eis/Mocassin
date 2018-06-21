using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Extensions;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Manager for indexed data objects that handles add/remove/insert operations on a list with indexed objects
    /// </summary>
    public class IndexedDataManager<TObject> where  TObject : ModelObject, IModelObject
    {
        /// <summary>
        /// Indexes the passed set of new model objects based upon the current end of the object list and adds them accordingly
        /// </summary>
        /// <param name="currentObjects"></param>
        /// <param name="newObjects"></param>
        public void IndexAndAdd(IList<TObject> currentObjects, IEnumerable<TObject> newObjects)
        {
            int index = currentObjects.Count - 1;
            foreach (var item in newObjects)
            {
                item.Index = ++index;
                currentObjects.Add(item);
            }
        }

        /// <summary>
        /// Indexes the passed set of ne model objects based upon either deprecated places or the end of the list and addss the accordingly
        /// </summary>
        /// <param name="currentObjects"></param>
        /// <param name="newObjects"></param>
        public void IndexAndAddUseDeprecated(IList<TObject> currentObjects, IEnumerable<TObject> newObjects)
        {
            foreach (var item in newObjects)
            {
                item.Index = currentObjects.ReplaceFirstOrAdd(value => value.IsDeprecated, item);
            }
        }

        /// <summary>
        /// Marks all model objects as deprecated that match the provided prediacte. Returns sequence of all deprecated objects
        /// </summary>
        /// <param name="currentObjects"></param>
        /// <param name="predicate"></param>
        public IEnumerable<TObject> DeprecateAll(IEnumerable<TObject> currentObjects, Predicate<TObject> predicate)
        {
            foreach (var item in currentObjects)
            {
                item.IsDeprecated = predicate(item);
                if (item.IsDeprecated)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Remov all objects that match the provided predicate and reindex the remaining. Returns a reindexing list that describes the changes
        /// </summary>
        /// <param name="currentObjects"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ReindexingList RemoveAndReindex(IList<TObject> currentObjects, Predicate<TObject> predicate)
        {
            var reindexingList = new ReindexingList(currentObjects.Count);
            int newIndex = -1;
            foreach (var item in currentObjects)
            {
                if (predicate(item))
                {
                    reindexingList.Add((item.Index, -1));
                    item.Index = -1;
                }
                else
                {
                    reindexingList.Add((item.Index, ++newIndex));
                    item.Index = newIndex;
                }
            }
            currentObjects.RemoveAll(value => value.Index == -1);
            return reindexingList;
        }
    }
}
