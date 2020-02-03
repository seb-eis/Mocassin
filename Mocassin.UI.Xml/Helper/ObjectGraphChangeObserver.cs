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
using Mocassin.Framework.Extensions;

namespace Mocassin.UI.Xml.Helper
{
    /// <summary>
    ///     Reflective watchdog system to watch all <see cref="INotifyCollectionChanged" /> and
    ///     <see cref="INotifyPropertyChanged" /> implementing properties of an object tree
    /// </summary>
    public class ObjectGraphChangeObserver : IDisposable
    {
        /// <summary>
        ///     Get the <see cref="ImmutableHashSet{T}"/> of property <see cref="Type"/> values that are ignored by default
        /// </summary>
        public static ImmutableHashSet<Type> DefaultIgnoredPropertyTypes { get; }

        private object LockObject { get; } = new object();

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> that relays change event arguments and sender
        /// </summary>
        private ReactiveEvent<(object Sender, EventArgs args)> ChangeEventOccuredEvent { get; }

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for added observed objects
        /// </summary>
        private ReactiveEvent<object> ObjectAddedEvent { get; }

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for removed observed objects
        /// </summary>
        private ReactiveEvent<object> ObjectRemovedEvent { get; }

        /// <summary>
        ///     Get the <see cref="Dictionary{TKey,TValue}" /> that holds the <see cref="IDisposable" /> for each observed item
        /// </summary>
        private Dictionary<object, IDisposable> Subscriptions { get; }

        /// <summary>
        ///     Get or set the observed root object
        /// </summary>
        private object RootObject { get; set; }

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that relays change event information
        /// </summary>
        public IObservable<(object Sender, EventArgs args)> ChangeEventNotifications => ChangeEventOccuredEvent.AsObservable();

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that provides notifications when an object is added to the subscription system
        /// </summary>
        public IObservable<object> ObjectAddedNotifications => ObjectAddedEvent.AsObservable();

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that provides observation when an object is removed from the subscription system
        /// </summary>
        public IObservable<object> ObjectRemovedNotifications => ObjectRemovedEvent.AsObservable();

        /// <summary>
        ///     Get a <see cref="HashSet{T}"/> of property <see cref="Type"/> that should be ignored during recursive event search
        /// </summary>
        public HashSet<Type> IgnoredTypes { get; }

        /// <summary>
        ///     Get or set a boolean flag if objects not implementing <see cref="INotifyCollectionChanged"/> or <see cref="INotifyPropertyChanged"/> should be included
        /// </summary>
        public bool IncludeSilentObjects { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if <see cref="INotifyCollectionChanged"/> implementors should also be included as <see cref="INotifyPropertyChanged"/> if possible
        /// </summary>
        public bool IncludeCollectionProperties { get; set; }

        /// <summary>
        ///     Get a boolean flag indicating if the system is observing an object
        /// </summary>
        public bool IsObserving => Subscriptions.Count > 0;

        /// <inheritdoc />
        static ObjectGraphChangeObserver()
        {
            DefaultIgnoredPropertyTypes = new HashSet<Type>
            {
                typeof(string)
            }.ToImmutableHashSet();
        }

        /// <summary>
        ///     Creates a new <see cref="ObjectGraphChangeObserver"/> with an optional set of ignored property <see cref="Type"/> values
        /// </summary>
        /// <param name="ignoredTypes"></param>
        public ObjectGraphChangeObserver(IEnumerable<Type> ignoredTypes = null)
        {
            ChangeEventOccuredEvent = new ReactiveEvent<(object Sender, EventArgs args)>();
            ObjectRemovedEvent = new ReactiveEvent<object>();
            ObjectAddedEvent = new ReactiveEvent<object>();
            Subscriptions = new Dictionary<object, IDisposable>();
            IgnoredTypes = new HashSet<Type>((ignoredTypes ?? Enumerable.Empty<Type>()).Concat(DefaultIgnoredPropertyTypes));
        }

        /// <summary>
        ///     Sets the provided object as the new observation root and attaches itself the the change notification events if requested
        /// </summary>
        /// <param name="rootObject"></param>
        /// <param name="startObservation"></param>
        public void SetObservationRoot(object rootObject, bool startObservation = true)
        {
            if (rootObject == null) throw new ArgumentNullException(nameof(rootObject));
            if (ReferenceEquals(RootObject, rootObject)) return;
            if (!TypeIsSearchIncluded(rootObject.GetType())) throw new InvalidOperationException("The root object type is not set to searchable.");
            if (RootObject != null) StopObservation();
            RootObject = rootObject;
            if (startObservation) StartObservation();
        }

        /// <summary>
        ///     Stops observation of the currently set root object
        /// </summary>
        public void StopObservation()
        {
            if (RootObject == null) return;
            foreach (var subscription in Subscriptions.Values) subscription?.Dispose();
            Subscriptions.Clear();
        }

        /// <summary>
        ///     Starts the observation of the currently set root object
        /// </summary>
        public void StartObservation()
        {
            if (IsObserving || RootObject == null) return;
            SubscribeToAccessibleEvents(RootObject);
        }

        /// <summary>
        ///     Attach to all unknown observable objects on the provided root object and begins to listening to the change events
        /// </summary>
        /// <param name="root"></param>
        /// <param name="bindingFlags"></param>
        private void SubscribeToAccessibleEvents(object root, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (root == null || !TypeIsSearchIncluded(root.GetType()) || Subscriptions.ContainsKey(root)) return;
            if (Subscriptions.Count == 0)
            {
                foreach (var item in FindIncludedObjectsRecursive(root, bindingFlags)) CreateSubscription(item);
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
            if (removedObject == null || !TypeIsSearchIncluded(removedObject.GetType())) return;
            foreach (var item in FindInaccessibleObjects(removedObject, bindingFlags)) RemoveSubscription(item);
        }

        /// <summary>
        ///     Creates a subscription for the change events of the provided <see cref="object"/>. Has no effect if the object does not provide change events
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
            IDisposable disposable;
            notifyCollectionChanged.CollectionChanged += OnChangeDetected;
            if (IncludeCollectionProperties && notifyCollectionChanged is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += OnChangeDetected;
                disposable = Disposable.Create(() =>
                {
                    notifyCollectionChanged.CollectionChanged -= OnChangeDetected;
                    notifyPropertyChanged.PropertyChanged -= OnChangeDetected;
                });
            }
            else
            {
                disposable = Disposable.Create(() => notifyCollectionChanged.CollectionChanged -= OnChangeDetected);   
            }
            Subscriptions.Add(notifyCollectionChanged, disposable);
        }

        /// <summary>
        ///     Recursively searches the object tree for all included objects while automatically handling cyclic reference
        /// </summary>
        /// <param name="root"></param>
        /// <param name="doneObjects"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public HashSet<object> FindIncludedObjectsRecursive(object root, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, HashSet<object> doneObjects = null)
        {
            doneObjects ??= new HashSet<object>();
            if (doneObjects.Contains(root)) return doneObjects;
            doneObjects.Add(root);

            if (root is INotifyCollectionChanged notifyCollectionChanged)
            {
                foreach (var item in AsCollectionInterface(notifyCollectionChanged)) FindIncludedObjectsRecursive(item, bindingFlags, doneObjects);
                return doneObjects;
            }

            foreach (var propertyInfo in root.GetType().GetProperties(bindingFlags).Where(x => TypeIsSearchIncluded(x.PropertyType)))
            {
                if (!(propertyInfo.GetValue(root) is { } childObject)) continue;
                FindIncludedObjectsRecursive(childObject, bindingFlags, doneObjects);
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
            var objects = FindIncludedObjectsRecursive(obj, bindingFlags);
            foreach (var knownObject in FindIncludedObjectsRecursive(RootObject, bindingFlags)) objects.Remove(knownObject);
            return objects;
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
            var objects = FindIncludedObjectsRecursive(obj, bindingFlags);
            foreach (var knownObject in Subscriptions.Keys) objects.Remove(knownObject);
            return objects;
        }

        /// <summary>
        ///     Disposes subscriptions that have become inaccessible from the root object
        /// </summary>
        /// <param name="bindingFlags"></param>
        private void DisposeInaccessibleSubscriptions(BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            var objects = FindIncludedObjectsRecursive(RootObject, bindingFlags);
            foreach (var key in Subscriptions.Keys.ToList(Subscriptions.Count).Where(x => !objects.Contains(x))) RemoveSubscription(key);
        }

        /// <summary>
        ///     Checks if the provided <see cref="Type"/> is included in the search routine
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool TypeIsSearchIncluded(Type type)
        {
            if (type.IsValueType || IgnoredTypes.Contains(type)) return false;
            return IncludeSilentObjects || typeof(INotifyCollectionChanged).IsAssignableFrom(type) || typeof(INotifyPropertyChanged).IsAssignableFrom(type);
        }

        /// <summary>
        ///     Action that is called when a previously unknown object is now observed
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void NotifyObservationStarted(object obj)
        {
            ObjectAddedEvent.OnNext(obj);
        }

        /// <summary>
        ///     Action that is called when a previously known object was removed from the observation
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void NotifyObservationEnded(object obj)
        {
            ObjectRemovedEvent.OnNext(obj);
        }

        /// <summary>
        ///     Converts the provided <see cref="INotifyCollectionChanged" /> to a <see cref="ICollection" /> and throws if the
        ///     cast cannot be done
        /// </summary>
        /// <param name="notifyCollectionChanged"></param>
        /// <returns></returns>
        protected ICollection AsCollectionInterface(INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged as ICollection
                   ?? throw new InvalidOperationException("INotifyCollectionChanged requires ICollection to be implemented.");
        }

        /// <summary>
        ///     Action that is attached to all change events of the object tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnChangeDetected(object sender, EventArgs args)
        {
            lock (LockObject)
            {
                DebugLogEvent();
                switch (args)
                {
                    case PropertyChangedEventArgs propertyChangedEventArgs:
                        var propertyInfo = sender.GetType().GetProperty(propertyChangedEventArgs.PropertyName);
                        if (propertyInfo == null || !TypeIsSearchIncluded(propertyInfo.PropertyType)) break;
                        DisposeInaccessibleSubscriptions();
                        if (propertyInfo.GetValue(sender) is { } value) SubscribeToAccessibleEvents(value);
                        break;

                    case NotifyCollectionChangedEventArgs collectionChangedEventArgs:
                    {
                        if (collectionChangedEventArgs.Action == NotifyCollectionChangedAction.Reset)
                        {
                            DisposeInaccessibleSubscriptions();
                            break;
                        }

                        foreach (var oldItem in (IEnumerable) collectionChangedEventArgs.OldItems ?? Enumerable.Empty<object>())
                            UnsubscribeFromInaccessibleEvents(oldItem);

                        foreach (var newItem in (IEnumerable) collectionChangedEventArgs.NewItems ?? Enumerable.Empty<object>())
                            SubscribeToAccessibleEvents(newItem);

                        break;
                    }
                }
                DebugLogEvent(false);
            }

            ChangeEventOccuredEvent.OnNext((sender, args));
        }

        /// <summary>
        ///     Notifies event subscribers that the events completed
        /// </summary>
        public void NotifyEventsCompleted()
        {
            ChangeEventOccuredEvent.OnCompleted();
            ObjectAddedEvent.OnCompleted();
            ObjectRemovedEvent.OnCompleted();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            StopObservation();
            NotifyEventsCompleted();
        }

        /// <summary>
        ///     Conditional method that logs the time on DEBUG
        /// </summary>
        /// <param name="isStart"></param>
        [Conditional("DEBUG")]
        private void DebugLogEvent(bool isStart = true)
        {
            Debug.WriteLine($"'{this}': {DateTime.Now:O} : Observer event {(isStart ? "begin" : "done ")} - {Subscriptions.Count} watched objects.");
        }
    }
}