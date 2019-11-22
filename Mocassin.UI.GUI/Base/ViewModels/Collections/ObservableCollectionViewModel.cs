using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mocassin.UI.GUI.Base.ViewModels.Collections
{
    /// <summary>
    ///     Base <see cref="ViewModelBase" /> for providing an <see cref="ObservableCollection{T}" /> with a limited size
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
        ///     Get or set the max size of the string collection
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
        public void InsertCollectionItem(int index, T value)
        {
            ExecuteOnAppThread(() => InsertCollectionItemInternal(index, value));
        }

        /// <inheritdoc />
        public void AddCollectionItem(T value)
        {
            ExecuteOnAppThread(() => AddCollectionItemInternal(value));
        }

        /// <inheritdoc />
        public void AddCollectionItems(IEnumerable<T> values)
        {
            ExecuteOnAppThread(() => AddCollectionItemsInternal(values));
        }

        /// <inheritdoc />
        public void RemoveCollectionItem(T value)
        {
            ExecuteOnAppThread(() => RemoveCollectionItemInternal(value));
        }

        /// <inheritdoc />
        public bool CollectionContains(T value)
        {
            return ObservableItems.Contains(value);
        }

        /// <inheritdoc />
        public void MoveCollectionItem(int oldIndex, int newIndex)
        {
            ExecuteOnAppThread(() => MoveCollectionItemInternal(oldIndex, newIndex));
        }

        /// <inheritdoc />
        public void ClearCollection()
        {
            ExecuteOnAppThread(ClearCollectionInternal);
        }
        /// <summary>
        ///     Internal implementation of <see cref="MoveCollectionItem"/>
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>

        protected virtual void MoveCollectionItemInternal(int oldIndex, int newIndex)
        {
            newIndex = newIndex >= 0 ? newIndex : ObservableItems.Count - 1;
            ObservableItems.Move(oldIndex, newIndex);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="AddCollectionItem" /> method
        /// </summary>
        /// <param name="value"></param>
        protected virtual void AddCollectionItemInternal(T value)
        {
            if (MaxCapacity > 0 && ObservableItems.Count >= MaxCapacity) ObservableItems.RemoveAt(0);
            ObservableItems.Add(value);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="RemoveCollectionItem" /> method
        /// </summary>
        /// <param name="value"></param>
        protected virtual void RemoveCollectionItemInternal(T value)
        {
            ObservableItems.Remove(value);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="InsertCollectionItem" /> method
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        protected virtual void InsertCollectionItemInternal(int index, T value)
        {
            ObservableItems.Insert(index, value);
            if (MaxCapacity > 0 && ObservableItems.Count >= MaxCapacity) ObservableItems.RemoveAt(0);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="ClearCollection"/> method
        /// </summary>
        protected virtual void ClearCollectionInternal()
        {
            ObservableItems.Clear();
        }

        /// <summary>
        ///     Internal implementation of the <see cref="AddCollectionItems" /> method
        /// </summary>
        protected virtual void AddCollectionItemsInternal(IEnumerable<T> values)
        {
            if (values == null) return;
            foreach (var item in values) AddCollectionItemInternal(item);
        }
    }
}