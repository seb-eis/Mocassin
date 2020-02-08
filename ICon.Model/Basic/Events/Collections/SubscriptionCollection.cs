using System;
using System.Collections.Generic;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Disposable subscription collection that stores a set of un-subscriber disposables belonging to a single event
    ///     source
    /// </summary>
    public class SubscriptionCollection : DisposableCollection, IEquatable<SubscriptionCollection>
    {
        /// <summary>
        ///     The subscription source object or event port
        /// </summary>
        public Type EventPortType { get; }

        /// <summary>
        ///     Creates new subscription collection for the specified port type
        /// </summary>
        /// <param name="portType"></param>
        public SubscriptionCollection(Type portType)
        {
            EventPortType = portType ?? throw new ArgumentNullException(nameof(portType));
        }

        /// <summary>
        ///     Creates new subscription collection for the specified event port type with a initial list of subscriptions
        /// </summary>
        /// <param name="portType"></param>
        /// <param name="subscriptions"></param>
        public SubscriptionCollection(Type portType, ICollection<IDisposable> subscriptions)
            : base(subscriptions)
        {
            EventPortType = portType ?? throw new ArgumentNullException(nameof(portType));
        }

        /// <inheritdoc />
        public bool Equals(SubscriptionCollection other)
        {
            return other != null && EventPortType == other.EventPortType;
        }

        /// <summary>
        ///     Disposes the collection only if the specified port type the internal stored type, returns true if dispose was
        ///     performed
        /// </summary>
        /// <param name="sourceType"></param>
        public bool DisposeIfSourceMatch(Type sourceType)
        {
            if (sourceType != EventPortType)
                return false;

            Dispose();
            return true;
        }
    }
}