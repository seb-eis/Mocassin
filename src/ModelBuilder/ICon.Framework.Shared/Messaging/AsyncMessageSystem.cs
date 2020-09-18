using System;
using System.Threading.Tasks;
using Mocassin.Framework.Events;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    ///     Asynchronous reactive push messaging system to distribute messages through subscriptions to IObservables
    /// </summary>
    public class AsyncMessageSystem : IPushMessageSystem
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
        ///     Creates new async message system for push notifications
        /// </summary>
        public AsyncMessageSystem()
        {
            SendInfoMessageEvent = new ReactiveEvent<InfoMessage>();
            SendWarningMessageEvent = new ReactiveEvent<WarningMessage>();
            SendErrorMessageEvent = new ReactiveEvent<ErrorMessage>();
            SendPushMessageEvent = new ReactiveEvent<PushMessage>();
        }

        /// <inheritdoc />
        public void SendMessage(InfoMessage message)
        {
            if (message == null) return;
            OnInfoMessageReceived(message);
            OnMessageReceived(message);
        }

        /// <inheritdoc />
        public void SendMessage(ErrorMessage message)
        {
            if (message == null) return;
            OnErrorMessageReceived(message);
            OnMessageReceived(message);
        }

        /// <inheritdoc />
        public void SendMessage(WarningMessage message)
        {
            if (message == null) return;
            OnWarningMessageReceived(message);
            OnMessageReceived(message);
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
        ///     Creates a new task that sends the information message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        protected Task OnInfoMessageReceived(InfoMessage message)
        {
            return Task.Run(() => SendInfoMessageEvent.OnNext(message));
        }

        /// <summary>
        ///     Creates a new task that sends the warning message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        protected Task OnWarningMessageReceived(WarningMessage message)
        {
            return Task.Run(() => SendWarningMessageEvent.OnNext(message));
        }

        /// <summary>
        ///     Creates a new task that sends the error message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        protected Task OnErrorMessageReceived(ErrorMessage message)
        {
            return Task.Run(() => SendErrorMessageEvent.OnNext(message));
        }

        /// <summary>
        ///     Creates a new task that sends the message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Task OnMessageReceived(PushMessage message)
        {
            return Task.Run(() => SendPushMessageEvent.OnNext(message));
        }

        /// <summary>
        ///     Creates a display string from a message event that contains the sender information
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string MessageToString(PushMessage message) => $"Message From: {message.Sender}{Environment.NewLine}{Environment.NewLine}{message}";
    }
}