﻿namespace Mocassin.UI.GUI.Base.ViewModels.Collections
{
    /// <summary>
    /// Represents a view model for a collection that supports affiliated remove, add and insert operations
    /// </summary>
    public interface ICollectionViewModel<in T>
    {
        /// <summary>
        ///     Inserts and item <see cref="T"/> into the view model collection at the given index
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
        ///     Removes the first occurence of <see cref="T"/> from the view model collection
        /// </summary>
        /// <param name="value"></param>
        void RemoveCollectionItem(T value);

        /// <summary>
        ///     Checks if the vale <see cref="T"/> is part of the view model collection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CollectionContains(T value);
    }
}