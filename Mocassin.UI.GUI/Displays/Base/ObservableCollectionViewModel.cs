using System.Collections.ObjectModel;
using Mocassin.UI.Base.ViewModel;

namespace Mocassin.UI.GUI.Displays.Base
{
    /// <summary>
    ///     Basic view model for providing an <see cref="ObservableCollection{T}" /> with a limited size
    /// </summary>
    public class ObservableCollectionViewModel<T> : ViewModelBase
    {
        /// <summary>
        ///     The <see cref="Capacity" /> backing field
        /// </summary>
        private int _capacity;

        /// <summary>
        ///     Get the <see cref="ObservableCollection{T}" /> that contains the string messages
        /// </summary>
        public ObservableCollection<T> ViewCollection { get; }

        /// <summary>
        ///     Get or set the max size of the string collection
        /// </summary>
        public int Capacity
        {
            get => _capacity;
            set => SetProperty(ref _capacity, value);
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
            if (ViewCollection.Count >= Capacity) ViewCollection.RemoveAt(0);
            ViewCollection.Add(value);
        }
    }
}