using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Messaging;
using Newtonsoft.Json;

namespace Mocassin.Framework.Operations
{
    /// <inheritdoc />
    public class ValidationReport : IValidationReport
    {
        /// <inheritdoc />
        public bool IsGood { get; protected set; }

        /// <summary>
        ///     List of warning messages produces during the validation
        /// </summary>
        public List<WarningMessage> Warnings { get; protected set; }

        /// <summary>
        ///     Creates a new validation result that is by default a success
        /// </summary>
        public ValidationReport()
        {
            Warnings = new List<WarningMessage>();
            IsGood = true;
        }

        /// <inheritdoc />
        public ValidationReport(IValidationReport report)
            : this()
        {
            if (report is ValidationReport validationReport)
            {
                IsGood = validationReport.IsGood;
                Warnings = validationReport.Warnings;
            }
            else
            {
                IsGood = report.IsGood;
                AddWarnings(report.GetWarnings());
            }
        }

        /// <summary>
        ///     Adds a warning message and sets validation to failed if the warning is critical
        /// </summary>
        /// <param name="message"></param>
        public void AddWarning(WarningMessage message)
        {
            Warnings.Add(message);
            if (message.IsCritical) IsGood = false;
        }

        /// <summary>
        ///     Address multiple warnings messages and sets validation to failed if one is critical
        /// </summary>
        /// <param name="messages"></param>
        public void AddWarnings(IEnumerable<WarningMessage> messages)
        {
            foreach (var warning in messages) AddWarning(warning);
        }

        /// <inheritdoc />
        public IEnumerable<WarningMessage> GetWarnings()
        {
            return Warnings.AsEnumerable();
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