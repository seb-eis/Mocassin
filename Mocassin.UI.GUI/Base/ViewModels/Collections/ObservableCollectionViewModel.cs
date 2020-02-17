using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mocassin.UI.GUI.Base.ViewModels.Collections
{
    /// <summary>
    ///     Base <see cref="ViewModelBase" /> for providing an <see cref="ObservableCollection{T}" /> with size restrictions
    ///     and ensured app thread executed collection modification
    /// </summary>
    public class ObservableCollectionViewModel<T> : ViewModelBase, IObservableCollectionViewModel<T>
    {
        /// <summary>
        ///     The <see cref="MaxCapacity" /> backing field
        /// </summary>
        private int maxCapacity;

        /// <summary>
        ///     Get the <see cref="ObservableCollection{T}" /> that contains the string messages
        /// </summary>
        public ObservableCollection<T> ObservableItems { get; }

        /// <summary>
        ///     Get or set the max size of the collection. Negative values lift the capacity restriction
        /// </summary>
        public int MaxCapacity
        {
            get => maxCapacity;
            set => SetProperty(ref maxCapacity, value);
        }

        /// <summary>
        ///     Creates new <see cref="ObservableCollectionViewModel{T}" /> with unlimited max capacity
        /// </summary>
        public ObservableCollectionViewModel()
        {
            maxCapacity = -1;
            ObservableItems = new ObservableCollection<T>();
        }

        /// <summary>
        ///     Creates new <see cref="ObservableCollectionViewModel{T}" /> with specified maxCapacity limit
        /// </summary>
        public ObservableCollectionViewModel(int maxCapacity)
        {
            MaxCapacity = maxCapacity;
            ObservableItems = new ObservableCollection<T>();
        }

        /// <inheritdoc />
        public void InsertItem(int index, T value)
        {
            ExecuteOnAppThread(() => InsertItemInternal(index, value));
        }

        /// <inheritdoc />
        public void AddItem(T value)
        {
            ExecuteOnAppThread(() => AddItemInternal(value));
        }

        /// <inheritdoc />
        public void AddItems(IEnumerable<T> values)
        {
            ExecuteOnAppThread(() => AddItemsInternal(values));
        }

        /// <inheritdoc />
        public void RemoveItem(T value)
        {
            ExecuteOnAppThread(() => RemoveItemInternal(value));
        }

        /// <inheritdoc />
        public bool Contains(T value)
        {
            return ObservableItems.Contains(value);
        }

        /// <inheritdoc />
        public void MoveItem(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex) return;
            ExecuteOnAppThread(() => MoveItemInternal(oldIndex, newIndex));
        }

        /// <inheritdoc />
        public void Clear()
        {
            ExecuteOnAppThread(ClearInternal);
        }

        /// <summary>
        ///     Calls dispose on all items that implement <see cref="IDisposable"/> and then clears the collection. This call is always executed on the application thread
        /// </summary>
        public void DisposeAllAndClear()
        {
            ExecuteOnAppThread(DisposeAllAndClearInternal);
        }

        /// <summary>
        ///     Get the first occurence of an item that matches the predicate or creates a new one if no match is found. By default the new item is added to the collection
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="constructor"></param>
        /// <param name="addNewToCollection"></param>
        /// <returns></returns>
        public T FirstOrNew(Func<T, bool> predicate, Func<T> constructor, bool addNewToCollection = true)
        {
            var result = ObservableItems.FirstOrDefault(predicate);
            if (result != null && !typeof(T).IsValueType || typeof(T).IsValueType && ObservableItems.Any(predicate)) return result;
            result = constructor.Invoke();
            if (addNewToCollection) AddItem(result);
            return result;
        }

        /// <summary>
        ///     Internal implementation of <see cref="DisposeAllAndClear"/>
        /// </summary>
        protected virtual void DisposeAllAndClearInternal()
        {
            foreach (var item in ObservableItems) (item as IDisposable)?.Dispose();
            ObservableItems.Clear();
        }

        /// <summary>
        ///     Internal implementation of <see cref="MoveItem" />
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        protected virtual void MoveItemInternal(int oldIndex, int newIndex)
        {
            newIndex = newIndex >= 0 ? newIndex : ObservableItems.Count - 1;
            ObservableItems.Move(oldIndex, newIndex);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="AddItem" /> method
        /// </summary>
        /// <param name="value"></param>
        protected virtual void AddItemInternal(T value)
        {
            if (MaxCapacity > 0 && ObservableItems.Count >= MaxCapacity) ObservableItems.RemoveAt(0);
            ObservableItems.Add(value);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="RemoveItem" /> method
        /// </summary>
        /// <param name="value"></param>
        protected virtual void RemoveItemInternal(T value)
        {
            ObservableItems.Remove(value);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="InsertItem" /> method
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        protected virtual void InsertItemInternal(int index, T value)
        {
            ObservableItems.Insert(index, value);
            if (MaxCapacity > 0 && ObservableItems.Count >= MaxCapacity) ObservableItems.RemoveAt(0);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="Clear" /> method
        /// </summary>
        protected virtual void ClearInternal()
        {
            ObservableItems.Clear();
        }

        /// <summary>
        ///     Internal implementation of the <see cref="AddItems" /> method
        /// </summary>
        protected virtual void AddItemsInternal(IEnumerable<T> values)
        {
            if (values == null) return;
            foreach (var item in values) AddItemInternal(item);
        }
    }
}