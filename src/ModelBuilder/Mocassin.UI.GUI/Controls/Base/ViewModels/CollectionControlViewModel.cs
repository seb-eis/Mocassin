using System;
using System.Collections.Generic;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.Base.ViewModels
{
    /// <summary>
    ///     Generic base <see cref="ViewModelBase" /> for views that provide <see cref="ICollection{T}" /> interfaces
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionControlViewModel<T> : ViewModelBase
    {
        private ICollection<T> items;
        private T selectedItem;

        /// <summary>
        ///     Get or set the <see cref="ICollection{T}" /> fo
        /// </summary>
        public ICollection<T> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        /// <summary>
        ///     Get or set the selected collection item
        /// </summary>
        public T SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        /// <summary>
        ///     Set a new <see cref="ICollection{T}" /> for the view model
        /// </summary>
        /// <param name="collection"></param>
        public virtual void SetCollection(ICollection<T> collection)
        {
            Items = collection;
            SelectedItem = default;
        }

        /// <summary>
        ///     Resynchronizes the view model with potential subscribers by force resetting the internal properties
        /// </summary>
        public void Resynchronize()
        {
            var itemBackup = SelectedItem;
            var itemsBackup = Items;
            ExecuteOnAppThread(() => SetCollection(null));
            ExecuteOnAppThread(() =>
            {
                SetCollection(itemsBackup);
                SelectedItem = itemBackup;
            });
        }

        /// <summary>
        ///     Duplicates an item from the collection and appends the items to the end of the collection
        /// </summary>
        /// <param name="item"></param>
        /// <param name="duplicator"></param>
        /// <param name="count"></param>
        public void DuplicateItem(T item, Func<T, T> duplicator, int count = 1)
        {
            if (!Items.Contains(item)) throw new InvalidOperationException("Item is not part of the collection.");
            var tempList = new List<T>(count);
            for (var i = 0; i < count; i++) tempList.Add(duplicator(item));
            ExecuteOnAppThread(() => Items.AddRange(tempList));
            Resynchronize();
        }
    }
}