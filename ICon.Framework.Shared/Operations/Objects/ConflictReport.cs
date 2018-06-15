using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

using ICon.Framework.Messaging;

namespace ICon.Framework.Operations
{
    /// <summary>
    /// Contains information about potential data conflicts and if/how the conflict resolver of the affiliated manager resolved the problem
    /// </summary>
    public class ConflictReport : IConflictReport
    {
        /// <summary>
        /// Flag that indicates if conflict resolving was successfull
        /// </summary>
        public bool IsGood { get; protected set; }

        /// <summary>
        /// List of contained warnings
        /// </summary>
        public List<WarningMessage> Warnings { get; protected set; }

        /// <summary>
        /// List of inner reports in cases where resolving or updating has triggerd other resolvers or updaters
        /// </summary>
        public List<IConflictReport> InnerReports { get; protected set; }

        /// <summary>
        /// Creates new conflict resolver report with empty warning list
        /// </summary>
        public ConflictReport()
        {
            IsGood = true;
            Warnings = new List<WarningMessage>();
            InnerReports = new List<IConflictReport>();
        }

        /// <summary>
        /// Get the enumerator for the warnigs list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<WarningMessage> GetWarningEnmuerator()
        {
            return Warnings.GetEnumerator();
        }

        /// <summary>
        /// Adds a new warning message to the report. Automatically unsets the good flag if the warning is critical
        /// </summary>
        /// <param name="message"></param>
        public void AddWarning(WarningMessage message)
        {
            if (message.IsCritical)
            {
                IsGood = false;
            }
            Warnings.Add(message);
        }

        /// <summary>
        /// Creates the default report for cases that cannot be automatically resolved because no resolver exists
        /// </summary>
        /// <returns></returns>
        public static ConflictReport CreateNoResolveRequiredReport(object obj)
        {
            return new ConflictReport()
            {
                IsGood = true,
                Warnings =
                {
                    new WarningMessage(null, $"No conflict resolving required for {obj.ToString()} was required")
                }
            };
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
