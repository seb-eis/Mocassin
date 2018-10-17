using System.Collections.Generic;
using Mocassin.Framework.Messaging;

namespace Mocassin.Framework.Operations
{
    /// <summary>
    ///     Represents an information object on data conflict occurence and potential resolving actions by a data conflict
    ///     resolver
    /// </summary>
    public interface IConflictReport : IReport
    {
        /// <summary>
        ///     Get the enumerator for contained warnings messages
        /// </summary>
        /// <returns></returns>
        IEnumerator<WarningMessage> GetWarningEnumerator();
    }
}