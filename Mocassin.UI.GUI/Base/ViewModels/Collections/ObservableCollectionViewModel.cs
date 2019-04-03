﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mocassin.UI.GUI.Base.ViewModels;

namespace Mocassin.UI.GUI.Base.ViewModels.Collections
{
    /// <summary>
    ///     Base <see cref="ViewModelBase" /> for providing an <see cref="ObservableCollection{T}" /> with a limited size
    /// </summary>
    public class ObservableCollectionViewModel<T> : ViewModelBase, IObservableCollectionViewModel<T>
    {
        /// <summary>
        ///     The <see cref="Capacity" /> backing field
        /// </summary>
        private int capacity;

        /// <summary>
        ///     Get the <see cref="ObservableCollection{T}" /> that contains the string messages
        /// </summary>
        public ObservableCollection<T> ObservableItems { get; }

        /// <summary>
        ///     Get or set the max size of the string collection
        /// </summary>
        public int Capacity
        {
            get => capacity;
            set => SetProperty(ref capacity, value);
        }

        /// <summary>
        ///     Creates new <see cref="ObservableCollectionViewModel{T}" /> with unlimited capacity
        /// </summary>
        public ObservableCollectionViewModel()
        {
            capacity = -1;
            ObservableItems = new ObservableCollection<T>();
        }

        /// <summary>
        ///     Creates new <see cref="ObservableCollectionViewModel{T}" /> with specified capacity limit
        /// </summary>
        public ObservableCollectionViewModel(int capacity)
        {
            Capacity = capacity;
            ObservableItems = new ObservableCollection<T>();
        }

        /// <inheritdoc />
        public void InsertCollectionItem(int index, T value)
        {
            ExecuteOnDispatcher(() => InsertCollectionItemInternal(index, value));
        }

        /// <inheritdoc />
        public void AddCollectionItem(T value)
        {
            ExecuteOnDispatcher(() => AddCollectionItemInternal(value));
        }

        /// <inheritdoc />
        public void AddCollectionItems(IEnumerable<T> values)
        {
            ExecuteOnDispatcher(() => AddCollectionItemsInternal(values));
        }

        /// <inheritdoc />
        public void RemoveCollectionItem(T value)
        {
            ExecuteOnDispatcher(() => RemoveCollectionItemInternal(value));
        }

        /// <inheritdoc />
        public bool CollectionContains(T value)
        {
            return ObservableItems.Contains(value);
        }

        /// <inheritdoc />
        public void ClearCollection()
        {
            ExecuteOnDispatcher(ClearCollectionInternal);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="AddCollectionItem" /> method
        /// </summary>
        /// <param name="value"></param>
        protected virtual void AddCollectionItemInternal(T value)
        {
            if (Capacity > 0 && ObservableItems.Count >= Capacity) ObservableItems.RemoveAt(0);
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
            if (Capacity > 0 && ObservableItems.Count >= Capacity) ObservableItems.RemoveAt(0);
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