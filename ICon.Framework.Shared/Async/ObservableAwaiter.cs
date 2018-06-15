using System;
using System.Runtime.CompilerServices;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Async
{
    /// <summary>
    /// Abstract base class for asynchronous awaiters that get notified for completion through an IObservable source and supply no value on completion
    /// </summary>
    public abstract class ObservableAwaiter : IAwaiter
    {
        /// <summary>
        /// Completion status checker delegate
        /// </summary>
        protected Func<Boolean> CheckStatus { get; set; }

        /// <summary>
        /// The subscription disposable for the continue subscription
        /// </summary>
        protected IDisposable ContinueSubscription { get; set; }

        /// <summary>
        /// Thread safe acces to the completion flag
        /// </summary>
        public Boolean IsCompleted => CheckStatus();

        protected ObservableAwaiter(Func<Boolean> checkStatus)
        {
            CheckStatus = checkStatus;
        }

        /// <summary>
        /// Get result (void), disposes the subscriptions to the observable
        /// </summary>
        public void GetResult()
        {
            ContinueSubscription?.Dispose();
        }

        /// <summary>
        /// The on completed method that handles the restore state method
        /// </summary>
        /// <param name="continuation"></param>
        public abstract void OnCompleted(Action continuation);

        /// <summary>
        /// Creates a new ObservableAwaiter for a notification source of arbitrary type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="notifier"></param>
        /// <returns></returns>
        public static IAwaiter Create<T1>(IObservable<T1> notifier, Func<Boolean> checkStatus)
        {
            return new ObservableAwaiter<T1>(notifier, checkStatus);
        }

        /// <summary>
        /// Creates an empty awaiter that ddoes nothing other than allowing to be awaited (Will instantly continue)
        /// </summary>
        /// <returns></returns>
        public static IAwaiter Empty()
        {
            return new EmptyAwaiter();
        }
    }

    /// <summary>
    /// Universal class for asynchronous awaiters that get notified for completion through an IObservable<TValue> source and supply no value on completion
    /// </summary>
    public class ObservableAwaiter<T1> : ObservableAwaiter, IAwaiter
    {
        /// <summary>
        /// Source of the continue notification
        /// </summary>
        private IObservable<T1> Notifier { get; }

        /// <summary>
        /// Creates new awaiter for the provided observable and check status method
        /// </summary>
        /// <param name="notifier"></param>
        public ObservableAwaiter(IObservable<T1> notifier, Func<Boolean> checkStatus) : base(checkStatus)
        {
            Notifier = notifier;
        }

        /// <summary>
        /// Binds the continuation invoke to the point where the observable returns true
        /// </summary>
        /// <param name="continuation"></param>
        public override void OnCompleted(Action continuation)
        {
            if (IsCompleted == true)
            {
                continuation.Invoke();
            }
            else
            {
                ContinueSubscription = Notifier.Subscribe(value => continuation.Invoke());
            }
        }
    }

    /// <summary>
    /// Empty awaiter that does nothing except allowing to use the await keyword on non awaitable IAwataible implementers
    /// </summary>
    public class EmptyAwaiter : IAwaiter
    {
        /// <summary>
        /// Is always completed
        /// </summary>
        public Boolean IsCompleted => true;

        /// <summary>
        /// Does nothing
        /// </summary>
        public void GetResult()
        {
            
        }

        /// <summary>
        /// Directly rinvokes continuation
        /// </summary>
        /// <param name="continuation"></param>
        public void OnCompleted(Action continuation)
        {
            continuation.Invoke();
        }
    }
}
