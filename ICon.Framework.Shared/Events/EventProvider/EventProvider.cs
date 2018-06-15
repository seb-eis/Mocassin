using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ICon.Framework.Events
{
    /// <summary>
    /// Event provider that provides the basis for generic push based asynchronous events through the IObserver interface
    /// </summary>
    /// <typeparam name="TSubject"></typeparam>
    public class EventProvider<TSubject>
    {
        /// <summary>
        /// The subject of the event to enable subscriptions and distribution of push notifications
        /// </summary>
        private Subject<TSubject> EventSubject { get; set; }

        /// <summary>
        /// Creates a new event provider
        /// </summary>
        public EventProvider()
        {
            EventSubject = new Subject<TSubject>();
        }

        /// <summary>
        /// Hides the identity of the observable sequence and provides an interface for event subscriptions
        /// </summary>
        /// <returns></returns>
        public IObservable<TSubject> AsObservable()
        {
            return EventSubject.AsObservable();
        }

        /// <summary>
        /// Distributes a new event value through a new task
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<Unit> DistributeAsync(TSubject value)
        {
            return Task.Run(() => { EventSubject.OnNext(value); return Unit.Default; });
        }

        /// <summary>
        /// Distributes a default argument event through a new task
        /// </summary>
        /// <returns></returns>
        public Task<Unit> DistributeAsync()
        {
            return DistributeAsync(default);
        }

        /// <summary>
        /// Distributes the event synchronous
        /// </summary>
        /// <param name="value"></param>
        public void Distribute(TSubject value)
        {
            EventSubject.OnNext(value);
        }

        /// <summary>
        /// Distributes the event synchronous with a subject default argument
        /// </summary>
        public void Distribute()
        {
            EventSubject.OnNext(default);
        }
    }
}
