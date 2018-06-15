using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

using ICon.Framework.Exceptions;

namespace ICon.Framework.Operations
{
    /// <summary>
    /// Represents an input result for user induced operations e.g. model data addition or removal
    /// </summary>
    public class OperationReport : IOperationReport
    {
        /// <summary>
        /// String that represents a short description of the operation
        /// </summary>
        public string OperationDescription { get; protected set; }

        /// <summary>
        /// Flag that indicates if the input operation was successful
        /// </summary>
        public bool IsGood { get; protected set; }

        /// <summary>
        /// Flag that indicates if the input operation caused an exception
        /// </summary>
        public bool IsError { get; protected set; }

        /// <summary>
        /// Flag that indicates if the operation has caused on demand data to expire
        /// </summary>
        public bool CacheExpired { get; set; }

        /// <summary>
        /// Flag that indicates that the project reported busy because an input operation is currently in progress
        /// </summary>
        public bool IsBusySignal { get; protected set; }

        /// <summary>
        /// Contains potentially occured exception
        /// </summary>
        public List<Exception> Exceptions { get; protected set; }

        /// <summary>
        /// Contains the validation result if the input operation caused validation errors
        /// </summary>
        public IValidationReport ValidationReport { get; protected set; }

        /// <summary>
        /// Contains the resolver report if the input operation triggered a conflict resolving handler
        /// </summary>
        public IConflictReport ConflictReport { get; protected set; }

        /// <summary>
        /// Cretaes new unnamed operation result that is by default a success until set otherwise
        /// </summary>
        public OperationReport()
        {
            IsGood = true;
        }

        /// <summary>
        /// Creates new shortly described operation result that is a success by default
        /// </summary>
        /// <param name="description"></param>
        public OperationReport(string description) : this()
        {
            OperationDescription = description;
        }

        /// <summary>
        /// Sets the validation result and updates the affiliated flags
        /// </summary>
        /// <param name="report"></param>
        public void SetValidationReport(IValidationReport report)
        {
            if (report != null)
            {
                ValidationReport = report;
                IsGood = (IsGood && report.IsGood) ? true : false;
            }
        }

        /// <summary>
        /// Sets the conflict resolver report and updates affiliated flags if required
        /// </summary>
        /// <param name="report"></param>
        public void SetConflictReport(IConflictReport report)
        {
            if (report != null)
            {
                ConflictReport = report;
                IsGood = (IsGood && report.IsGood) ? true : false;
            }
        }

        /// <summary>
        /// Adds an exception and updates the affiliated flags
        /// </summary>
        /// <param name="exception"></param>
        public void AddException(Exception exception)
        {
            if (Exceptions == null)
            {
                Exceptions = new List<Exception>();
            }
            if (exception != null)
            {
                Exceptions.Add(exception);
                IsGood = false;
                IsError = true;
            }
        }

        /// <summary>
        /// Marks the operation result as a busy signal without changing already existing content
        /// </summary>
        /// <returns></returns>
        public OperationReport AsBusySignal()
        {
            IsBusySignal = true;
            IsGood = false;
            return this;
        }

        /// <summary>
        /// Creates a new model operation result that represents an input argument failure due to unexpected type
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static OperationReport MakeUnexpectedTypeResult(Type unexpectedType, params Type[] expectedTypes)
        {
            if (unexpectedType == null)
            {
                throw new ArgumentNullException(nameof(unexpectedType));
            }

            if (expectedTypes == null)
            {
                throw new ArgumentNullException(nameof(expectedTypes));
            }

            var result = new OperationReport("Model object handler pipeline failed due to unexpected type");
            var builder = new StringBuilder();
            builder.AppendLine("Expected types :");
            foreach (var item in expectedTypes)
            {
                builder.AppendLine(item.ToString());
            }
            result.AddException(new UnexpectedInputTypeException(builder.ToString(), unexpectedType));
            return result;
        }

        /// <summary>
        /// Get the enumerator for the exceptions
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Exception> GetExceptionsEnumerator()
        {
            return Exceptions.GetEnumerator();
        }

        /// <summary>
        /// Returns json string of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
