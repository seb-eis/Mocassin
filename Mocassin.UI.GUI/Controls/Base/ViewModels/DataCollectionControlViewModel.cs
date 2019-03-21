using System.Collections.Generic;
using Mocassin.UI.GUI.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.Base.ViewModels
{
    /// <summary>
    ///     Generic base <see cref="ViewModelBase" /> for views that provide collection data for manipulation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataCollectionControlViewModel<T> : ViewModelBase
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
    }
}