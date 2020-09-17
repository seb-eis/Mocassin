using System;
using System.Reactive;
using Mocassin.Framework.Events;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for implementations of model event managers that provide data change push notifications
    /// </summary>
    internal abstract class ModelEventManager : IModelEventPort
    {
        /// <summary>
        ///     Event provider for the subject of manager rests
        /// </summary>
        internal ReactiveEvent<Unit> OnManagerResets { get; }

        /// <summary>
        ///     Event provider for the subject of expired extended data cache
        /// </summary>
        internal ReactiveEvent<Unit> OnExtendedDataExpiration { get; }

        /// <summary>
        ///     Event provider for the subject of manager disconnect requests
        /// </summary>
        internal ReactiveEvent<Unit> OnManagerDisconnectRequests { get; }

        /// <summary>
        ///     Event provider for the subject of new model objects in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnNewModelObjects { get; }

        /// <summary>
        ///     Event provider for the subject of removed model objects in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnRemovedModelObjects { get; }

        /// <summary>
        ///     Event provider for the subject of changed model objects in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnChangedModelObjects { get; }

        /// <summary>
        ///     Event provider for the subject of changed model parameters in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnChangedModelParameters { get; }

        /// <summary>
        ///     Event provider for the subject of changed model object indexing in the manager
        /// </summary>
        internal ReactiveEvent<EventArgs> OnChangedModelIndexing { get; }

        /// <inheritdoc />
        public IObservable<Unit> WhenManagerReset => OnManagerResets.AsObservable();

        /// <inheritdoc />
        public IObservable<Unit> WhenExtendedDataExpired => OnExtendedDataExpiration.AsObservable();

        /// <inheritdoc />
        public IObservable<Unit> WhenManagerDisconnects => OnManagerDisconnectRequests.AsObservable();

        /// <inheritdoc />
        public IObservable<EventArgs> WhenModelObjectAdded => OnNewModelObjects.AsObservable();

        /// <inheritdoc />
        public IObservable<EventArgs> WhenModelObjectRemoved => OnRemovedModelObjects.AsObservable();

        /// <inheritdoc />
        public IObservable<EventArgs> WhenModelObjectChanged => OnChangedModelObjects.AsObservable();

        /// <inheritdoc />
        public IObservable<EventArgs> WhenModelParameterChanged => OnChangedModelParameters.AsObservable();

        /// <inheritdoc />
        public IObservable<EventArgs> WhenModelIndexingChanged => OnChangedModelIndexing.AsObservable();

        /// <summary>
        ///     Creates new model manager
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