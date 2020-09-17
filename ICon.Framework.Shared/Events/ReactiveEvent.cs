using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Mocassin.Framework.Events
{
    /// <summary>
    ///     Provides a basic mechanism for an event system based on the <see cref="IObservable{T}" /> interface
    /// </summary>
    /// <typeparam name="TSubject"></typeparam>
    public class ReactiveEvent<TSubject>
    {
        /// <summary>
        ///     The subject of the event to enable subscriptions and distribution of push notifications
        /// </summary>
        private Subject<TSubject> Subject { get; }

        /// <summary>
        ///     Get a boolean flag if the event has any subscribers
        /// </summary>
        public bool HasObservers => Subject.HasObservers;

        /// <summary>
        ///     Creates a new event provider
        /// </summary>
        public ReactiveEvent()
        {
            Subject = new Subject<TSubject>();
        }

        /// <summary>
        ///     Hides the identity of the observable sequence and provides an interface for event subscriptions
        /// </summary>
        /// <returns></returns>
        public IObservable<TSubject> AsObservable() => Subject.AsObservable();

        /// <summary>
        ///     Distributes a new event value through a new task
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<Unit> OnNextAsync(TSubject value)
        {
            return Task.Run(() =>
            {
                Subject.OnNext(value);
                return Unit.Default;
            });
        }

        /// <summary>
        ///     Distributes a default argument event through a new task
        /// </summary>
        /// <returns></returns>
        public Task<Unit> OnNextAsync() => OnNextAsync(default);

        /// <summary>
        ///     Distributes the event synchronous
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(TSubject value)
        {
            Subject.OnNext(value);
        }

        /// <summary>
        ///     Distributes the event synchronous with a subject default argument
        /// </summary>
        public void OnNext()
        {
            Subject.OnNext(default);
        }

        /// <summary>
        ///     Distributes an error synchronously to all subscribers
        /// </summary>
        /// <param name="exception"></param>
        public void OnError(Exception exception)
        {
            Subject.OnError(exception);
        }

        /// <summary>
        ///     Distributes completion to all subscribers synchronously
        /// </summary>
        public void OnCompleted()
        {
            Subject.OnCompleted();
        }
    }
}