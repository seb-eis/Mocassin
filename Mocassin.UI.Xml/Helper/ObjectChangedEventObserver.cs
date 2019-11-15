using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using Mocassin.Framework.Events;

namespace Mocassin.UI.Xml.Helper
{
    /// <summary>
    ///     Observer system to watch the entire <see cref="INotifyCollectionChanged" /> and
    ///     <see cref="INotifyPropertyChanged" /> implementing properties of an object tree
    /// </summary>
    public class ObjectChangedEventObserver : IDisposable
    {
        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> that relays change event arguments and sender
        /// </summary>
        private ReactiveEvent<(object Sender, EventArgs args)> ChangeDetectedEvent { get; }

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for observation starts
        /// </summary>
        private ReactiveEvent<object> ObjectObservationStartedEvent { get; }

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for observation ends
        /// </summary>
        private ReactiveEvent<object> ObjectObservationEndedEvent { get; }

        /// <summary>
        ///     Get the <see cref="Dictionary{TKey,TValue}" /> that holds the <see cref="IDisposable" /> for each observed item
        /// </summary>
        private Dictionary<object, IDisposable> Subscriptions { get; }

        /// <summary>
        ///     Get or set the observed root object
        /// </summary>
        private object ObservedRootObject { get; set; }

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that provides change detected notifications
        /// </summary>
        public IObservable<(object Sender, EventArgs args)> ChangeDetectedNotifications => ChangeDetectedEvent.AsObservable();

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that provides observation started notifications
        /// </summary>
        public IObservable<object> ObjectObservationStartedNotifications => ObjectObservationStartedEvent.AsObservable();

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that provides observation ended notifications
        /// </summary>
        public IObservable<object> ObjectObservationEndedNotifications => ObjectObservationEndedEvent.AsObservable();

        /// <inheritdoc />
        public ObjectChangedEventObserver()
        {
            ChangeDetectedEvent = new ReactiveEvent<(object Sender, EventArgs args)>();
            ObjectObservationEndedEvent = new ReactiveEvent<object>();
            ObjectObservationStartedEvent = new ReactiveEvent<object>();
            Subscriptions = new Dictionary<object, IDisposable>();
        }

        /// <summary>
        ///     Sets the provided object as the observation root and attaches itself the the change notification events
        /// </summary>
        /// <param name="rootObject"></param>
        public void ObserveObject(object rootObject)
        {
            if (ReferenceEquals(ObservedRootObject, rootObject)) return;
            if (ObservedRootObject != null) throw new InvalidOperationException("Change subscriber already attached to another project.");
            BeginListeningToAccessibleEvents(rootObject);
            ObservedRootObject = rootObject;
        }

        /// <summary>
        ///     Stops observation of the currently set root object
        /// </summary>
        public void StopObservation()
        {
            if (ObservedRootObject == null) return;
            foreach (var subscription in Subscriptions.Values) subscription.Dispose();
            Subscriptions.Clear();
            ObservedRootObject = null;
        }

        /// <summary>
        ///     Attach to all unknown observable objects on the provided root object and begins to listening to the change events
        /// </summary>
        /// <param name="root"></param>
        /// <param name="bindingFlags"></param>
        private void BeginListeningToAccessibleEvents(object root, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (root == null || Subscriptions.ContainsKey(root)) return;
            if (Subscriptions.Count == 0)
            {
                foreach (var item in FindObservables(root)) CreateTrackingEntry(item);
                return;
            }

            foreach (var item in FindUnknownObjects(root)) CreateTrackingEntry(item);
        }

        /// <summary>
        ///     Stops to listen to all events that exist in the provided object tree but cannot be accessed through the observed root object
        /// </summary>
        /// <param name="removedObject"></param>
        /// <param name="bindingFlags"></param>
        private void StopListeningToInaccessibleEvents(object removedObject, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (removedObject == null) return;
            foreach (var item in FindInaccessibleObjects(removedObject)) RemoveTrackingEntry(item);
        }

        /// <summary>
        ///     Adds a tracking reference for the provided object. Returns the new number of references or a negative value if the
        ///     object cannot be tracked
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private void CreateTrackingEntry(object obj)
        {
            switch (obj)
            {
                case INotifyCollectionChanged notifyCollectionChanged:
                    TrackCollectionProperty(notifyCollectionChanged);
                    break;

                case INotifyPropertyChanged notifyPropertyChanged:
                    TrackProperty(notifyPropertyChanged);
                    break;

                default:
                    throw new InvalidOperationException("Provided object cannot be tracked.");
            }

            OnObservationStarted(obj);
        }

        /// <summary>
        ///     Removes the tracking entry fro the provided object
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveTrackingEntry(object obj)
        {
            if (!Subscriptions.TryGetValue(obj, out var disposable)) return;
            disposable.Dispose();
            Subscriptions.Remove(obj);
            OnObservationEnded(obj);
        }

        /// <summary>
        ///     Creates and adds a subscription tracker for a <see cref="INotifyPropertyChanged" /> object
        /// </summary>
        /// <param name="notifyPropertyChanged"></param>
        private void TrackProperty(INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += OnChangeDetected;
            var disposable = Disposable.Create(() => notifyPropertyChanged.PropertyChanged -= OnChangeDetected);
            Subscriptions.Add(notifyPropertyChanged, disposable);
        }

        /// <summary>
        ///     Creates and adds a subscription tracker for a <see cref="INotifyCollectionChanged" /> object
        /// </summary>
        /// <param name="notifyCollectionChanged"></param>
        private void TrackCollectionProperty(INotifyCollectionChanged notifyCollectionChanged)
        {
            notifyCollectionChanged.CollectionChanged += OnChangeDetected;
            var disposable = Disposable.Create(() => notifyCollectionChanged.CollectionChanged -= OnChangeDetected);
            Subscriptions.Add(notifyCollectionChanged, disposable);
        }

        /// <summary>
        ///     Recursively searches the object tree for all objects that are observable from it
        /// </summary>
        /// <param name="root"></param>
        /// <param name="set"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public HashSet<object> FindObservables(object root, HashSet<object> set = null, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            set = set ?? new HashSet<object>();
            if (set.Contains(root)) return set;
            set.Add(root);

            if (root is INotifyCollectionChanged notifyCollectionChanged)
            {
                foreach (var item in EnsureIsCollection(notifyCollectionChanged)) FindObservables(item, set);
                return set;
            }

            foreach (var propertyInfo in root.GetType().GetProperties(bindingFlags))
            {
                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(propertyInfo.PropertyType) &&
                    !typeof(INotifyCollectionChanged).IsAssignableFrom(propertyInfo.PropertyType)) continue;

                if (!(propertyInfo.GetValue(root) is object childObject)) continue;
                FindObservables(childObject, set);
            }

            return set;
        }

        /// <summary>
        ///     Gets the <see cref="HashSet{T}" /> of objects that are accessible on the provided object tree but not from the root
        ///     object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private HashSet<object> FindInaccessibleObjects(object obj)
        {
            var objObservables = FindObservables(obj);
            foreach (var knownObject in FindObservables(ObservedRootObject)) objObservables.Remove(knownObject);
            return objObservables;
        }

        /// <summary>
        ///     Gets the <see cref="HashSet{T}" /> of objects that are currently unknown to the tracker but are accessible from the provided object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private HashSet<object> FindUnknownObjects(object obj)
        {
            var objObservables = FindObservables(obj);
            foreach (var knownObject in Subscriptions.Keys) objObservables.Remove(knownObject);
            return objObservables;
        }

        /// <summary>
        ///     Removes subscriptions that have become inaccessible
        /// </summary>
        private void RemoveInaccessibleSubscriptions()
        {
            var accessibles = FindObservables(ObservedRootObject);
            foreach (var key in Subscriptions.Keys.ToList().Where(x => !accessibles.Contains(x)))
            {
                RemoveTrackingEntry(key);
            }
        }

        /// <summary>
        ///     Action that is called when a previously unknown object is now observed
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnObservationStarted(object obj)
        {
            ObjectObservationStartedEvent.OnNext(obj);
        }

        /// <summary>
        ///     Action that is called when a previously known object was removed from the observation
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnObservationEnded(object obj)
        {
            ObjectObservationEndedEvent.OnNext(obj);
        }

        /// <summary>
        ///     Converts the provided <see cref="INotifyCollectionChanged" /> to a <see cref="ICollection" /> and throws if the
        ///     cast cannot be done
        /// </summary>
        /// <param name="notifyCollectionChanged"></param>
        /// <returns></returns>
        protected ICollection EnsureIsCollection(INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged as ICollection
                   ?? throw new InvalidOperationException("INotifyCollectionChanged requires ICollection to be implemented.");
        }

        /// <summary>
        ///     Action that is attached to all change events of the object tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnChangeDetected(object sender, EventArgs args)
        {
            switch (args)
            {
                case PropertyChangedEventArgs propertyChangedEventArgs:
                    RemoveInaccessibleSubscriptions();
                    var value = sender.GetType().GetProperty(propertyChangedEventArgs.PropertyName)?.GetValue(sender);
                    if (value != null) BeginListeningToAccessibleEvents(value);
                    break;

                case NotifyCollectionChangedEventArgs collectionChangedEventArgs:
                {
                    if (collectionChangedEventArgs.Action == NotifyCollectionChangedAction.Reset)
                    {
                        RemoveInaccessibleSubscriptions();
                        return;
                    }

                    foreach (var oldItem in (IEnumerable) collectionChangedEventArgs.OldItems ?? Enumerable.Empty<object>())
                    {
                        if (oldItem == null) continue;
                        StopListeningToInaccessibleEvents(oldItem);
                    }

                    foreach (var newItem in (IEnumerable) collectionChangedEventArgs.NewItems ?? Enumerable.Empty<object>())
                    {
                        BeginListeningToAccessibleEvents(newItem);
                    }

                    break;
                }
            }

            ChangeDetectedEvent.OnNext((sender, args));
        }

        /// <summary>
        ///     Notifies event subscribers that the events completed
        /// </summary>
        public void NotifyEventsCompleted()
        {
            ChangeDetectedEvent.OnCompleted();
            ObjectObservationStartedEvent.OnCompleted();
            ObjectObservationEndedEvent.OnCompleted();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            StopObservation();
            NotifyEventsCompleted();
        }
    }
}