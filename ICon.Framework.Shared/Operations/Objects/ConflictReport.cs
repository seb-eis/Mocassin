using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Messaging;
using Newtonsoft.Json;

namespace Mocassin.Framework.Operations
{
    /// <inheritdoc />
    public class ConflictReport : IConflictReport
    {
        /// <inheritdoc />
        public bool IsGood { get; protected set; }

        /// <summary>
        ///     List of contained warnings
        /// </summary>
        public List<WarningMessage> Warnings { get; protected set; }

        /// <summary>
        ///     List of inner reports in cases where resolving or updating has triggered other resolvers or update actions
        /// </summary>
        public List<IConflictReport> InnerReports { get; protected set; }

        /// <summary>
        ///     Creates new conflict resolver report with empty warning list
        /// </summary>
        public ConflictReport()
        {
            IsGood = true;
            Warnings = new List<WarningMessage>();
            InnerReports = new List<IConflictReport>();
        }

        /// <inheritdoc />
        public IEnumerable<WarningMessage> GetWarnings()
        {
            return Warnings.AsEnumerable();
        }

        /// <inheritdoc />
        public void Merge(IConflictReport other)
        {
            if (other != null)
                InnerReports.Add(other);
        }

        /// <summary>
        ///     Adds a new warning message to the report. Automatically un-sets the good flag if the warning is critical
        /// </summary>
        /// <param name="message"></param>
        public void AddWarning(WarningMessage message)
        {
            if (message.IsCritical) IsGood = false;
            Warnings.Add(message);
        }

        /// <summary>
        ///     Creates the default report for cases that cannot be automatically resolved because no resolver exists
        /// </summary>
        /// <returns></returns>
        public static ConflictReport CreateNoResolveRequiredReport(object obj)
        {
            return new ConflictReport
            {
                IsGood = true,
                Warnings =
                {
                    new WarningMessage(null, $"No conflict resolving required for {obj} was required")
                }
            };
        }

        /// <summary>
        ///     Returns json string of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}