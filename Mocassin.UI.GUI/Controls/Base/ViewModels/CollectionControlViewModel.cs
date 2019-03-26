using System.Collections.Generic;
using Mocassin.UI.GUI.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.Base.ViewModels
{
    /// <summary>
    ///     Generic base <see cref="ViewModelBase" /> for views that provide <see cref="ICollection{T}"/> interfaces
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionControlViewModel<T> : ViewModelBase
    {
        private T selectedCollectionItem;
        private ICollection<T> dataCollection;

        /// <summary>
        ///     Get or set the <see cref="ICollection{T}" /> fo
        /// </summary>
        public ICollection<T> DataCollection
        {
            get => dataCollection;
            set => SetProperty(ref dataCollection, value);
        }

        /// <summary>
        ///     Get or set the selected collection item <see cref="T" />
        /// </summary>
        public T SelectedCollectionItem
        {
            get => selectedCollectionItem;
            set => SetProperty(ref selectedCollectionItem, value);
        }

        /// <summary>
        ///     Set a new <see cref="ICollection{T}"/> for the view model
        /// </summary>
        /// <param name="collection"></param>
        public void SetCollection(ICollection<T> collection)
        {
            DataCollection = collection;
            SelectedCollectionItem = default;
        }
    }
}