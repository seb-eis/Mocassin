using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;

namespace Mocassin.Framework.Extensions
{
    /// <summary>
    ///     Extension class for general extension collections
    /// </summary>
    public static class MocassinCollectionExtensions
    {
        /// <summary>
        ///     Uniformly select a subset of entries from a collection using the provided random number generator
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="collection"></param>
        /// <param name="count"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static IEnumerable<T1> SelectRandom<T1>(this ICollection<T1> collection, int count, System.Random random)
        {
            if (count >= collection.Count)
            {
                foreach (var item in collection) yield return item;

                yield break;
            }

            int passed = -1, used = 0;
            foreach (var item in collection)
            {
                if (used == count) yield break;

                if (!(count - used > (collection.Count - ++passed) * random.NextDouble()))
                    continue;

                ++used;
                yield return item;
            }
        }

        /// <summary>
        ///     Project each element of a uniformly selected subset of specified size from a generic collection into a new form
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="collection"></param>
        /// <param name="count"></param>
        /// <param name="random"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T2> SelectRandom<T1, T2>(this ICollection<T1> collection, int count, System.Random random, Func<T1, T2> selector)
        {
            return collection.SelectRandom(count, random).Select(selector);
        }

        /// <summary>
        ///     Adds multiple values to a generic collection
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="collection"></param>
        /// <param name="values"></param>
        public static void AddRange<T1>(this ICollection<T1> collection, IEnumerable<T1> values)
        {
            foreach (var item in values) collection.Add(item);
        }

        /// <summary>
        ///     Selects a set of random index entries from a <see cref="ICollection{T}" />
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="collection"></param>
        /// <param name="count"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static IEnumerable<int> SelectRandomIndex<T1>(this ICollection<T1> collection, int count, System.Random random)
        {
            if (count >= collection.Count)
            {
                for (var i = 0; i < collection.Count; i++) yield return i;

                yield break;
            }

            int passed = -1, used = 0;
            for (var i = 0; i < collection.Count; i++)
            {
                if (used == count) yield break;
                if (count - used > (collection.Count - ++passed) * random.NextDouble())
                {
                    ++used;
                    yield return i;
                }
            }
        }

        /// <summary>
        ///     Causes a one way synchronization of an <see cref="IList{T}" /> to an <see cref="ObservableCollection{T}" />
        ///     observable. Returns a <see cref="IDisposable" /> to cancel the synchronization. Only content is synchronized, not content order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observer"></param>
        /// <param name="observable"></param>
        /// <returns></returns>
        public static IDisposable ListenToContentChanges<T>(this IList<T> observer, ObservableCollection<T> observable)
        {
            void OnChange(object sender, NotifyCollectionChangedEventArgs args)
            {
                if (!ReferenceEquals(sender, observable)) throw new InvalidOperationException("Sender is not the synchronization source.");
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        observer.Add((T) args.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        observer.Remove((T) args.OldItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        observer.Remove((T) args.OldItems[0]);
                        observer.Add((T) args.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        observer.Clear();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            observable.CollectionChanged += OnChange;
            return Disposable.Create(() => observable.CollectionChanged -= OnChange);
        }

        /// <summary>
        ///     Tries to find and return the first item that matches the predicate or uses the provided constructor to create a new
        ///     one. Optional flag to add/not add the item to the source collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="constructor"></param>
        /// <param name="addNewToSource"></param>
        /// <returns></returns>
        public static T FirstOrNew<T>(this ICollection<T> collection, Func<T, bool> predicate, Func<T> constructor, bool addNewToSource = true) where T : class
        {
            var result = collection.FirstOrDefault(predicate);
            if (result != null) return result;
            result = constructor.Invoke();
            if (addNewToSource) collection.Add(result);
            return result;
        }
    }
}