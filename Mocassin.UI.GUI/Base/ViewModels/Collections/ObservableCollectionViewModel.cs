using System.Collections.ObjectModel;
using Mocassin.UI.GUI.Base.ViewModels;

namespace Mocassin.UI.GUI.Base.ViewModels.Collections
{
    /// <summary>
    ///     Base <see cref="ViewModel" /> for providing an <see cref="ObservableCollection{T}" /> with a limited size
    /// </summary>
    public class ObservableCollectionViewModel<T> : ViewModel, IObservableCollectionViewModel<T>
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
        ///     Creates new <see cref="ObservableCollectionViewModel{T}" />
        /// </summary>
        public ObservableCollectionViewModel()
        {
            capacity = 10;
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
            InvokeOnDispatcher(() => InsertCollectionItemInternal(index, value));
        }

        /// <inheritdoc />
        public void AddCollectionItem(T value)
        {
            InvokeOnDispatcher(() => AddCollectionItemInternal(value));
        }

        /// <inheritdoc />
        public void RemoveCollectionItem(T value)
        {
            InvokeOnDispatcher(() => RemoveCollectionItemInternal(value));
        }

        /// <inheritdoc />
        public bool CollectionContains(T value)
        {
            return ObservableItems.Contains(value);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="AddCollectionItem" /> method
        /// </summary>
        /// <param name="value"></param>
        protected virtual void AddCollectionItemInternal(T value)
        {
            if (ObservableItems.Count >= Capacity) ObservableItems.RemoveAt(0);
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
        }
    }
}