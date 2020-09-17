using System;
using System.Collections.Generic;

namespace Mocassin.Framework.Operations
{
    /// <summary>
    ///     Represents an information object about success or failure of a model change attempt by an outside source
    /// </summary>
    public interface IOperationReport : IReport
    {
        /// <summary>
        ///     Flag that indicates if a program error occured during the change attempt
        /// </summary>
        bool IsError { get; }

        /// <summary>
        ///     Boolean flag that indicates if the report contains a validation failure
        /// </summary>
        bool HasValidationError { get; }

        /// <summary>
        ///     Boolean flag that indicates if the report has relevant conflict handling information
        /// </summary>
        bool HasUnsolvedConflict { get; }

        /// <summary>
        ///     Flag that indicates that the project returned a busy signal because an input operation is already in progress
        /// </summary>
        bool IsBusySignal { get; }

        /// <summary>
        ///     String that represents a short description of the operation
        /// </summary>
        string OperationDescription { get; set; }

        /// <summary>
        ///     Contains potentially occured exception
        /// </summary>
        IList<Exception> Exceptions { get; set; }

        /// <summary>
        ///     Access to the validation information produced during the model change attempt
        /// </summary>
        IValidationReport ValidationReport { get; set; }

        /// <summary>
        ///     Access to the resolver report if an operation called the conflict resolver
        /// </summary>
        IConflictReport ConflictReport { get; set; }

        /// <summary>
        ///     Get the <see cref="DateTime" /> when the report was created
        /// </summary>
        DateTime TimeStamp { get; }

        /// <summary>
        ///     Adds an exception to the operation report
        /// </summary>
        /// <param name="exception"></param>
        void AddException(Exception exception);

        /// <summary>
        ///     Collects all information from the passed operation report and merges the data into this report
        /// </summary>
        /// <param name="other"></param>
        void Merge(IOperationReport other);
    }
}