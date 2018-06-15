using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Carries reindexing information for the contents of a model data container after deprecated data was removed as pairs of old and new index (-1 means removed)
    /// </summary>
    public class ReindexingList : IList<(int Old, int New)>
    {
        /// <summary>
        /// Contains the actual reindexing info as pairs of new and old index
        /// </summary>
        public List<(int Old, int New)> Data;

        /// <summary>
        /// Creates new reindexing list with an initial capacity of 0
        /// </summary>
        public ReindexingList() : this(0)
        {

        }

        /// <summary>
        /// Creates new list with specified start capacity
        /// </summary>
        /// <param name="capacity"></param>
        public ReindexingList(Int32 capacity)
        {
            Data = new List<(Int32 Old, Int32 New)>(capacity);
        }

        /// <summary>
        /// Index based access
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (Int32 Old, Int32 New) this[Int32 index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }

        /// <summary>
        /// Number of reindexing entries
        /// </summary>
        public Int32 Count => Data.Count;

        /// <summary>
        /// Flag if read only (Always false)
        /// </summary>
        public Boolean IsReadOnly => false;

        /// <summary>
        /// Adds new item to the list
        /// </summary>
        /// <param name="item"></param>
        public void Add((Int32 Old, Int32 New) item)
        {
            Data.Add(item);
        }

        /// <summary>
        /// Clears reindexing info
        /// </summary>
        public void Clear()
        {
            Data.Clear();
        }

        /// <summary>
        /// Checks if an item is contained in the list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Contains((Int32 Old, Int32 New) item)
        {
            return Data.Contains(item);
        }

        /// <summary>
        /// Copies internal reindexing list to array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo((Int32 Old, Int32 New)[] array, Int32 arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns enumerator for internal reindexing list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<(Int32 Old, Int32 New)> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of an item if present
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Int32 IndexOf((Int32 Old, Int32 New) item)
        {
            return Data.IndexOf(item);
        }

        /// <summary>
        /// Inserts item at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(Int32 index, (Int32 Old, Int32 New) item)
        {
            Data.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurence of the specified item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Remove((Int32 Old, Int32 New) item)
        {
            return Data.Remove(item);
        }

        /// <summary>
        /// Removes item at the provided index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(Int32 index)
        {
            Data.RemoveAt(index);
        }

        /// <summary>
        /// Get the enumerator for the internal list
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        /// Set the internal list capacity to the actual number of entries
        /// </summary>
        public void TrimExcess()
        {
            Data.TrimExcess();
        }
    }
}
