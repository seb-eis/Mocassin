using System.Collections.ObjectModel;
using Mocassin.UI.Base.ViewModel;

namespace Mocassin.UI.GUI.Base.ViewModels.Collections
{
    /// <summary>
    ///     Basic view model for providing an <see cref="ObservableCollection{T}" /> with a limited size
    /// </summary>
    public class ObservableCollectionViewModel<T> : ViewModel
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
        /// Creates new <see cref="ObservableCollectionViewModel{T}"/>
        /// </summary>
        public ObservableCollectionViewModel()
        {
            capacity = 10;
            ObservableItems = new ObservableCollection<T>();
        }

        /// <summary>
        ///     Appends an item <see cref="T" /> to the end of the observable collection
        /// </summary>
        /// <param name="value"></param>
        public void AppendCollectionItem(T value)
        {
            InvokeOnDispatcher(() => AppendCollectionItemInternal(value));
        }

        /// <summary>
        ///     Internal implementation of the <see cref="AppendCollectionItem" /> method
        /// </summary>
        /// <param name="value"></param>
        private void AppendCollectionItemInternal(T value)
        {
            if (ObservableItems.Count >= Capacity) ObservableItems.RemoveAt(0);
            ObservableItems.Add(value);
        }
    }
}