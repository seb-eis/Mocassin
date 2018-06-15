using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;

using ICon.Framework.Async;

namespace ICon.Framework.Messaging
{
    /// <summary>
    /// Synchronous reactive push messaging system to distribute messages through subscriptions to IObservables
    /// </summary>
    public class SyncMessageSystem : IPushMessageSystem
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

        /// <summary>
        /// Observable for subscriptions to info messages (Hot observable)
        /// </summary>
        public IObservable<InfoMessage> WhenInfoMessageSend => InfoMessageSubject.AsObservable();


        /// <summary>
        /// Observable for subscriptions to warning messages (Hot observable)
        /// </summary>
        public IObservable<WarningMessage> WhenWarningMessageSend => WarningMessageSubject.AsObservable();


        /// <summary>
        /// Observable for subscriptions to error messages (Hot observable)
        /// </summary>
        public IObservable<ErrorMessage> WhenErrorMessageSend => ErrorMessageSubject.AsObservable();


        /// <summary>
        /// Observable for subscriptions to all messages (Hot observable)
        /// </summary>
        public IObservable<PushMessage> WhenMessageSend => PushMessageSubject.AsObservable();

        /// <summary>
        /// Boolean that indicates if the message display callback to System.Console is currently active or not
        /// </summary>
        public Boolean ConsoleSubscriptionActive { get; private set; }

        /// <summary>
        /// Stores the console subscription disposable
        /// </summary>
        private IDisposable ConsoleSubscription { get; set; }

        /// <summary>
        /// Counts how many messages are currently send and await completion
        /// </summary>
        public Int32 AwaitCompletion { get; protected set; }

        /// <summary>
        /// Creates new sync message system for push notifications
        /// </summary>
        public SyncMessageSystem()
        {
            InfoMessageSubject = new Subject<InfoMessage>();
            WarningMessageSubject = new Subject<WarningMessage>();
            ErrorMessageSubject = new Subject<ErrorMessage>();
            PushMessageSubject = new Subject<PushMessage>();
        }

        /// <summary>
        /// Takes an information message and distributes the message to all registered observers
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(InfoMessage message)
        {
            if (message != null)
            {
                PrepareSending();
                OnInfoMessageReceived(message);
                OnMessageReceived(message);
                FinishSending();
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
                PrepareSending();
                OnErrorMessageReceived(message);
                OnMessageReceived(message);
                FinishSending();
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
                PrepareSending();
                OnWarningMessageReceived(message);
                OnMessageReceived(message);
                FinishSending();
            }
        }

        /// <summary>
        /// Takes arbitrary operation message and distributes the message to all registered observers
        /// </summary>
        /// <param name="message"></param>
        protected void SendMessage(PushMessage message)
        {
            if (message != null)
            {
                OnMessageReceived(message);
            }
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
