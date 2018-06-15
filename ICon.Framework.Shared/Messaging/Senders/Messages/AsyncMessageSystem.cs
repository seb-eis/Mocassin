using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Reactive.Subjects;
using System.Reactive.Linq;

using ICon.Framework.Async;

namespace ICon.Framework.Messaging
{
    /// <summary>
    /// Asynchronous reactive push messaging system to distribute messages through subscriptions to IObservables
    /// </summary>
    public class AsyncMessageSystem : IPushMessageSystem
    {
        /// <summary>
        /// Subject for info messages
        /// </summary>
        private Subject<InfoMessage> InfoMessaging { get; }

        /// <summary>
        /// Subject for error messages
        /// </summary>
        private Subject<ErrorMessage> ErrorMessaging { get; }

        /// <summary>
        /// Subject for warning messages
        /// </summary>
        private Subject<WarningMessage> WarningMessaging { get; }

        /// <summary>
        /// Subject for arbitrary push messages
        /// </summary>
        private Subject<PushMessage> AllMessaging { get; }

        /// <summary>
        /// Observable for subscriptions to info messages (Hot observable)
        /// </summary>
        public IObservable<InfoMessage> WhenInfoMessageSend => InfoMessaging.AsObservable();


        /// <summary>
        /// Observable for subscriptions to warning messages (Hot observable)
        /// </summary>
        public IObservable<WarningMessage> WhenWarningMessageSend => WarningMessaging.AsObservable();


        /// <summary>
        /// Observable for subscriptions to error messages (Hot observable)
        /// </summary>
        public IObservable<ErrorMessage> WhenErrorMessageSend => ErrorMessaging.AsObservable();


        /// <summary>
        /// Observable for subscriptions to all messages (Hot observable)
        /// </summary>
        public IObservable<PushMessage> WhenMessageSend => AllMessaging.AsObservable();

        /// <summary>
        /// Boolean that indicates if the message display callback to System.Console is currently active or not
        /// </summary>
        public Boolean ConsoleSubscriptionActive { get; private set; }

        /// <summary>
        /// Stores the console subscription disposable
        /// </summary>
        private IDisposable ConsoleSubscription { get; set; }

        /// <summary>
        /// Creates new async message system for push notifications
        /// </summary>
        public AsyncMessageSystem()
        {
            InfoMessaging = new Subject<InfoMessage>();
            WarningMessaging = new Subject<WarningMessage>();
            ErrorMessaging = new Subject<ErrorMessage>();
            AllMessaging = new Subject<PushMessage>();
        }

        /// <summary>
        /// Takes an information message and distributes the message to all registered observers
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(InfoMessage message)
        {
            if (message != null)
            {
                OnInfoMessageReceived(message);
                OnMessageReceived(message);
            }
        }

        /// <summary>
        /// Takes an error message and distributes the message to all registered observers
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(ErrorMessage message)
        {
            if (message != null)
            {
                OnErrorMessageReceived(message);
                OnMessageReceived(message);
            }
        }

        /// <summary>
        /// Takes a warning message and distributes the message to all registered observers
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(WarningMessage message)
        {
            if (message != null)
            {
                OnWarningMessageReceived(message);
                OnMessageReceived(message);
            }
        }

        /// <summary>
        /// Creates a new task that sends the information message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        protected Task OnInfoMessageReceived(InfoMessage message)
        {
            return Task.Run(() => InfoMessaging.OnNext(message));
        }

        /// <summary>
        /// Creates a new task that sends the warning message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        protected Task OnWarningMessageReceived(WarningMessage message)
        {
            return Task.Run(() => WarningMessaging.OnNext(message));
        }

        /// <summary>
        /// Creates a new task that sends the error message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        protected Task OnErrorMessageReceived(ErrorMessage message)
        {
            return Task.Run(() => ErrorMessaging.OnNext(message));
        }

        /// <summary>
        /// Creates a new task that sends the message to all subscribers
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Task OnMessageReceived(PushMessage message)
        {
            return Task.Run(() => AllMessaging.OnNext(message));
        }

        /// <summary>
        /// Dumbs operation message string to the console
        /// </summary>
        /// <param name="message"></param>
        public void DumpMessageToConsole(PushMessage message)
        {
            Console.WriteLine(MessageToString(message));
        }

        /// <summary>
        /// Subscribes the console dump to receive operation messages
        /// </summary>
        public void SubscribeConsoleDisplay()
        {
            if (ConsoleSubscriptionActive == false)
            {
                ConsoleSubscription = WhenMessageSend.Subscribe(message => DumpMessageToConsole(message));
            }
            ConsoleSubscriptionActive = true;
        }

        /// <summary>
        /// Unsubscribes the console dump from receiving operation messages
        /// </summary>
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
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private String MessageToString(PushMessage message)
        {
            return $"Message From: {message.Sender.ToString()}{Environment.NewLine}{Environment.NewLine}{message.ToString()}";
        }
    }
}
