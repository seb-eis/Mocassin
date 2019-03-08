using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Mocassin.Framework.Async;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    /// Synchronous reactive push messaging system to distribute messages through subscriptions to IObservables
    /// </summary>
    public class MessageSystem : IPushMessageSystem
    {
        /// <summary>
        /// Subject for info messages
        /// </summary>
        private Subject<InfoMessage> InfoMessageSubject { get; }

        /// <summary>
        /// Subject for error messages
        /// </summary>
        private Subject<ErrorMessage> ErrorMessageSubject { get; }

        /// <summary>
        /// Subject for warning messages
        /// </summary>
        private Subject<WarningMessage> WarningMessageSubject { get; }

        /// <summary>
        /// Subject for arbitrary push messages
        /// </summary>
        private Subject<PushMessage> PushMessageSubject { get; }

        /// <inheritdoc />
        public IObservable<InfoMessage> InfoMessageNotification => InfoMessageSubject.AsObservable();

        /// <inheritdoc />
        public IObservable<WarningMessage> WarningMessageNotification => WarningMessageSubject.AsObservable();

        /// <inheritdoc />
        public IObservable<ErrorMessage> ErrorMessageNotification => ErrorMessageSubject.AsObservable();

        /// <inheritdoc />
        public IObservable<PushMessage> AnyMessageNotification => PushMessageSubject.AsObservable();

        /// <inheritdoc />
        public bool ConsoleSubscriptionActive { get; private set; }

        /// <summary>
        /// Stores the console subscription disposable
        /// </summary>
        private IDisposable ConsoleSubscription { get; set; }

        /// <summary>
        /// Counts how many messages are currently send and await completion
        /// </summary>
        public int AwaitCompletion { get; protected set; }

        /// <summary>
        /// Creates new sync message system for push notifications
        /// </summary>
        public MessageSystem()
        {
            InfoMessageSubject = new Subject<InfoMessage>();
            WarningMessageSubject = new Subject<WarningMessage>();
            ErrorMessageSubject = new Subject<ErrorMessage>();
            PushMessageSubject = new Subject<PushMessage>();
        }

        /// <inheritdoc />
        public void SendMessage(InfoMessage message)
        {
            if (message == null)
                return;

            PrepareSending();
            OnInfoMessageReceived(message);
            OnMessageReceived(message);
            FinishSending();
        }

        /// <inheritdoc />
        public void SendMessage(ErrorMessage message)
        {
            if (message == null)
                return;

            PrepareSending();
            OnErrorMessageReceived(message);
            OnMessageReceived(message);
            FinishSending();
        }

        /// <inheritdoc />
        public void SendMessage(WarningMessage message)
        {
            if (message == null)
                return;

            PrepareSending();
            OnWarningMessageReceived(message);
            OnMessageReceived(message);
            FinishSending();
        }

        /// <summary>
        /// Takes arbitrary operation message and distributes the message to all registered observers
        /// </summary>
        /// <param name="message"></param>
        protected void SendMessage(PushMessage message)
        {
            if (message != null)
                OnMessageReceived(message);
        }

        /// <summary>
        /// Invokes the subscription handlers on all info message subscribers
        /// </summary>
        /// <param name="message"></param>
        protected void OnInfoMessageReceived(InfoMessage message)
        {
            InfoMessageSubject.OnNext(message);
        }

        /// <summary>
        /// Invokes the subscription handlers on all warning message subscribers
        /// </summary>
        /// <param name="message"></param>
        protected void OnWarningMessageReceived(WarningMessage message)
        {
            WarningMessageSubject.OnNext(message);
        }

        /// <summary>
        /// Invokes the subscription handlers on all error message subscribers
        /// </summary>
        /// <param name="message"></param>
        protected void OnErrorMessageReceived(ErrorMessage message)
        {
            ErrorMessageSubject.OnNext(message);
        }

        /// <summary>
        /// Invokes the subscription handlers on all arbitrary message subscribers
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected void OnMessageReceived(PushMessage message)
        {
            PushMessageSubject.OnNext(message);
        }

        /// <inheritdoc />
        public void DumpMessageToConsole(PushMessage message)
        {
            Console.WriteLine(MessageToString(message));
        }

        /// <inheritdoc />
        public void SubscribeConsoleDisplay()
        {
            if (ConsoleSubscriptionActive == false)
                ConsoleSubscription = AnyMessageNotification.Subscribe(DumpMessageToConsole);

            ConsoleSubscriptionActive = true;
        }

        /// <inheritdoc />
        public void UnsubscribeConsoleDisplay()
        {
            if (ConsoleSubscriptionActive)
            {
                ConsoleSubscription?.Dispose();
                ConsoleSubscription = null;
            }
            ConsoleSubscriptionActive = false;
        }

        /// <summary>
        /// Creates a display string from a message event that contains the sender information
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string MessageToString(PushMessage message)
        {
            return $"Message From: {message.Sender}{Environment.NewLine}{Environment.NewLine}{message}";
        }

        /// <summary>
        /// Locks the sender lock and counts the number of uncompleted senders up
        /// </summary>
        protected void PrepareSending()
        {
            AwaitCompletion++;
        }

        /// <summary>
        /// Locks the sender object and counts the number of unfinished messages down and notifies awaiters of completion
        /// </summary>
        protected void FinishSending()
        {
            AwaitCompletion--;
        }

        /// <summary>
        /// Gets an awaiter for the message system that frees the next time the system has finished sending all messages
        /// </summary>
        /// <returns></returns>
        public IAwaiter GetAwaiter()
        {
            return ObservableAwaiter.Empty();
        }
    }
}
