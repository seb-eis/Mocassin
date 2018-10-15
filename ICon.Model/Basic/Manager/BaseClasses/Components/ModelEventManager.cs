using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Threading.Tasks;

using ICon.Framework.Events;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base calss for implementations of model event managers that provide data change push notifications
    /// </summary>
    internal abstract class ModelEventManager : IModelEventPort
    {
        /// <summary>
        /// Event provider for the subject of manager rests
        /// </summary>
        internal ReactiveEvent<Unit> OnManagerResets { get; }

        /// <summary>
        /// Event provider for the subject of expired extended data cache
        /// </summary>
        internal ReactiveEvent<Unit> OnExtendedDataExpiration { get; }

        /// <summary>
        /// Event provider for the subject of manager disconnect requests
        /// </summary>
        internal ReactiveEvent<Unit> OnManagerDisconnectRequests { get; }

        /// <summary>
        /// Event provider for the subject of new model objects in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnNewModelObjects { get; }

        /// <summary>
        /// Event provider for the subject of removed model objects in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnRemovedModelObjects { get; }

        /// <summary>
        /// Event provider for the subject of changed model objects in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnChangedModelObjects { get; }

        /// <summary>
        /// Event provider for the subject of changed model parameters in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnChangedModelParameters { get; }

        /// <summary>
        /// Event provider for the subject of changed model object indexing in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnChangedModelIndexing { get; }

        /// <summary>
        /// Push notifier that informs about a complete clear of the manager
        /// </summary>
        public IObservable<Unit> WhenManagerReset => OnManagerResets.AsObservable();

        /// <summary>
        /// Push notifier that informs about expired cached data in the manager
        /// </summary>
        public IObservable<Unit> WhenExtendedDataExpired => OnExtendedDataExpiration.AsObservable();

        /// <summary>
        /// Push notifier that informs about a disconnection request to the manager (Subscribers should do nothing else that dispose their subscriptions to the event port)
        /// </summary>
        public IObservable<Unit> WhenManagerDisconnects => OnManagerDisconnectRequests.AsObservable();

        /// <summary>
        /// Push notifier that informs about model object additions to the manager
        /// </summary>
        public IObservable<EventArgs> WhenModelObjectAdded => OnNewModelObjects.AsObservable();

        /// <summary>
        /// Push notifier that informs about model object removals from the manager
        /// </summary>
        public IObservable<EventArgs> WhenModelObjectRemoved => OnRemovedModelObjects.AsObservable();

        /// <summary>
        /// Push notifier that informs about model object changes in the manager
        /// </summary>
        public IObservable<EventArgs> WhenModelObjectChanged => OnChangedModelObjects.AsObservable();

        /// <summary>
        /// Push notifier that informs about model parameter changes in the manager
        /// </summary>
        public IObservable<EventArgs> WhenModelParameterChanged => OnChangedModelParameters.AsObservable();

        /// <summary>
        /// Push notifier that informs about model object reindexing in the manager
        /// </summary>
        public IObservable<EventArgs> WhenModelIndexingChanged => OnChangedModelIndexing.AsObservable();

        /// <summary>
        /// Creates new model manager
        /// </summary>
        protected ModelEventManager()
        {
            OnManagerResets = new ReactiveEvent<Unit>();
            OnManagerDisconnectRequests = new ReactiveEvent<Unit>();
            OnExtendedDataExpiration = new ReactiveEvent<Unit>();
            OnNewModelObjects = new ReactiveEvent<EventArgs>();
            OnRemovedModelObjects = new ReactiveEvent<EventArgs>();
            OnChangedModelObjects = new ReactiveEvent<EventArgs>();
            OnChangedModelParameters = new ReactiveEvent<EventArgs>();
            OnChangedModelIndexing = new ReactiveEvent<EventArgs>();
        }
    }
}
