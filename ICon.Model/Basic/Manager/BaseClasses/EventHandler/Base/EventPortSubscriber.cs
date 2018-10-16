using System;
using System.Collections.Generic;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Represents a bundle event port subscriber that groups event subscriptions together for a single event port
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class EventPortSubscriber<T1> where T1 : IModelEventPort
    {
        /// <summary>
        ///     The list of subscription delegates
        /// </summary>
        protected List<Func<T1, IDisposable>> SubscriptionDelegates { get; set; }

        /// <summary>
        ///     Creates new bundle subscription from delegates. Throws if one of the delegates if of wrong type
        /// </summary>
        /// <param name="delegates"></param>
        public EventPortSubscriber(IEnumerable<Delegate> delegates)
        {
            SubscriptionDelegates = new List<Func<T1, IDisposable>>();
            foreach (var item in delegates)
                SubscriptionDelegates.Add((Func<T1, IDisposable>) item);
        }

        /// <summary>
        ///     Performs the event subscriptions and returns a disposable collection to unsubscribe all subscriptions at once
        /// </summary>
        /// <param name="eventPort"></param>
        /// <returns></returns>
        [EventPortConnector]
        public IDisposable InvokeSubscription(T1 eventPort)
        {
            var unsubscription = new DisposableCollection();
            foreach (var item in SubscriptionDelegates)
                unsubscription.Add(item(eventPort));

            return unsubscription;
        }
    }
}