using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ICon.Framework.Events;
using ICon.Framework.Operations;

namespace ICon.Framework.Messaging
{
    /// <summary>
    /// Async reporting system to distribute reports to subscribers from a running operation thread
    /// </summary>
    public class AsyncReportSystem
    {
        /// <summary>
        /// Disposable for the activation of operation report console dumps
        /// </summary>
        protected IDisposable ConsoleSubscription { get; set; }

        /// <summary>
        /// Reactive event provider for operation report distribution and subscription
        /// </summary>
        protected EventProvider<IOperationReport> OnOperationReports { get; set; }

        /// <summary>
        /// Reactive event provider for validation report distribution and subscription
        /// </summary>
        protected EventProvider<IValidationReport> OnValidationReports { get; set; }

        /// <summary>
        /// Reactive event provider for conflict report distribution and subscription
        /// </summary>
        protected EventProvider<IConflictReport> OnConflictReports { get; set; }

        /// <summary>
        /// Obeservable for subscriptions to push based notifications awith new operation reports
        /// </summary>
        public IObservable<IOperationReport> WhenOperationReportSend => OnOperationReports.AsObservable();

        /// <summary>
        /// Obeservable for subscriptions to push based notifications awith new validation reports
        /// </summary>
        public IObservable<IValidationReport> WhenValidationReportSend => OnValidationReports.AsObservable();

        /// <summary>
        /// Obeservable for subscriptions to push based notifications awith new conflict reports
        /// </summary>
        public IObservable<IConflictReport> WhenConflictReportSend => OnConflictReports.AsObservable();

        /// <summary>
        /// Sends an operation report in a new thread to all subscribers
        /// </summary>
        /// <param name="report"></param>
        public void Send(IOperationReport report)
        {
            OnOperationReports.OnNextAsync(report);
        }
    }
}
