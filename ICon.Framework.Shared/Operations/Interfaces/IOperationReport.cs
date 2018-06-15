using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Operations
{
    /// <summary>
    /// Represents an information object about success or failure of a model change attempt by an outside source
    /// </summary>
    public interface IOperationReport : IReport
    {
        /// <summary>
        /// Flag that indicates if a program error occured during the change attempt
        /// </summary>
        Boolean IsError { get; }

        /// <summary>
        /// Flag that indiactes that the project returned a busy signal because an input operation is already in progress
        /// </summary>
        Boolean IsBusySignal { get; }

        /// <summary>
        /// String that represents a short description of the operation
        /// </summary>
        String OperationDescription { get; }

        /// <summary>
        /// Containes the exceptions responsible for the error flag (Null if no error occured)
        /// </summary>
        IEnumerator<Exception> GetExceptionsEnumerator();

        /// <summary>
        /// Access to the validation information produced during the model change attempt 
        /// </summary>
        IValidationReport ValidationReport { get; }

        /// <summary>
        /// Access to the resolver report if an operation called the conflict resolver
        /// </summary>
        IConflictReport ConflictReport { get; }
    }
}
