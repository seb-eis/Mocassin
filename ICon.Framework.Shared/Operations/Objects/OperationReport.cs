using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mocassin.Framework.Exceptions;
using Newtonsoft.Json;

namespace Mocassin.Framework.Operations
{
    /// <inheritdoc />
    public class OperationReport : IOperationReport
    {
        /// <inheritdoc />
        public string OperationDescription { get; set; }

        /// <inheritdoc />
        public bool IsGood { get; protected set; }

        /// <inheritdoc />
        public bool IsError { get; protected set; }

        /// <inheritdoc />
        public bool HasValidationError => ValidationReport != null && ValidationReport.GetWarnings().Any(x => x.IsCritical);

        /// <inheritdoc />
        public bool HasUnsolvedConflict => ConflictReport != null && ConflictReport.GetWarnings().Any(x => x.IsCritical);

        /// <summary>
        ///     Flag that indicates if the operation has caused on demand data to expire
        /// </summary>
        public bool IsCacheExpired { get; set; }

        /// <inheritdoc />
        public bool IsBusySignal { get; protected set; }

        /// <inheritdoc />
        public IList<Exception> Exceptions { get; set; }

        /// <inheritdoc />
        public IValidationReport ValidationReport { get; set; }

        /// <inheritdoc />
        public IConflictReport ConflictReport { get; set; }

        public DateTime TimeStamp { get; }

        /// <summary>
        ///     Creates new unnamed operation result that is by default a success until set otherwise
        /// </summary>
        public OperationReport()
        {
            TimeStamp = DateTime.Now;
            IsGood = true;
        }

        /// <summary>
        ///     Creates new shortly described operation result that is a success by default
        /// </summary>
        /// <param name="description"></param>
        public OperationReport(string description)
            : this()
        {
            OperationDescription = description;
        }

        /// <summary>
        ///     Sets the validation result and updates the affiliated flags
        /// </summary>
        /// <param name="report"></param>
        public void SetValidationReport(IValidationReport report)
        {
            if (report == null)
                return;

            ValidationReport = report;
            IsGood = IsGood && report.IsGood;
        }

        /// <summary>
        ///     Sets the conflict resolver report and updates affiliated flags if required
        /// </summary>
        /// <param name="report"></param>
        public void SetConflictReport(IConflictReport report)
        {
            if (report == null)
                return;

            ConflictReport = report;
            IsGood = IsGood && report.IsGood;
        }

        /// <summary>
        ///     Adds an exception and updates the affiliated flags
        /// </summary>
        /// <param name="exception"></param>
        public void AddException(Exception exception)
        {
            if (Exceptions == null)
                Exceptions = new List<Exception>();

            if (exception == null)
                return;

            Exceptions.Add(exception);
            IsGood = false;
            IsError = true;
        }

        /// <inheritdoc />
        public void Merge(IOperationReport other)
        {
            foreach (var exception in other.Exceptions)
            {
                AddException(exception);
            }

            ConflictReport = ConflictReport ?? new ConflictReport();
            ValidationReport = ValidationReport ?? new ValidationReport();

            ConflictReport.Merge(other.ConflictReport);
            ValidationReport.Merge(other.ValidationReport);

            IsGood = IsGood && ValidationReport.IsGood && ConflictReport.IsGood;
        }

        /// <summary>
        ///     Marks the operation result as a busy signal without changing already existing content
        /// </summary>
        /// <returns></returns>
        public OperationReport AsBusySignal()
        {
            IsBusySignal = true;
            IsGood = false;
            return this;
        }

        /// <summary>
        ///     Creates a new model operation result that represents an input argument failure due to unexpected type
        /// </summary>
        /// <param name="unexpectedType"></param>
        /// <param name="expectedTypes"></param>
        /// <returns></returns>
        public static OperationReport MakeUnexpectedTypeResult(Type unexpectedType, params Type[] expectedTypes)
        {
            if (unexpectedType == null)
                throw new ArgumentNullException(nameof(unexpectedType));

            if (expectedTypes == null)
                throw new ArgumentNullException(nameof(expectedTypes));

            var result = new OperationReport("Model object handler pipeline failed due to unexpected type");
            var builder = new StringBuilder();
            builder.AppendLine("Expected types :");
            foreach (var item in expectedTypes) 
                builder.AppendLine(item.ToString());

            result.AddException(new InvalidPipelineInputException(builder.ToString(), unexpectedType));
            return result;
        }

        /// <summary>
        ///Creates a new <see cref="OperationReport"/> for model build errors
        /// </summary>
        /// <param name="description"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static OperationReport MakeObjectBuildErrorReport(string description, string message)
        {
            var report = new OperationReport(description);
            report.AddException(new InvalidOperationException($"Object build error:\n{message}"));
            return report;
        }

        /// <summary>
        ///     Returns json string of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}