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
        ///     Get the warning sequence
        /// </summary>
        /// <returns></returns>
        IEnumerable<WarningMessage> GetWarnings();

        /// <summary>
        ///     Merges the data from the other conflict report into this one
        /// </summary>
        /// <param name="other"></param>
        void Merge(IConflictReport other);
    }
}