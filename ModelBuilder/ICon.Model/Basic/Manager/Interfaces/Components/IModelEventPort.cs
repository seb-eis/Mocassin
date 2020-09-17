using System;
using System.Reactive;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     General base interface for all model notification port interfaces that provide at least a notification when a
    ///     manager is cleared
    /// </summary>
    public interface IModelEventPort
    {
        /// <summary>
        ///     Push notifier that informs about a complete clear of the manager
        /// </summary>
        IObservable<Unit> WhenManagerReset { get; }

        /// <summary>
        ///     Push notifier that informs about expired cached data in the manager
        /// </summary>
        IObservable<Unit> WhenExtendedDataExpired { get; }

        /// <summary>
        ///     Push notifier that informs about a disconnection request to the manager (Subscribers should do nothing else that
        ///     dispose their subscriptions to the event port)
        /// </summary>
        IObservable<Unit> WhenManagerDisconnects { get; }

        /// <summary>
        ///     Push notifier about newly added model objects to the manager
        /// </summary>
        IObservable<EventArgs> WhenModelObjectAdded { get; }

        /// <summary>
        ///     Push notifier about model object removals in the manager
        /// </summary>
        IObservable<EventArgs> WhenModelObjectRemoved { get; }

        /// <summary>
        ///     Push notifier about model object changes in the manager
        /// </summary>
        IObservable<EventArgs> WhenModelObjectChanged { get; }

        /// <summary>
        ///     Push notifier about model parameter changes in the manager
        /// </summary>
        IObservable<EventArgs> WhenModelParameterChanged { get; }

        /// <summary>
        ///     Push notifier about model object reindexing in the manager
        /// </summary>
        IObservable<EventArgs> WhenModelIndexingChanged { get; }
    }
}