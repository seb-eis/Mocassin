using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Messaging;

namespace ICon.Framework.Operations
{
    /// <summary>
    /// Represents an information object about success or failure of a model data validation
    /// </summary>
    public interface IValidationReport : IReport
    {
        /// <summary>
        /// Get an enumerator to access the stored warning messages
        /// </summary>
        /// <returns></returns>
        IEnumerator<WarningMessage> GetWarningsEnumerator();
    }
}
