using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Mocassin.UI.GUI.Base.ViewModels.Collections
{
    /// <summary>
    ///     Represents a view model for an <see cref="ObservableCollection{T}" /> that supports affiliated remove, add and
    ///     insert operations and ensures that the actions are performed on the app thread
    /// </summary>
    public interface IObservableCollectionViewModel<T> : INotifyPropertyChanged
    {
        /// <summary>
        ///     Get the <see cref="ObservableCollection{T}" /> that the model controls
        /// </summary>
        ObservableCollection<T> ObservableItems { get; }

        /// <summary>
        ///     Inserts and item <see cref="T" /> into the view model collection at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        void InsertCollectionItem(int index, T value);

        /// <summary>
        ///     Appends an item <see cref="T" /> to the end of the view model collection
        /// </summary>
        /// <param name="value"></param>
        void AddCollectionItem(T value);

        /// <summary>
        ///     Appends an <see cref="IEnumerable{T}" /> sequence of item <see cref="T" /> to the end of the view model collection
        /// </summary>
        void AddCollectionItems(IEnumerable<T> values);

        /// <summary>
        ///     Removes the first occurence of <see cref="T" /> from the view model collection
        /// </summary>
        /// <param name="value"></param>
        void RemoveCollectionItem(T value);

        /// <summary>
        ///     Checks if the vale <see cref="T" /> is part of the view model collection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CollectionContains(T value);

        /// <summary>
        ///     Moves a collection item from one index to another. Negative values for the new index default to the last entry
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        void MoveCollectionItem(int oldIndex, int newIndex);

        /// <summary>
        ///     Clears all contents of the collection
        /// </summary>
        void ClearCollection();
    }
}