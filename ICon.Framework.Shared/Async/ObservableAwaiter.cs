using System;

namespace Mocassin.Framework.Async
{
    /// <summary>
    ///     Abstract base class for asynchronous awaiter that get notified for completion through an IObservable source and
    ///     supply no value on completion
    /// </summary>
    public abstract class ObservableAwaiter : IAwaiter
    {
        /// <summary>
        ///     Completion status checker delegate
        /// </summary>
        protected Func<bool> CheckStatus { get; set; }

        /// <summary>
        ///     The subscription disposable for the continue subscription
        /// </summary>
        protected IDisposable ContinueSubscription { get; set; }

        /// <inheritdoc />
        public bool IsCompleted => CheckStatus();

        /// <summary>
        ///     Creates a new <see cref="ObservableAwaiter" /> using the provided <see cref="Func{TResult}" /> completion check
        /// </summary>
        /// <param name="checkStatus"></param>
        protected ObservableAwaiter(Func<bool> checkStatus)
        {
            CheckStatus = checkStatus;
        }

        /// <inheritdoc />
        public void GetResult()
        {
            ContinueSubscription?.Dispose();
        }

        /// <inheritdoc />
        public abstract void OnCompleted(Action continuation);

        /// <summary>
        ///     Creates a new ObservableAwaiter for a notification source of arbitrary type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="notifier"></param>
        /// <param name="checkStatus"></param>
        /// <returns></returns>
        public static IAwaiter Create<T1>(IObservable<T1> notifier, Func<bool> checkStatus)
        {
            return new ObservableAwaiter<T1>(notifier, checkStatus);
        }

        /// <summary>
        ///     Creates an empty awaiter that ddoes nothing other than allowing to be awaited (Will instantly continue)
        /// </summary>
        /// <returns></returns>
        public static IAwaiter Empty()
        {
            return new EmptyAwaiter();
        }
    }

    /// <summary>
    ///     Universal class for asynchronous awaiter that get notified for completion through an IObservable
    ///     source and supply no value on completion
    /// </summary>
    public class ObservableAwaiter<T1> : ObservableAwaiter
    {
        /// <summary>
        ///     Source of the continue notification
        /// </summary>
        private IObservable<T1> Notifier { get; }

        /// <summary>
        ///     Creates new awaiter for the provided observable and check status method
        /// </summary>
        /// <param name="notifier"></param>
        /// <param name="checkStatus"></param>
        public ObservableAwaiter(IObservable<T1> notifier, Func<bool> checkStatus)
            : base(checkStatus)
        {
            Notifier = notifier;
        }

        /// <inheritdoc />
        public override void OnCompleted(Action continuation)
        {
            if (IsCompleted)
                continuation.Invoke();
            else
                ContinueSubscription = Notifier.Subscribe(value => continuation.Invoke());
        }
    }

    /// <summary>
    ///     Empty awaiter that does nothing except allowing to use the await keyword on non awaitable awaitable implementers
    /// </summary>
    public class EmptyAwaiter : IAwaiter
    {
        /// <inheritdoc />
        public bool IsCompleted => true;

        /// <inheritdoc />
        public void GetResult()
        {
        }

        /// <inheritdoc />
        public void OnCompleted(Action continuation)
        {
            continuation.Invoke();
        }
    }
}