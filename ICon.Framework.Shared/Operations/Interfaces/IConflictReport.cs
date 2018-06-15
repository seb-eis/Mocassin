using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Messaging;

namespace ICon.Framework.Operations
{
    /// <summary>
    /// Represents an information object on data conflict occurence and potential resolving actions by a data conflict resolver
    /// </summary>
    public interface IConflictReport : IReport
    {
        /// <summary>
        /// Get the enumerator for contained warnings messages
        /// </summary>
        /// <returns></returns>
        IEnumerator<WarningMessage> GetWarningEnmuerator();
    }
}
