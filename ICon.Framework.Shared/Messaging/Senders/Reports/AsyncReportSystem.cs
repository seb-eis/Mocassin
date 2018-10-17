using System;
using Mocassin.Framework.Events;
using Mocassin.Framework.Operations;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    ///     Async reporting system to distribute reports to subscribers from a running operation thread
    /// </summary>
    public class AsyncReportSystem
    {
        /// <summary>
        ///     Disposable for the activation of operation report console dumps
        /// </summary>
        protected IDisposable ConsoleSubscription { get; set; }

        /// <summary>
        ///     Reactive event provider for operation report distribution and subscription
        /// </summary>
        protected ReactiveEvent<IOperationReport> OnOperationReports { get; set; }

        /// <summary>
        ///     Reactive event provider for validation report distribution and subscription
        /// </summary>
        protected ReactiveEvent<IValidationReport> OnValidationReports { get; set; }

        /// <summary>
        ///     Reactive event provider for conflict report distribution and subscription
        /// </summary>
        protected ReactiveEvent<IConflictReport> OnConflictReports { get; set; }

        /// <summary>
        ///     Observable for subscriptions to push based notifications await new operation reports
        /// </summary>
        public IObservable<IOperationReport> WhenOperationReportSend => OnOperationReports.AsObservable();

        /// <summary>
        ///     Observable for subscriptions to push based notifications await new validation reports
        /// </summary>
        public IObservable<IValidationReport> WhenValidationReportSend => OnValidationReports.AsObservable();

        /// <summary>
        ///     Observable for subscriptions to push based notifications await new conflict reports
        /// </summary>
        public IObservable<IConflictReport> WhenConflictReportSend => OnConflictReports.AsObservable();

        /// <summary>
        ///     Sends an operation report in a new thread to all subscribers
        /// </summary>
        /// <param name="report"></param>
        public void Send(IOperationReport report)
        {
            OnOperationReports.OnNextAsync(report);
        }
    }
}