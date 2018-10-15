using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ICon.Framework.Messaging
{
    /// <summary>
    ///     Asynchronous reactive push messaging system to distribute messages through subscriptions to IObservables
    /// </summary>
    public class AsyncMessageSystem : IPushMessageSystem
    {
        /// <summary>
        ///     Subject for info messages
        /// </summary>
        private Subject<InfoMessage> InfoMessaging { get; }

        /// <summary>
        ///     Subject for error messages
        /// </summary>
        private Subject<ErrorMessage> ErrorMessaging { get; }

        /// <summary>
        ///     Subject for warning messages
        /// </summary>
        private Subject<WarningMessage> WarningMessaging { get; }

        /// <summary>
        ///     Subject for arbitrary push messages
        /// </summary>
        private Subject<PushMessage> AllMessaging { get; }

        /// <inheritdoc />
        public IObservable<InfoMessage> WhenInfoMessageSend => InfoMessaging.AsObservable();

        /// <inheritdoc />
        public IObservable<WarningMessage> WhenWarningMessageSend => WarningMessaging.AsObservable();

        /// <inheritdoc />
        public IObservable<ErrorMessage> WhenErrorMessageSend => ErrorMessaging.AsObservable();

        /// <inheritdoc />
        public IObservable<PushMessage> WhenMessageSend => AllMessaging.AsObservable();

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
            InfoMessaging = new Subject<InfoMessage>();
            WarningMessaging = new Subject<WarningMessage>();
            ErrorMessaging = new Subject<ErrorMessage>();
            AllMessaging = new Subject<PushMessage>();
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

        /// <summary>
        ///     Creates a new task that sends the information message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        protected Task OnInfoMessageReceived(InfoMessage message)
        {
            return Task.Run(() => InfoMessaging.OnNext(message));
        }

        /// <summary>
        ///     Creates a new task that sends the warning message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        protected Task OnWarningMessageReceived(WarningMessage message)
        {
            return Task.Run(() => WarningMessaging.OnNext(message));
        }

        /// <summary>
        ///     Creates a new task that sends the error message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        protected Task OnErrorMessageReceived(ErrorMessage message)
        {
            return Task.Run(() => ErrorMessaging.OnNext(message));
        }

        /// <summary>
        ///     Creates a new task that sends the message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Task OnMessageReceived(PushMessage message)
        {
            return Task.Run(() => AllMessaging.OnNext(message));
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
                ConsoleSubscription = WhenMessageSend.Subscribe(DumpMessageToConsole);

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
    }
}