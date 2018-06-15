using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Operations
{
    /// <summary>
    /// Represents a report about a subject that could fail or be a success an contains information
    /// </summary>
    public interface IReport
    {
        /// <summary>
        /// Flag that indicates if the report was created due to success or failure
        /// </summary>
        bool IsGood { get; }
    }
}
