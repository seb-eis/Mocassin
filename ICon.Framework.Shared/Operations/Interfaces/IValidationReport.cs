using System.Collections.Generic;
using Mocassin.Framework.Messaging;

namespace Mocassin.Framework.Operations
{
    /// <summary>
    ///     Represents an information object about success or failure of a model data validation
    /// </summary>
    public interface IValidationReport : IReport
    {
        /// <summary>
        ///     Get all stored warnings messages
        /// </summary>
        /// <returns></returns>
        IEnumerable<WarningMessage> GetWarnings();
    }
}