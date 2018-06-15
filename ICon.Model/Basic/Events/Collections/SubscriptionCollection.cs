using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Disposable subscription collection that stores a set of unsubscriber disposbales belonging to a single event source
    /// </summary>
    public class SubscriptionCollection : DisposableCollection, IEquatable<SubscriptionCollection>
    {
        /// <summary>
        /// The subscription source object or event port
        /// </summary>
        public Type EventPortType { get; private set; }

        /// <summary>
        /// Creates new subscription collection for the specified port type
        /// </summary>
        /// <param name="portType"></param>
        public SubscriptionCollection(Type portType) : base()
        {
            EventPortType = portType ?? throw new ArgumentNullException(nameof(portType));
        }

        /// <summary>
        /// Creates new subscription collection for the specififed event port type with a initial list of subscriptions
        /// </summary>
        /// <param name="portType"></param>
        /// <param name="subscriptions"></param>
        public SubscriptionCollection(Type portType, List<IDisposable> subscriptions) : base(subscriptions)
        {
            EventPortType = portType ?? throw new ArgumentNullException(nameof(portType));
        }

        /// <summary>
        /// Disposes the collection only if the specififed port type the internal stored type, returns true if dispose was performed
        /// </summary>
        /// <param name="sourceType"></param>
        public Boolean DisposeIfSourceMatch(Type sourceType)
        {
            if (sourceType == EventPortType)
            {
                Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if both containers are for the same event source type
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(SubscriptionCollection other)
        {
            return EventPortType == other.EventPortType;
        }
    }
}
