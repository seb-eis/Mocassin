using System;
using ICon.Framework.Async;

namespace ICon.Framework.Messaging
{
    /// <summary>
    /// Interface for all push message systems that distribute messages through observable subscriptions (Can be Sync or Async)
    /// </summary>
    public interface IPushMessageSystem
    {
        /// <summary>
        /// Observable for error message subscriptions
        /// </summary>
        IObservable<ErrorMessage> WhenErrorMessageSend { get; }

        /// <summary>
        /// Observable for info message subscriptions
        /// </summary>
        IObservable<InfoMessage> WhenInfoMessageSend { get; }

        /// <summary>
        /// Observable for warning message subscriptions
        /// </summary>
        IObservable<WarningMessage> WhenWarningMessageSend { get; }

        /// <summary>
        /// Observable for subscriptions to all messages
        /// </summary>
        IObservable<PushMessage> WhenMessageSend { get; }

        /// <summary>
        /// Flag if the console output is subscribed to the messages
        /// </summary>
        bool ConsoleSubscriptionActive { get; }

        /// <summary>
        /// Sends an error message to all subscribers of error messages
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(ErrorMessage message);

        /// <summary>
        /// Send an info message to all subscribers of info messages
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(InfoMessage message);

        /// <summary>
        /// Send a warning message to all subscribers of warning messages
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(WarningMessage message);

        /// <summary>
        /// Function that dumps a push message to the console
        /// </summary>
        /// <param name="message"></param>
        void DumpMessageToConsole(PushMessage message);

        /// <summary>
        /// Subscribes the console dumb of the messages if not already subscribed
        /// </summary>
        void SubscribeConsoleDisplay();

        /// <summary>
        /// Cancels the console dumb subscription if present
        /// </summary>
        void UnsubscribeConsoleDisplay();
    }
}