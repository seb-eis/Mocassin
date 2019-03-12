using System;
using Mocassin.Framework.Events;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    ///     Synchronous reactive push messaging system to distribute messages through subscriptions to IObservables
    /// </summary>
    public class MessageSystem : IPushMessageSystem
    {
        /// <summary>
        ///     The <see cref="ReactiveEvent{TSubject}" /> for <see cref="InfoMessage" /> notifications
        /// </summary>
        private ReactiveEvent<InfoMessage> SendInfoMessageEvent { get; }

        /// <summary>
        ///     The <see cref="ReactiveEvent{TSubject}" /> for <see cref="ErrorMessage" /> notifications
        /// </summary>
        private ReactiveEvent<ErrorMessage> SendErrorMessageEvent { get; }

        /// <summary>
        ///     The <see cref="ReactiveEvent{TSubject}" /> for <see cref="WarningMessage" /> notifications
        /// </summary>
        private ReactiveEvent<WarningMessage> SendWarningMessageEvent { get; }

        /// <summary>
        ///     The <see cref="ReactiveEvent{TSubject}" /> for <see cref="PushMessage" /> notifications
        /// </summary>
        private ReactiveEvent<PushMessage> SendPushMessageEvent { get; }

        /// <inheritdoc />
        public IObservable<InfoMessage> InfoMessageNotification => SendInfoMessageEvent.AsObservable();

        /// <inheritdoc />
        public IObservable<WarningMessage> WarningMessageNotification => SendWarningMessageEvent.AsObservable();

        /// <inheritdoc />
        public IObservable<ErrorMessage> ErrorMessageNotification => SendErrorMessageEvent.AsObservable();

        /// <inheritdoc />
        public IObservable<PushMessage> AnyMessageNotification => SendPushMessageEvent.AsObservable();

        /// <inheritdoc />
        public bool ConsoleSubscriptionActive { get; private set; }

        /// <summary>
        ///     Stores the console subscription disposable
        /// </summary>
        private IDisposable ConsoleSubscription { get; set; }

        /// <summary>
        ///     Counts how many messages are currently send and await completion
        /// </summary>
        public int AwaitCompletion { get; protected set; }

        /// <summary>
        ///     Creates new sync message system for push notifications
        /// </summary>
        public MessageSystem()
        {
            SendErrorMessageEvent = new ReactiveEvent<ErrorMessage>();
            SendInfoMessageEvent = new ReactiveEvent<InfoMessage>();
            SendPushMessageEvent = new ReactiveEvent<PushMessage>();
            SendWarningMessageEvent = new ReactiveEvent<WarningMessage>();
        }

        /// <inheritdoc />
        public void SendMessage(InfoMessage message)
        {
            if (message == null) return;

            PrepareSending();
            OnInfoMessageReceived(message);
            OnMessageReceived(message);
            FinishSending();
        }

        /// <inheritdoc />
        public void SendMessage(ErrorMessage message)
        {
            if (message == null) return;

            PrepareSending();
            OnErrorMessageReceived(message);
            OnMessageReceived(message);
            FinishSending();
        }

        /// <inheritdoc />
        public void SendMessage(WarningMessage message)
        {
            if (message == null) return;

            PrepareSending();
            OnWarningMessageReceived(message);
            OnMessageReceived(message);
            FinishSending();
        }

        /// <summary>
        ///     Takes arbitrary operation message and distributes the message to all registered observers
        /// </summary>
        /// <param name="message"></param>
        protected void SendMessage(PushMessage message)
        {
            if (message != null) OnMessageReceived(message);
        }

        /// <summary>
        ///     Invokes the subscription handlers on all info message subscribers
        /// </summary>
        /// <param name="message"></param>
        protected void OnInfoMessageReceived(InfoMessage message)
        {
            SendInfoMessageEvent.OnNext(message);
        }

        /// <summary>
        ///     Invokes the subscription handlers on all warning message subscribers
        /// </summary>
        /// <param name="message"></param>
        protected void OnWarningMessageReceived(WarningMessage message)
        {
            SendWarningMessageEvent.OnNext(message);
        }

        /// <summary>
        ///     Invokes the subscription handlers on all error message subscribers
        /// </summary>
        /// <param name="message"></param>
        protected void OnErrorMessageReceived(ErrorMessage message)
        {
            SendErrorMessageEvent.OnNext(message);
        }

        /// <summary>
        ///     Invokes the subscription handlers on all arbitrary message subscribers
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected void OnMessageReceived(PushMessage message)
        {
            SendPushMessageEvent.OnNext(message);
        }

        /// <inheritdoc />
        public void DumpMessageToConsole(PushMessage message)
        {
            Console.WriteLine(MessageToString(message));
        }

        /// <inheritdoc />
        public void SubscribeConsoleDisplay()
        {
            if (ConsoleSubscriptionActive == false) ConsoleSubscription = AnyMessageNotification.Subscribe(DumpMessageToConsole);

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
        ///     Creates a display string from a message event that contains the sender information
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string MessageToString(PushMessage message)
        {
            return $"Message From: {message.Sender}{Environment.NewLine}{Environment.NewLine}{message}";
        }

        /// <summary>
        ///     Locks the sender lock and counts the number of uncompleted senders up
        /// </summary>
        protected void PrepareSending()
        {
            AwaitCompletion++;
        }

        /// <summary>
        ///     Locks the sender object and counts the number of unfinished messages down and notifies awaiters of completion
        /// </summary>
        protected void FinishSending()
        {
            AwaitCompletion--;
        }
    }
}