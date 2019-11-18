using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
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
        ///     Get the <see cref="ImmutableHashSet{T}"/> of property <see cref="Type"/> values that are ignored by default
        /// </summary>
        public static ImmutableHashSet<Type> DefaultIgnoredPropertyTypes { get; }

        private object LockObject { get; } = new object();

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
        private object RootObject { get; set; }

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

        /// <summary>
        ///     Get a <see cref="HashSet{T}"/> of property <see cref="Type"/> that should be ignored during recursive event search
        /// </summary>
        public HashSet<Type> IgnoredTypes { get; }


        /// <inheritdoc />
        static ObjectChangedEventObserver()
        {
            DefaultIgnoredPropertyTypes = new HashSet<Type>
            {
                typeof(string), typeof(int), typeof(long), typeof(double), typeof(float), typeof(ulong), typeof(uint), typeof(byte), typeof(short),
                typeof(ushort)
            }.ToImmutableHashSet();
        }

        /// <summary>
        ///     Creates a new <see cref="ObjectChangedEventObserver"/> with an optional set of ignored property <see cref="Type"/> values
        /// </summary>
        /// <param name="ignoredTypes"></param>
        public ObjectChangedEventObserver(IEnumerable<Type> ignoredTypes = null)
        {
            ChangeDetectedEvent = new ReactiveEvent<(object Sender, EventArgs args)>();
            ObjectObservationEndedEvent = new ReactiveEvent<object>();
            ObjectObservationStartedEvent = new ReactiveEvent<object>();
            Subscriptions = new Dictionary<object, IDisposable>();
            IgnoredTypes = new HashSet<Type>((ignoredTypes ?? Enumerable.Empty<Type>()).Concat(DefaultIgnoredPropertyTypes));
        }

        /// <summary>
        ///     Sets the provided object as the observation root and attaches itself the the change notification events
        /// </summary>
        /// <param name="rootObject"></param>
        public void ObserveObject(object rootObject)
        {
            if (rootObject == null) throw new ArgumentNullException(nameof(rootObject));
            if (ReferenceEquals(RootObject, rootObject)) return;
            if (RootObject != null) throw new InvalidOperationException("Change subscriber already attached to another project.");
            if (IgnoredTypes.Contains(rootObject.GetType())) throw new InvalidOperationException("The object type is set to ignore.");
            SubscribeToAccessibleEvents(rootObject);
            RootObject = rootObject;
        }

        /// <summary>
        ///     Stops observation of the currently set root object
        /// </summary>
        public void StopObservation()
        {
            if (RootObject == null) return;
            foreach (var subscription in Subscriptions.Values) subscription.Dispose();
            Subscriptions.Clear();
            RootObject = null;
        }

        /// <summary>
        ///     Attach to all unknown observable objects on the provided root object and begins to listening to the change events
        /// </summary>
        /// <param name="root"></param>
        /// <param name="bindingFlags"></param>
        private void SubscribeToAccessibleEvents(object root, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (root == null || IgnoredTypes.Contains(root.GetType()) || Subscriptions.ContainsKey(root)) return;
            if (Subscriptions.Count == 0)
            {
                foreach (var item in FindEventSuppliers(root, null, bindingFlags)) CreateSubscription(item);
                return;
            }

            foreach (var item in FindUnknownEventSuppliers(root)) CreateSubscription(item);
        }

        /// <summary>
        ///     Stops to listen to all events that exist in the provided object tree but cannot be accessed through the observed
        ///     root object
        /// </summary>
        /// <param name="removedObject"></param>
        /// <param name="bindingFlags"></param>
        private void UnsubscribeFromInaccessibleEvents(object removedObject, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (removedObject == null || IgnoredTypes.Contains(removedObject.GetType())) return;
            foreach (var item in FindInaccessibleObjects(removedObject)) RemoveSubscription(item);
        }

        /// <summary>
        ///     Adds a tracking subscription for the provided object. Returns the new number of references or a negative value if
        ///     the
        ///     object cannot be tracked
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private void CreateSubscription(object obj)
        {
            switch (obj)
            {
                case INotifyCollectionChanged notifyCollectionChanged:
                    SubscribeToEvent(notifyCollectionChanged);
                    break;

                case INotifyPropertyChanged notifyPropertyChanged:
                    SubscribeToEvent(notifyPropertyChanged);
                    break;

                default:
                    return;
            }

            NotifyObservationStarted(obj);
        }

        /// <summary>
        ///     Removes the tracking subscription for the provided object
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveSubscription(object obj)
        {
            if (!Subscriptions.TryGetValue(obj, out var disposable)) return;
            disposable.Dispose();
            Subscriptions.Remove(obj);
            NotifyObservationEnded(obj);
        }

        /// <summary>
        ///     Creates and adds a subscription tracker for a <see cref="INotifyPropertyChanged" /> object
        /// </summary>
        /// <param name="notifyPropertyChanged"></param>
        private void SubscribeToEvent(INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += OnChangeDetected;
            var disposable = Disposable.Create(() => notifyPropertyChanged.PropertyChanged -= OnChangeDetected);
            Subscriptions.Add(notifyPropertyChanged, disposable);
        }

        /// <summary>
        ///     Creates and adds a subscription tracker for a <see cref="INotifyCollectionChanged" /> object
        /// </summary>
        /// <param name="notifyCollectionChanged"></param>
        private void SubscribeToEvent(INotifyCollectionChanged notifyCollectionChanged)
        {
            notifyCollectionChanged.CollectionChanged += OnChangeDetected;
            var disposable = Disposable.Create(() => notifyCollectionChanged.CollectionChanged -= OnChangeDetected);
            Subscriptions.Add(notifyCollectionChanged, disposable);
        }

        /// <summary>
        ///     Recursively searches the object tree for all objects that are observable from it
        /// </summary>
        /// <param name="root"></param>
        /// <param name="doneObjects"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public HashSet<object> FindEventSuppliers(object root, HashSet<object> doneObjects = null,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            doneObjects = doneObjects ?? new HashSet<object>();
            if (doneObjects.Contains(root)) return doneObjects;
            doneObjects.Add(root);

            if (root is INotifyCollectionChanged notifyCollectionChanged)
            {
                foreach (var item in EnsureIsCollection(notifyCollectionChanged)) FindEventSuppliers(item, doneObjects);
                return doneObjects;
            }

            foreach (var propertyInfo in root.GetType().GetProperties(bindingFlags).Where(x => !IgnoredTypes.Contains(x.PropertyType)))
            {
                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(propertyInfo.PropertyType) &&
                    !typeof(INotifyCollectionChanged).IsAssignableFrom(propertyInfo.PropertyType)) continue;

                if (!(propertyInfo.GetValue(root) is object childObject)) continue;
                FindEventSuppliers(childObject, doneObjects);
            }

            return doneObjects;
        }

        /// <summary>
        ///     Gets the <see cref="HashSet{T}" /> of objects that are accessible on the provided object tree but not from the root
        ///     object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        private HashSet<object> FindInaccessibleObjects(object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            var objObservables = FindEventSuppliers(obj, null, bindingFlags);
            foreach (var knownObject in FindEventSuppliers(RootObject, null, bindingFlags)) objObservables.Remove(knownObject);
            return objObservables;
        }

        /// <summary>
        ///     Gets the <see cref="HashSet{T}" /> of objects that are currently unknown to the tracker but are accessible from the
        ///     provided object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        private HashSet<object> FindUnknownEventSuppliers(object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            var objObservables = FindEventSuppliers(obj, null, bindingFlags);
            foreach (var knownObject in Subscriptions.Keys) objObservables.Remove(knownObject);
            return objObservables;
        }

        /// <summary>
        ///     Disposes subscriptions that have become inaccessible from the root object
        /// </summary>
        /// <param name="bindingFlags"></param>
        private void DisposeInaccessibleSubscriptions(BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            var eventSuppliers = FindEventSuppliers(RootObject, null, bindingFlags);
            foreach (var key in Subscriptions.Keys.ToList().Where(x => !eventSuppliers.Contains(x))) RemoveSubscription(key);
        }

        /// <summary>
        ///     Action that is called when a previously unknown object is now observed
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void NotifyObservationStarted(object obj)
        {
            ObjectObservationStartedEvent.OnNext(obj);
        }

        /// <summary>
        ///     Action that is called when a previously known object was removed from the observation
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void NotifyObservationEnded(object obj)
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
            lock (LockObject)
            {
                var watch = Stopwatch.StartNew();
                switch (args)
                {
                    case PropertyChangedEventArgs propertyChangedEventArgs:
                        DisposeInaccessibleSubscriptions();
                        var value = sender.GetType().GetProperty(propertyChangedEventArgs.PropertyName)?.GetValue(sender);
                        if (value != null) SubscribeToAccessibleEvents(value);
                        break;

                    case NotifyCollectionChangedEventArgs collectionChangedEventArgs:
                    {
                        if (collectionChangedEventArgs.Action == NotifyCollectionChangedAction.Reset)
                        {
                            DisposeInaccessibleSubscriptions();
                            break;
                        }

                        foreach (var oldItem in (IEnumerable) collectionChangedEventArgs.OldItems ?? Enumerable.Empty<object>())
                        {
                            if (oldItem == null) continue;
                            UnsubscribeFromInaccessibleEvents(oldItem);
                        }

                        foreach (var newItem in (IEnumerable) collectionChangedEventArgs.NewItems ?? Enumerable.Empty<object>())
                            SubscribeToAccessibleEvents(newItem);

                        break;
                    }
                }
                watch.Stop();
                Debug.WriteLine($"'{this}': Watch system update - {Subscriptions.Count} objects in {watch.Elapsed}.");
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